// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace AzureAppServiceMigrationTool.Helpers
{
    [Serializable]
    public class Application
    {
        private string _name;
        private string _appPoolName;
        private string _path;
        private List<VirtualDirectory> _vdirs;

        public Application(string name, string path)
        {
            _name = name;
            _path = path;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public List<VirtualDirectory> VDirs
        {
            get { return _vdirs; }
            set { _vdirs = value; }
        }

        public string AppPoolName
        {
            get { return _appPoolName; }
            set { _appPoolName = value; }
        }
    }
}