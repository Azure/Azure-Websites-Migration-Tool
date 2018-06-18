// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using AzureAppServiceMigrationTool.Helpers;
using AzureAppServiceMigrationTool.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.SqlClient;

namespace AzureAppServiceMigrationTool.Controls
{
    public partial class MigrationCandidatesControl : UserControl, IWizardStep
    {
        private BackgroundWorker _worker;
        internal class SelectedObjects
        {
            public List<Site> SelectedSites { get; set; }
            public List<Database> SelectedDatabases { get; set; }
            public List<IISServer> SelectedServers { get; set; }

        };

        public MigrationCandidatesControl()
        {
            InitializeComponent();
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
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

        private void GetSelectedSites()
        {
            SelectedObjects selectedObjs = (SelectedObjects)this.siteTree.Tag;

            if ((null == selectedObjs) ||
                (0 == selectedObjs.SelectedSites.Count())) {
                //
                // This shouldn't happen, since the Continue button is only
                // enabled if there is at least one site.
                //
                return;
            }

            Dictionary<IISServer, List<ApplicationPool>> allAppPools = new Dictionary<IISServer, List<ApplicationPool>>();

            // Clear the list of servers
            this.IISServers.Servers.Clear();

            foreach (IISServer selectedServer in selectedObjs.SelectedServers) {
                // first clear any sites the server has.
                selectedServer.Sites.Clear();

                // save the list of AppPools and reset the server's list.
                allAppPools.Add(selectedServer, selectedServer.AppPools);
                selectedServer.AppPools = new List<ApplicationPool>();

                // add this server back into the global list.
                this.IISServers.Servers.Add(selectedServer.Name, selectedServer);
            }

            foreach (Site selectedSite in selectedObjs.SelectedSites) {
                // first clear any databases the site has
                selectedSite.Databases.Clear();
                IISServer server = selectedSite.ParentServer;
                selectedSite.ParentServer = null; //prevent recursion during serialization

                // add this site back into its server's site collection.
                server.Sites.Add(selectedSite);

                // add app pools for this site back into the server's AppPool collection.
                List<ApplicationPool> appPools = allAppPools[server];
                if ((null==appPools) || (0 == appPools.Count)) { continue; }

                ApplicationPool ap = null;
                if (!server.AppPools.Exists(x => x.Name == selectedSite.AppPoolName)) {
                    ap = appPools.Find(x => x.Name == selectedSite.AppPoolName);
                    if (null != ap) {
                        server.AppPools.Add(ap);
                        appPools.Remove(ap); // trim the list 
                    }
                }

                foreach (Helpers.Application app in selectedSite.Applications) {
                    if (!server.AppPools.Exists(x => x.Name == app.AppPoolName)) {
                        ap = appPools.Find(x => x.Name == app.AppPoolName);
                        if (null != ap) {
                            server.AppPools.Add(ap);
                            appPools.Remove(ap); // trim the list 
                            if (0 == appPools.Count) { break; }
                        }
                    }
                }       
            }

            foreach (Database selectedDatabase in selectedObjs.SelectedDatabases) {
                // add any databases back to their respective sites.
                Site site = selectedDatabase.ParentSite;
                selectedDatabase.ParentSite = null; //prevent recursion during serialization
                site.Add(selectedDatabase);
                
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
                        SelectedDatabases = new List<Database>(),
                        SelectedServers = new List<IISServer>()
                    };
                    
                    var rootNode = this.siteTree.Nodes.Add("Root", "Migration Candidates", 0, 0);

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

                            // Display candidate web sites
                            foreach (Site site in reader.Server.Sites)
                            {
                                site.ServerName = reader.Server.Name;
                                site.ParentServer = reader.Server;
                                // We rely on Site::ToString() to display the name of the site.
                                TreeNode siteNode = new TreeNode(site.SiteName, 1, 1)
                                {
                                    Tag = site,
                                    Name = site.ServerName + site.SiteName 
                                };

                                foreach (Database db in site.Databases) {
                                    siteNode.Nodes.Add(new TreeNode(db.ToString(), 5, 5) { 
                                        Tag = db
                                    });
                                    
                                }
                                
                                siteNode.Nodes.Add(new TreeNode("Custom connection string", 5, 5)
                                {
                                    Tag = "AddDB",Checked=false
                                });
                                serverNode.Nodes.Add(siteNode);
                            }
                        }
                    }

                    //
                    // Set the root checked after all of the nodes have been
                    // added, and then fire the checked event recursively
                    // for all of the elements in the tree just once.
                    //
                    rootNode.Checked = true;
                    this.SyncCheckedStatus(rootNode, true);

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
            FireGoToEvent(WizardSteps.RemoteComputerInfo, null);
        }

