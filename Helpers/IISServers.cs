// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace CompatCheckAndMigrate.Helpers
{
    [Serializable]
    public class IISServers
    {
        public Dictionary<string, IISServer> Servers { get; set; }

        public IISServers()
        {
            this.Servers = new Dictionary<string, IISServer>();
        }

        public string Serialize()
        {
            var serializer = new JavaScriptSerializer();
            string retValue = serializer.Serialize(this);
            return retValue;
        }
    }
}