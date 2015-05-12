// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using CompatCheckAndMigrate.Helpers;
using CompatCheckAndMigrate.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace CompatCheckAndMigrate.Controls
{
    public partial class MigrationCandidatesControl : UserControl, IWizardStep
    {
        private BackgroundWorker _worker;
        internal class SelectedObjects
        {
            public List<Site> SelectedSites { get; set; }
            public List<Database> SelectedDatabases { get; set; }
        };

        public MigrationCandidatesControl()
        {
            InitializeComponent();
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
            this.Sites = new Dictionary<string, Site>();
            this.IISServers = new IISServers();
            ProcessIISInfo();
        }

        private void FireGoToEvent(WizardSteps step, object state = null)
        {
            EventHandler<GoToWizardStepEventArgs> _goTo = GoTo;
            if (_goTo != null)
            {
                _goTo(this, new GoToWizardStepEventArgs(step, state));
            }
        }

        public IISServers IISServers = new IISServers();
        public Dictionary<string, Site> Sites = new Dictionary<string, Site>();

        private void GetSelectedSites()
        {
            SelectedObjects selectedObjs = (SelectedObjects)this.siteTree.Tag;
            if ((null == selectedObjs) ||
                (0 == selectedObjs.SelectedSites.Count())) {
                return;
            }

            foreach (Site selectedSite in selectedObjs.SelectedSites) {
                selectedSite.Databases.Clear();
            }

            foreach (Database selectedDatabase in selectedObjs.SelectedDatabases) {
                selectedDatabase.ParentSite.Add(selectedDatabase);
                selectedDatabase.ParentSite = null; //prevent recursion during serialization
            }

            //List<Site> selectedSites = new List<Site>();
            //this.GetAllSelectedSites(this.siteTree.Nodes[0], selectedSites);
            var applicationPools = new Dictionary<string, ApplicationPool>();
            // reset the sites and AppPools for each server
            foreach (var server in this.IISServers.Servers.Values)
            {
                server.Sites = new List<Site>();
                foreach (var applicationPool in server.AppPools)
                {
                    applicationPools.Add(server.Name + applicationPool.Name, applicationPool);
                }

                server.AppPools = new List<ApplicationPool>();
            }

            // only add the selectedSites and AppPools
            foreach (var selectedSite in selectedObjs.SelectedSites)
            {
                this.IISServers.Servers[selectedSite.ServerName].Sites.Add(selectedSite);
                // add site AppPool
                this.AddApplicationPool(applicationPools[selectedSite.ServerName + selectedSite.AppPoolName], selectedSite.ServerName);
                foreach (var application in selectedSite.Applications)
                {
                    // Add AppPool for each application
                    this.AddApplicationPool(applicationPools[selectedSite.ServerName + application.AppPoolName], selectedSite.ServerName);
                }
            }
        }

        private void AddApplicationPool(ApplicationPool applicationPoolToAdd, string serverName)
        {
            if (!this.IISServers.Servers[serverName].AppPools.Contains(applicationPoolToAdd))
            {
                this.IISServers.Servers[serverName].AppPools.Add(applicationPoolToAdd);
            }
        }

        private void GetAllSelectedSites(TreeNode treeNode, List<Site> sites)
        {
            if (treeNode.Nodes.Count == 0 && treeNode.Checked)
            {
                // checked site node
                sites.Add(this.Sites[treeNode.Name]);
            }
            else
            {
                foreach (TreeNode node in treeNode.Nodes)
                {
                    this.GetAllSelectedSites(node, sites);
                }
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            GetSelectedSites();

            FireGoToEvent(WizardSteps.ReadinessReport, this.IISServers);
        }
        
        private void ProcessIISInfo()
        {
            // clear the siteTree
            this.siteTree.Nodes.Clear();
            // Hide listbox and show busy animation.
            this.descriptionLabel.Visible = this.StartButton.Visible = this.btnBack.Visible = false; // this.websitesCheckedListBox.Visible
            this.busyPictureBox.Visible = this.busyMessageLabel.Visible = true;
            // Start local websites scan in a background thread.
            this._worker = new BackgroundWorker();
            this.busyMessageLabel.Text = "Please wait while we inspect servers for migration candidates";

            this._worker.DoWork += (object doWorkSender, DoWorkEventArgs doWorkEventArgs) =>
            {
                List<IISInfoReader> readers = new List<IISInfoReader>();
                if (RemoteSystemInfos.Servers.Any())
                {
                    foreach (RemoteSystemInfo remoteSystemInfo in RemoteSystemInfos.Servers.Values)
                    {
                        if (remoteSystemInfo.ComputerName == "localhost")
                        {
                            readers.Add(Helper.GetIISInfoReader(Helper.AzureMigrationId, null));
                        }
                        else
                        {
                            readers.Add(Helper.GetIISInfoReader(Helper.AzureMigrationId, remoteSystemInfo));
                        }
                    }
                }
                else
                {
                    // local server
                    readers.Add(Helper.GetIISInfoReader(Helper.AzureMigrationId, null));
                }

                doWorkEventArgs.Result = readers;
            };

            this._worker.RunWorkerCompleted += (object runWorkerCompletedSender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) =>
                {
                    // Hide busy animation.
                    this.busyPictureBox.Visible = this.busyMessageLabel.Visible = false;
                    if (runWorkerCompletedEventArgs.Error != null || Helper.IisVersion < 6)
                    {
                        Helper.ShowErrorMessageAndExit(runWorkerCompletedEventArgs.Error.Message);
                    }

                    if (runWorkerCompletedEventArgs.Result == null)
                    {
                        Helper.ShowErrorMessageAndExit("IIS Configuration could not be read. Please re-run the tool");
                    }

                    
                    this.siteTree.Nodes.Clear();
                    this.siteTree.CheckBoxes = true;

                    this.siteTree.Tag = new SelectedObjects()
                    {
                        SelectedSites = new List<Site>(),
                        SelectedDatabases = new List<Database>()
                    };
                    
                    var rootNode = this.siteTree.Nodes.Add("Root", "Migration Candidates", 0, 0);
                    //rootNode.Checked = true;
                    // Get result from the async task.
                    List<IISInfoReader> readers = (List<IISInfoReader>) runWorkerCompletedEventArgs.Result;

                    // Save server object for later use.
                    foreach (var reader in readers)
                    {
                        if (reader != null)
                        {
                            if (!this.IISServers.Servers.ContainsKey(reader.Server.Name))
                            {
                                // TODO: what if we don't know the name?
                                this.IISServers.Servers.Add(reader.Server.Name, reader.Server);
                            }

                            var serverNode = rootNode.Nodes.Add("Server", 
                                                                string.Format("Web Server: {0}", 
                                                                reader.Server.Name), 
                                                                7, 
                                                                7);
                            serverNode.Tag = reader.Server;
                            //serverNode.Checked = true;

                            // Display candidate web sites
                            foreach (Site site in reader.Server.Sites)
                            {
                                site.ServerName = reader.Server.Name;
                                // We rely on Site::ToString() to display the name of the site.

                                //TreeNode siteNode = serverNode.Nodes.Add(site.ServerName + site.SiteName, site.SiteName, 1, 1);
                                TreeNode siteNode = new TreeNode(site.SiteName, 1, 1)
                                {
                                    Tag = site,
                                    //Checked = true,
                                    Name = site.ServerName + site.SiteName 
                                };

                                foreach (Database db in site.Databases) {
                                    siteNode.Nodes.Add(new TreeNode(db.ToString(), 5, 5) { 
                                        Tag = db//, Checked = true
                                    });
                                }
                                serverNode.Nodes.Add(siteNode);
                                this.Sites.Add(site.ServerName + site.SiteName, site);
                                //siteNode.Checked = true;
                            }
                        }
                    }
                    rootNode.Checked = true;

                    // Show listbox with results.
                    this.descriptionLabel.Visible = this.StartButton.Visible = this.btnBack.Visible = true; // this.websitesCheckedListBox.Visible = 
                    this.siteTree.ExpandAll();
                };

            this._worker.RunWorkerAsync();
        }

        private void MigrationCandidatesControl_Load(object sender, EventArgs e)
        {
            
        }

        private void licenseAgreementLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Helper.OpenWebLink(Helper.LicenseLink);
        }

        private void codeplexRepoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Helper.OpenWebLink(Helper.CodePlexRepoLink);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.AddRemoteServers, null);
        }

        public List<Site> SelectedSites = new List<Site>();

        private void siteTree_AfterCheck(object sender, TreeViewEventArgs e) {
            TreeNode selectedNode = e.Node;
            bool nodeChecked = selectedNode.Checked;
            
            //
            // If this node has children, then sync their checked status.
            //
            if ((null != selectedNode.Nodes) && (selectedNode.Nodes.Count != 0)) {
                foreach (TreeNode child in selectedNode.Nodes) {
                    if (child.Checked != selectedNode.Checked) {
                        child.Checked = selectedNode.Checked;
                    }
                }
            }

            if ( (selectedNode == this.siteTree.TopNode) || ( null == this.siteTree.Tag) ){
                return;
            }

            SelectedObjects selectedObjs = (SelectedObjects)this.siteTree.Tag;
            Type objectType = selectedNode.Tag.GetType();

            //
            // If this is a child node, and it's been checked, make sure its
            // parent nodes are checked.
            //
            if (nodeChecked) {
                TreeNode nodeParent = selectedNode.Parent;
                while (null != nodeParent) {
                    if (!nodeParent.Checked) {
                        nodeParent.Checked = true;
                    }
                    nodeParent = nodeParent.Parent;
                }

                if (typeof(Site) == objectType) {
                    if (!selectedObjs.SelectedSites.Contains((Site)selectedNode.Tag)) {
                        selectedObjs.SelectedSites.Add((Site)selectedNode.Tag);
                    }
                    // It's actually not possible to check (enable) anything 
                    // in the tree without at least one site being checked.
                    this.StartButton.Enabled = true;
                }
                else if (typeof(Database) == objectType) {
                    if (!selectedObjs.SelectedDatabases.Contains((Database)selectedNode.Tag)) {
                        selectedObjs.SelectedDatabases.Add((Database)selectedNode.Tag);
                    }
                }
            }
            else {
                if (typeof(Site) == objectType) {
                    if (selectedObjs.SelectedSites.Contains((Site)selectedNode.Tag)) {
                        selectedObjs.SelectedSites.Remove((Site)selectedNode.Tag);
                    }
                }
                else if (typeof(Database) == objectType) {
                    if (selectedObjs.SelectedDatabases.Contains((Database)selectedNode.Tag)) {
                        selectedObjs.SelectedDatabases.Remove((Database)selectedNode.Tag);
                    }
                }
                this.StartButton.Enabled = (0 != selectedObjs.SelectedSites.Count());
            }
            return;
        }

        //private void siteTree_AfterCheck(object sender, TreeViewEventArgs e)
        //{
        //    if (e.Node.Nodes.Count > 0)
        //    {
        //        this.CheckAllChildNodes(e.Node, e.Node.Checked);
        //    }

        //    if (this.siteTree.Nodes.Count > 0)
        //    {
        //        this.StartButton.Enabled = this.AtLeastOneChecked(this.siteTree.Nodes[0]);
        //    }
        //    //var site = this.Sites.Values.First();
        //    //this.SelectedSites.Add(site);
        //}

        //private bool AtLeastOneChecked(TreeNode treeNode)
        //{
        //    bool oneChecked = false;
        //    foreach (TreeNode node in treeNode.Nodes)
        //    {
        //        if (node.Checked && node.Nodes.Count == 0)
        //        {
        //            return true;
        //        }

        //        oneChecked |= this.AtLeastOneChecked(node);
        //    }

        //    return oneChecked;
        //}

        //private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        //{
        //    foreach (TreeNode node in treeNode.Nodes)
        //    {
        //        node.Checked = nodeChecked;
        //        // If the current node has child nodes, call the
        //        // CheckAllChildNodes method recursively.
        //        this.CheckAllChildNodes(node, nodeChecked);
        //    }
        //}
    }
}
