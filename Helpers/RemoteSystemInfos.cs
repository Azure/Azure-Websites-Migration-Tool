// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AzureAppServiceMigrationTool.Helpers
{
    public class RemoteSystemInfos
    {
        private static Dictionary<string, RemoteSystemInfo> smServers = new Dictionary<string, RemoteSystemInfo>();
        public static Dictionary<string, RemoteSystemInfo> Servers
        {
            get { return smServers; }
            set { smServers = value; }
        }

        private static Dictionary<string, string> smEmptyServers = new Dictionary<string, string>();
        public static Dictionary<string, string> EmptyServers
        {
            get { return smEmptyServers; }
            set { smEmptyServers = value; }
        }

        public static void AddOrUpdate(string computerName, string username, string password, string driveLetter)
        {
            var remoteSystemInfo = new RemoteSystemInfo(computerName, username, password, driveLetter);
            if (smServers.ContainsKey(computerName))
            {
                smServers[computerName] = remoteSystemInfo;
            }
            else
            {
                smServers.Add(computerName, remoteSystemInfo);
            }
        }

        public static void AddOrUpdateEmptyServer(string key, string computerName)
        {
            if (smEmptyServers.ContainsKey(key))
            {
                smEmptyServers[key] = computerName;
            }
            else
            {
                smEmptyServers.Add(key, computerName);
            }
        }
    }
}