// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace CompatCheckAndMigrate.Helpers
{
    public enum EnabledAuthenticationType
    {
        Anonymous = 0,
        Windows,
        Passport,
        Basic,
        Digest,
        ClientCertificate,
        IisClientCertificate,
        Forms,
    }

    [Serializable]
    public class Site
    {
        private string _appPoolName;
        private readonly List<Application> _applications;
        private readonly List<Binding> _bindings;
        private readonly List<Database> _databases;
        private readonly List<VirtualDirectory> _vdirs;
        private readonly string _name;
        private readonly long _siteId;
        private EnabledAuthenticationType _enabledAuthType;
        internal string SiteCreationError { get; set; }
        internal PublishSettings PublishProfile { get; set; }
        internal bool ContentPublishState { get; set; }
        internal bool DbPublishState { get; set; }
        private List<string> _defaultDocuments;
        private List<string> _gacedAssemblies;
        public bool IIS5CompatMode { get; set; }
        public List<string> Errors {get; internal set;}
        public double IisVersion { get; set; }
        public string OsVersion { get; set; }

        public Site(string name, long siteId)
        {
            _name = name;
            _applications = new List<Application>();
            _vdirs = new List<VirtualDirectory>();
            _bindings = new List<Binding>();
            _databases = new List<Database>();
            _siteId = siteId;
            _gacedAssemblies = new List<string>();
            _defaultDocuments = new List<string>();
            IIS5CompatMode = false;
            this.DbPublishState = true;
            this.ContentPublishState = true;
            Errors = new List<string>();
        }

        internal string PhysicalPath { get; set; }

        public string GetGacedAssemblies()
        {
            return GacedAssemblyUsage ? 
                string.Join(",", _gacedAssemblies.ToArray()) :
                "None";
        }

        public string SiteName
        {
            get { return _name; }
        }

        public bool GacedAssemblyUsage
        {
            get { return _gacedAssemblies.Count > 0; }
        }

        public long SiteId
        {
            get { return _siteId; }
        }

        public EnabledAuthenticationType EnabledAuthType
        {
            get { return _enabledAuthType; }
            set { _enabledAuthType = value; }
        }

        public List<string> DefaultDocuments
        {
            get { return _defaultDocuments; }
            set { _defaultDocuments = value; }
        }

        public List<Application> Applications
        {
            get { return _applications; }
        }
        public List<VirtualDirectory> VDirs
        {
            get { return _vdirs; }
        }

        public List<Database> Databases
        {
            get { return _databases; }
        }

        public List<Binding> Bindings
        {
            get { return _bindings; }
        }

        public string AppPoolName
        {
            get { return _appPoolName; }
            set { _appPoolName = value; }
        }

       
        public string ServerName { get; set; }

        public void Add(Application app)
        {
            _applications.Add(app);
        }
        public void Add(VirtualDirectory vdir)
        {
            _vdirs.Add(vdir);
        }

        public void Add(Binding binding)
        {
            _bindings.Add(binding);
        }

        public void Add(Database db)
        {
            _databases.Add(db);
        }

        public void Add(string gacAssembly)
        {
            _gacedAssemblies.Add(gacAssembly);
        }

        public void Add(List<string> gacAssemblyList)
        {
            _gacedAssemblies.AddRange(gacAssemblyList);
        }

        public override string ToString()
        {
            if (this.ServerName != null)
            {
                return string.Format("{0} : {1}", this.ServerName, this.SiteName);
            }

            return this.SiteName;
        }
    }
}