        private void siteTree_AfterCheck(object sender, TreeViewEventArgs e) {
            // The code only executes if the user caused the checked state to change. 
            if (e.Action == TreeViewAction.Unknown)
            {
                return;
            }

            TreeNode selectedNode = e.Node;
            bool nodeChecked = selectedNode.Checked;
            
            //
            // If this node has children, then sync their checked status.
            //
            this.SyncCheckedStatus(selectedNode, nodeChecked);
            //
            // If this is a child node, and it's been checked, make sure its
            // parent nodes are checked.
            //
            TreeNode nodeParent = selectedNode.Parent;
            while (null != nodeParent)
            {
                if (!nodeParent.Checked)
                {
                    nodeParent.Checked = true;
                }
                nodeParent = nodeParent.Parent;
            }
        }

        private void SyncCheckedStatus(TreeNode selectedNode, bool nodeChecked)
        {
            this.SyncSelectedObjects(selectedNode, nodeChecked);
            if (selectedNode.Nodes.Count != 0)
            {
                foreach (TreeNode child in selectedNode.Nodes)
                {
                    if (child.Tag.ToString() != "AddDB")
                    {
                        child.Checked = nodeChecked;
                        this.SyncCheckedStatus(child, nodeChecked);
                    }
                }
            }
        }

        private void SyncSelectedObjects(TreeNode selectedNode, bool nodeChecked)
        {
            if ((selectedNode == this.siteTree.TopNode) ||
                 (null == this.siteTree.Tag))
            {
                return;
            }

            SelectedObjects selectedObjs = (SelectedObjects)this.siteTree.Tag;
            Type objectType = selectedNode.Tag.GetType();

            //
            // Add or remove selected items from the selected objects list.
            //
            if (nodeChecked) {
                if(typeof(string) == objectType && selectedNode.Tag == "AddDB")
                {
                    AddDbConnectionDialog dbDialog = new AddDbConnectionDialog();
                    if(dbDialog.ShowDialog() == DialogResult.OK)
                    {
                        string dbConnectionString = dbDialog.textBoxDbConnectionString.Text.Trim();
                        try
                        {
                            var dbConn = new DbConnectionStringBuilder { ConnectionString = dbConnectionString };
                            // dbConn.ConnectionString = dbConnectionString;
                            if (dbConn.ContainsKey("Provider") && (dbConn["Provider"].ToString() == "SQLOLEDB" || dbConn["Provider"].ToString().Contains("SQLNCLI")))
                            {
                                dbConn.Remove("Provider");
                            }

                            var sqlConn = new SqlConnectionStringBuilder(dbConn.ConnectionString);


                            //sqlConn.ConnectionString = dbConnectionString;
                            Site site = (Site)selectedNode.Parent.Tag;
                            Database db = new Database("", sqlConn.InitialCatalog, sqlConn.ConnectionString) { ParentSite = site };
                            site.Add(db);
                            selectedNode.Tag = db;
                            objectType = typeof(Database);
                        }
                        catch (System.ArgumentException ex)
                        {
                            string message = "Invalid connection string.\r\n\r\nValid connection string should be like\r\n 'Data Source=<servername>; Initial Catalog=<intialCatalog>; Trusted_Connection=<Yes|No>'";
                            MessageBox.Show(message);
                            selectedNode.Tag = "AddDB";
                            selectedNode.Checked = false;
                            TraceHelper.Tracer.WriteTrace(message);
                            return;
                        }
                    }
                    else
                    {
                        selectedNode.Tag = "AddDB";
                        selectedNode.Checked = false;
                        return;
                    }
                   
                }
                //
                // If this is a child node, and it's been checked, make sure its
                // parent nodes are checked.
                //
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
                else if (typeof(Database) == objectType)
                {
                    if (!selectedObjs.SelectedDatabases.Contains((Database)selectedNode.Tag))
                    {
                        selectedObjs.SelectedDatabases.Add((Database)selectedNode.Tag);
                    }
                }
                else if (typeof(IISServer) == objectType)
                {
                    if (!selectedObjs.SelectedServers.Contains((IISServer)selectedNode.Tag))
                    {
                        selectedObjs.SelectedServers.Add((IISServer)selectedNode.Tag);
                    }
                }
            }
            else
            {
                if (typeof(Site) == objectType)
                {
                    if (selectedObjs.SelectedSites.Contains((Site)selectedNode.Tag))
                    {
                        selectedObjs.SelectedSites.Remove((Site)selectedNode.Tag);
                    }
                }
                else if (typeof(Database) == objectType)
                {
                    if (selectedObjs.SelectedDatabases.Contains((Database)selectedNode.Tag))
                    {
                        selectedObjs.SelectedDatabases.Remove((Database)selectedNode.Tag);
                    }
                }
                else if (typeof(IISServer) == objectType)
                {
                    if (!selectedObjs.SelectedServers.Contains((IISServer)selectedNode.Tag))
                    {
                        selectedObjs.SelectedServers.Remove((IISServer)selectedNode.Tag);
                    }
                }
                this.StartButton.Enabled = (0 != selectedObjs.SelectedSites.Count());
            }
        }
    }
}
