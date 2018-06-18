// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AzureAppServiceMigrationTool.Helpers;
using AzureAppServiceMigrationTool.ObjectModel;

namespace AzureAppServiceMigrationTool.Controls
{
    public partial class SendFeedbackControl : UserControl, IWizardStep
    {
        private IISServers IISServers;
        private const int MaxBufferLength = 8000;
        private StringBuilder ErrorBuffer;

        public SendFeedbackControl()
        {
            InitializeComponent();
            ErrorBuffer = new StringBuilder();
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
            busyPictureBox.Visible = true;
            if (state != null)
            {
                this.IISServers = (IISServers)state;
            }

            long builderLength = 0;
            if (ErrorBuffer.Length > 0)
            {
                ErrorBuffer.Remove(0, ErrorBuffer.Length);
            }

            foreach (var server in this.IISServers.Servers.Values)
            {
                foreach (Site website in server.Sites.Where(s => s.PublishProfile != null && !string.IsNullOrEmpty(s.SiteCreationError)))
                {
                    ErrorBuffer.AppendFormat("Site: {0} creation failed. {1} (server: {2})\r\n", website.SiteName, website.SiteCreationError, server.Name);
                }
            }

            foreach (var server in this.IISServers.Servers.Values)
            {
                foreach (Site website in server.Sites.Where(s => s.PublishProfile != null && (!s.ContentPublishState || !s.DbPublishState)))
                {
                    if (ErrorBuffer.Length < MaxBufferLength)
                    {
                        string contentTrace = string.Empty;
                        string dbTrace = string.Empty;

                        if (File.Exists(website.PublishProfile.ContentTraceFile))
                        {
                            contentTrace = File.ReadAllText(website.PublishProfile.ContentTraceFile).Replace("<", " ").Replace(">", " ");
                        }

                        if (builderLength + contentTrace.Length <= MaxBufferLength)
                        {
                            ErrorBuffer.AppendLine(contentTrace);
                        }

                        if (File.Exists(website.PublishProfile.DbTraceFile))
                        {
                            dbTrace = File.ReadAllText(website.PublishProfile.DbTraceFile).Replace("<", " ").Replace(">", " ");
                        }

                        if (builderLength + dbTrace.Length <= MaxBufferLength)
                        {
                            ErrorBuffer.AppendLine(dbTrace);
                        }
                    }
                }
            }

            foreach (string url in Helper.UrlsForCookie)
            {
                Helper.InternetSetCookie(url, null, "AZMigrationTool = SkipHeaderAndFooter");
            }

            emailBrowser.Navigate(Helper.UrlCombine(
                     Helper.ScmSitePrimary,
                     Helper.Mail));
            checkMailSettingsTimer.Enabled = true;
        }

        private void FireGoToEvent(WizardSteps step, object state, bool isNavigatingBack)
        {
            EventHandler<GoToWizardStepEventArgs> _goTo = GoTo;
            if (_goTo != null)
            {
                _goTo(this, new GoToWizardStepEventArgs(step, state, isNavigatingBack));
            }
        }

        private void emailBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            busyPictureBox.Visible = true;
        }

        private void emailBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            busyPictureBox.Visible = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void checkMailSettingsTimer_Tick(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                if (emailBrowser.Document != null)
                {
                    HtmlElement mailSentHtmlElement = emailBrowser.Document.GetElementById("mailsentstatus");
                    if (mailSentHtmlElement != null)
                    {
                        try
                        {
                            checkMailSettingsTimer.Enabled = false;
                            HtmlElement mailSubjectHtmlElement = emailBrowser.Document.GetElementById("subject");
                            HtmlElement mailBodyHtmlElement = emailBrowser.Document.GetElementById("body");
                            if (mailSubjectHtmlElement != null && mailBodyHtmlElement != null)
                            {
                                mailSubjectHtmlElement.InnerText = "Feedback";
                                mailBodyHtmlElement.InnerText = ErrorBuffer.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            TraceHelper.Tracer.WriteTrace(ex.ToString());
                        }
                    }
                }
            }
                ));
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.ContentAndDbMigration, this.IISServers, true);
        }
    }
}
