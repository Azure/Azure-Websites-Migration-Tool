// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace CompatCheckAndMigrate.Helpers
{
    [Serializable]
    public class VirtualDirectory
    {
        private string _name;
        private string _path;


        public VirtualDirectory(string name, string path)
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

  
    }
}