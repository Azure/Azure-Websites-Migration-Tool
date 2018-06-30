// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;

namespace AzureAppServiceMigrationAssistant.Helpers
{
    [Serializable]
    public class ApplicationPool
    {
        private readonly bool _enable32On64;
        private readonly bool _isClassicPipeline;
        private readonly string _name;
        private double _netFxVersion;

        public ApplicationPool(string managedPipeLineMode, double netFxVersion, bool enable32On64, string appPoolName)
            :this(!string.Equals(managedPipeLineMode, "Integrated", StringComparison.OrdinalIgnoreCase), netFxVersion, enable32On64, appPoolName)
        {
        }

        public ApplicationPool(bool isClassicPipelineMode, double netFxVersion, bool enable32On64, string appPoolName)
        {
            _netFxVersion = netFxVersion;
            _name = appPoolName;
            _enable32On64 = enable32On64;
            _isClassicPipeline = isClassicPipelineMode;
        }

        public Double NetFxVersion
        {
            get { return _netFxVersion; }
            set { _netFxVersion = value; }
        }

        public bool IsClassicMode
        {
            get { return _isClassicPipeline; }
        }

        public bool Enable32BitOn64
        {
            get { return _enable32On64; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}