// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System.Diagnostics;
using System.Windows.Forms;
using AzureAppServiceMigrationTool.Helpers;

namespace AzureAppServiceMigrationTool.Controls
{
    public partial class PublishStatus : UserControl
    {
        private readonly string _dbTraceFilename;
        private readonly string _contentTraceFilename;
        private readonly bool _hasDatabase;
        private bool _dbPublished;
        private bool _contentPublished;

        public PublishStatus(string sitename, string siteLink, bool hasDatabase, string contentTraceFileName, string dbTraceFilename = null)
        {
            InitializeComponent();

            siteNameLabel.Text = sitename;
            siteLinkLabel.Text = siteLink;

            _hasDatabase = hasDatabase;
            _contentTraceFilename = contentTraceFileName;
            _dbTraceFilename = hasDatabase ? dbTraceFilename : null;
            siteProgressBar.Minimum = 0;
            siteProgressBar.Maximum = 100;
            siteProgressBar.Value = 0;
            siteProgressBar.Step = 1;
            ResetStatus();
        }

        public PublishStatus(string sitename, string siteFailedError)
        {
            InitializeComponent();

            siteNameLabel.Text = sitename;

            SiteStatusBox.SizeMode = PictureBoxSizeMode.StretchImage;
            SiteStatusBox.Image = Properties.Resources.Error;

            siteStatusMessage.Text = siteFailedError;
            DbStatusBox.Visible = false;
            dbStatusMessage.Visible = false;
            dbStatusLink.Visible = false;
            siteStatusLink.Visible = false;
            siteLinkLabel.Visible = false;
        }

        public void UpdateMaxProgressBarvalue(int maxValue)
        {
            this.siteProgressBar.Invoke(new MethodInvoker(delegate()
            {
                this.siteProgressBar.Maximum = maxValue;
            }));
        }

        public void PerformStep()
        {
            this.siteProgressBar.Invoke(new MethodInvoker(delegate() { this.siteProgressBar.PerformStep(); }));
        }

        public void ResetDbStatus()
        {
            if (_hasDatabase)
            {
                if (!this.InvokeRequired)
                {
                    DbStatusBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    DbStatusBox.Image = Properties.Resources.AnimatedProgressBar;
                    dbStatusLink.Visible = false;
                }
                else
                {
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        DbStatusBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        DbStatusBox.Image = Properties.Resources.AnimatedProgressBar;
                        dbStatusLink.Visible = false;
                    }));
                }
            }
        }

        public void ResetContentStatus()
        {
            if (!this.InvokeRequired)
            {
                SiteStatusBox.SizeMode = PictureBoxSizeMode.StretchImage;
                SiteStatusBox.Image = Properties.Resources.AnimatedProgressBar;
                siteStatusLink.Visible = false;
                siteLinkLabel.Visible = false;
            }
            else
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    SiteStatusBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    SiteStatusBox.Image = Properties.Resources.AnimatedProgressBar;
                    siteStatusLink.Visible = false;
                    siteLinkLabel.Visible = false;
                }));
            }
        }

        public void ResetStatus()
        {
            SiteStatusBox.SizeMode = PictureBoxSizeMode.StretchImage;
            SiteStatusBox.Image = Properties.Resources.AnimatedProgressBar;
            DbStatusBox.SizeMode = PictureBoxSizeMode.StretchImage;
            DbStatusBox.Image = Properties.Resources.AnimatedProgressBar;
            if (!_hasDatabase)
            {
                DbStatusBox.Visible = false;
                dbStatusMessage.Visible = false;
                dbStatusLink.Visible = false;
            }

            dbStatusLink.Visible = false;
            siteStatusLink.Visible = false;
            siteLinkLabel.Visible = false;
        }

        public void DbPublished(bool success)
        {
            _dbPublished = success;
            this.Invoke(new MethodInvoker(delegate()
                {
                    DbStatusBox.Image = success ? Properties.Resources.OK : Properties.Resources.Error;
                    dbStatusLink.Visible = !success;
                    siteLinkLabel.Visible = success;
                }));
        }

        public void ContentPublished(bool success)
        {
            _contentPublished = success;

            this.Invoke(new MethodInvoker(delegate()
            {
                SiteStatusBox.Image = success ? Properties.Resources.OK : Properties.Resources.Error;
                siteStatusLink.Visible = !success;
                siteLinkLabel.Visible = success && (!_hasDatabase || (_hasDatabase && _dbPublished));
                if (success || SiteStatusBox.Image == Properties.Resources.OK)
                {
                    if (siteProgressBar.Maximum == 0)
                    {
                        siteProgressBar.Maximum = 1;
                        siteProgressBar.Value = 1;
                    }
                    else
                    {
                        siteProgressBar.Value = siteProgressBar.Maximum;
                    }
                }

            }));
        }

        private void dbStatusLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_hasDatabase)
            {
                Process.Start("notepad.exe", _dbTraceFilename);
            }
        }

        private void siteStatusLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("notepad.exe", _contentTraceFilename);
        }

        private void siteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Helper.OpenWebLink(siteLinkLabel.Text);
        }
    }
}
