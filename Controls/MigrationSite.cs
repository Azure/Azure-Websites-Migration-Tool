// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CompatCheckAndMigrate.Helpers;
using CompatCheckAndMigrate.ObjectModel;

namespace CompatCheckAndMigrate.Controls
{
    public partial class MigrationSite : UserControl, IWizardStep
    {
        private static readonly Regex RegexInputData = new Regex("value='([^']*)'", RegexOptions.IgnoreCase);
        private static readonly Regex RegexInputNumData = new Regex(@"value=([\d]*)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexGetSiteId = new Regex(@"id=([a-zA-Z0-9-]*)", RegexOptions.IgnoreCase);

        private IISServers IISServers;

        public MigrationSite()
        {
            InitializeComponent();
            this.IISServers = null;
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
            if (state != null)
            {
                this.IISServers = (IISServers)state;
            }

            btnPublish.Visible = false;

            foreach (string url in Helper.UrlsForCookie)
            {
                Helper.InternetSetCookie(url, null, "AZMigrationTool = SkipHeaderAndFooter");
            }

            string urlToNavigate = Helper.UrlCombine(
                     Helper.PostMigratePortal,
                     Helper.Results,
                     Helper.AzureMigrationId);
            if (isNavigatingBack)
            {
                urlToNavigate = Helper.UrlCombine(Helper.ScmSitePrimary, "/picksubscription/index", Helper.AzureMigrationId);
            }

            siteBrowser.Navigate("about:blank");
            siteBrowser.Navigate(urlToNavigate);
            checkPublishSettingsTimer.Enabled = true;
        }

        private void siteBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            busyPictureBox.Visible = false;
        }

        private void FireGoToEvent(WizardSteps step, object state = null)
        {
            EventHandler<GoToWizardStepEventArgs> _goTo = GoTo;
            if (_goTo != null)
            {
                _goTo(this, new GoToWizardStepEventArgs(step, state));
            }
        }

        private void siteBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            busyPictureBox.Visible = true;
        }

        private string GetInputValue(string outerHtml, bool isNumeric)
        {
            var regex = isNumeric ? RegexInputNumData : RegexInputData;
            string content = outerHtml;
            if (!string.IsNullOrEmpty(content))
            {
                var m = regex.Match(content);
                if (m.Success && m.Groups.Count > 1 && !string.IsNullOrEmpty(m.Groups[1].Value))
                {
                    if (isNumeric)
                    {
                        return m.Groups[1].Value;
                    }

                    return m.Groups[1].Value.Replace("&#13;", string.Empty)
                                    .Replace("&#10;", string.Empty)
                                    .Replace("&#9;", string.Empty);
                }
            }

            return string.Empty;
        }

        private Dictionary<string, string> CheckSiteForError()
        {
            Dictionary<string, string> siteErrorMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var totalSitesOuterHtml = string.Empty;
            this.Invoke(new MethodInvoker(delegate()
            {
                if (siteBrowser.Document != null)
                {
                    var siteCountElement = siteBrowser.Document.GetElementById("totalnumberofsites");
                    if (siteCountElement != null)
                    {
                        totalSitesOuterHtml = siteCountElement.OuterHtml;
                    }
                }
            }));

            if (!string.IsNullOrEmpty(totalSitesOuterHtml))
            {
                var siteCountData = GetInputValue(totalSitesOuterHtml, true);
                if (!string.IsNullOrEmpty(siteCountData))
                {
                    int siteCount = 0;
                    Int32.TryParse(siteCountData, out siteCount);
                    for (int i = 0; i < siteCount; i++)
                    {
                        HtmlElement siteCollectionElement = siteBrowser.Document.GetElementById("sitecollection" + i);
                        if (siteCollectionElement != null)
                        {
                            string errorReason = siteCollectionElement.InnerText;
                            string siteIdData = siteCollectionElement.InnerHtml;
                            if (siteIdData.Contains("images/error.png"))
                            {
                                var m = RegexGetSiteId.Match(siteIdData);
                                if (m.Success && m.Groups.Count > 1 && !string.IsNullOrEmpty(m.Groups[1].Value))
                                {
                                    string matchValue = m.Groups[1].Value;
                                    string siteName = matchValue.Substring(0, matchValue.Length - 4);
                                    siteErrorMap[siteName] = errorReason;
                                }
                            }
                        }
                    }
                }
            }

            return siteErrorMap;
        }

        private void checkPublishSettingsTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                string outerHtml = null;

                this.Invoke(new MethodInvoker(delegate()
                {
                    if (siteBrowser.Document != null)
                    {
                        var publishElement = siteBrowser.Document.GetElementById("publishprofile");
                        if (publishElement != null)
                        {
                            outerHtml = publishElement.OuterHtml;
                        }
                    }
                }));

                if (!string.IsNullOrEmpty(outerHtml))
                {
                    try
                    {
                        string value = GetInputValue(outerHtml, false);
                        if (!string.IsNullOrEmpty(value) &&
                            value.Contains("publishProfile"))
                        {
                            checkPublishSettingsTimer.Enabled = false;
                            Dictionary<string, string> errorMap = CheckSiteForError();
                            string publishFilePath =
                                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                        Helper.AzureMigrationId + ".publishsettings");
                            using (var writer = new StreamWriter(publishFilePath, false))
                            {
                                writer.WriteLine(value);
                            }

                            if (this.IISServers.Servers.Values.Any())
                            {
                                // TODO: FIX FOR MULTIPLE SERVERS / SCALE OUT
                                this.IISServers.Servers.Values.First().SetPublishSetting(errorMap, value);

                                this.btnPublish.Invoke(new MethodInvoker(delegate()
                                {
                                    btnPublish.Visible = true;
                                }));
                            }
                            else
                            {
                                Helper.ShowErrorMessageAndExit("Some Error occurred: No sites found to publish");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnPublish_Click(object sender, EventArgs e)
        {
            try
            {
                string outerHtml = null;

                this.Invoke(new MethodInvoker(delegate()
                {
                    if (siteBrowser.Document != null)
                    {
                        var publishElement = siteBrowser.Document.GetElementById("publishprofile");
                        if (publishElement != null)
                        {
                            outerHtml = publishElement.OuterHtml;
                        }
                    }
                }));

                if (!string.IsNullOrEmpty(outerHtml))
                {
                    try
                    {
                        string value = GetInputValue(outerHtml, false);
                        if (!string.IsNullOrEmpty(value) &&
                            value.Contains("publishProfile"))
                        {
                            checkPublishSettingsTimer.Enabled = false;
                            Dictionary<string, string> errorMap = CheckSiteForError();
                            string publishFilePath =
                                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                        Helper.AzureMigrationId + ".publishsettings");
                            using (var writer = new StreamWriter(publishFilePath, false))
                            {
                                writer.WriteLine(value);
                            }

                            // TODO: FIX FOR MULTIPLE SERVERS / SCALE OUT
                            this.IISServers.Servers.Values.First().SetPublishSetting(errorMap, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            var wizardStep = Helper.IsWebDeployInstalled
                ? WizardSteps.ContentAndDbMigration
                : WizardSteps.SiteNotMigrated;

            this.Invoke(new MethodInvoker(delegate()
            {
                FireGoToEvent(wizardStep, this.IISServers);
            }));
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.ReadinessReport, this.IISServers);
        }
    }
}