// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace CompatCheckAndMigrate.Helpers
{
    [Serializable]
    public class IISServer
    {
        private readonly string _azureMigrationID;
        private readonly double _iisVersion;
        private readonly string _osVersion;
        private List<Site> _sites;
        private List<ApplicationPool> _appPools;
        public List<string> SchemaCheckErrors { get; set; }
        public string Name { get; set; }
        // The servers are modified, hence we keep 2 copies, one for the original values.
        public bool Original { get; set; }

        public IISServer(string azureMigrationID, double iisVersion, string osVersion, string name)
        {
            this._azureMigrationID = azureMigrationID;
            this._iisVersion = iisVersion;
            this._osVersion = osVersion;
            this._sites = new List<Site>();
            this._appPools = new List<ApplicationPool>();
            this.SchemaCheckErrors = new List<string>();
            this.Name = name;
        }

        public string AzureMigrationID
        {
            get { return _azureMigrationID; }
        }

        public double IISVersion
        {
            get { return _iisVersion; }
        }

        public string OSVersion
        {
            get { return _osVersion; }
        }

        public void Add(Site site)
        {
            Sites.Add(site);
        }

        public void Add(ApplicationPool appPool)
        {
            AppPools.Add(appPool);
        }

        public string Serialize()
        {
            var serializer = new JavaScriptSerializer();
            string retValue = serializer.Serialize(this);
            return retValue;
        }

        public List<Site> Sites
        {
            get { return _sites; }
            internal set { _sites = value; }
        }

        public List<ApplicationPool> AppPools
        {
            get { return _appPools; }
            set { this._appPools = value; }
        }

        public void SetPublishSetting(Dictionary<string, string> siteErrorInfo, string publishSettingsFilePath, string serverName)
        {
            foreach (var site in this.Sites)
            {
                var publishSetting = new PublishSettings(publishSettingsFilePath, site.SiteName, serverName);
                if (!publishSetting.Initialized)
                {
                    publishSetting = new PublishSettings(publishSettingsFilePath, site.SiteName);
                }

                if (publishSetting.Initialized)
                {
                    site.PublishProfile = publishSetting;
                    string siteError;
                    if (siteErrorInfo != null && siteErrorInfo.TryGetValue(site.PublishProfile.SiteName, out siteError))
                    {
                        site.SiteCreationError = siteError;
                    }
                }
            }
        }
    }
}