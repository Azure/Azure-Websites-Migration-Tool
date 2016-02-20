// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;

namespace CompatCheckAndMigrate.Helpers
{
    public class GacAssemblyDetector
    {
        public string SitePath { get; set; }
        public HashSet<string> GacedAssemblyList { get; private set; }
        public string NetFxFolderPath { get; private set; }

        public static HashSet<string> KnownTokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        static GacAssemblyDetector()
        {
            KnownTokens.Add("b03f5f7f11d50a3a");
            KnownTokens.Add("b77a5c561934e089");
            KnownTokens.Add("31bf3856ad364e35");
        }

        public GacAssemblyDetector(string sitePath)
        {
            SitePath = Environment.ExpandEnvironmentVariables(sitePath);
            GacedAssemblyList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string netFxPath = (string)Registry.GetValue(MigrationConstants.NetFolderPath, MigrationConstants.NetInstallRootKey, string.Empty);
            NetFxFolderPath = Helper.ChangePathSlash(netFxPath);
        }

        public List<string> GetGacedAssemblies()
        {
            if (string.IsNullOrEmpty(SitePath) ||
                !Directory.Exists(SitePath))
            {
                return new List<string>();
            }

            if (RemoteSystemInfos.Servers != null)
            {
                return new List<string>();
            }

#if DEBUG
            return new List<string>();
#endif

            var tempDomain = AppDomain.CreateDomain("ReflectionForGac");
            var fileList = new string[]{};
            try
            {
                fileList = Directory.GetFiles(SitePath, "*.dll", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                TraceHelper.Tracer.WriteTrace(ex.ToString());
            }

            if (fileList.Any())
            {
                foreach (var file in fileList)
                {
                    try
                    {
                        GetReferencedAssemblies(Assembly.LoadFrom(file));
                    }
                    catch
                    {
                        // failed to load some assembly. Its best effort basis
                    }
                }
            }

            //fileList = Directory.GetFiles(SitePath, "*.aspx", SearchOption.AllDirectories);
            //foreach (var file in fileList)
            //{
            //    Assembly assembly = null;
            //    try
            //    {
            //        //BuildManager.GetReferencedAssemblies()
            //        //   This throws because it needs virtual path and we have physical path.
            //        var p = BuildManager.CreateInstanceFromVirtualPath("testsite/default.aspx", typeof(Page));
            //    }
            //    catch (Exception ex)
            //    {
                     
            //    }
            //}
            
            AppDomain.Unload(tempDomain);
            return GacedAssemblyList.ToList();
        }

        private bool CheckIfAssemblyPathHasKnownToken(string assemblyPath)
        {
            foreach (var token in KnownTokens.ToList())
            {
                if (assemblyPath.IndexOf(token, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void GetReferencedAssemblies(Assembly assembly)
        {
            var assemblyNameList = assembly.GetReferencedAssemblies();
            foreach (var tempAssembly in assemblyNameList)
            {
                Assembly loadedTempAssembly = null;
                var token = tempAssembly.GetPublicKeyToken(); 
                var keyToken = BitConverter.ToString(token).Replace("-", string.Empty);
                if (KnownTokens.Contains(keyToken))
                {
                    continue;
                }

                try
                {
                    loadedTempAssembly = Assembly.Load(tempAssembly.FullName);
                }
                catch (Exception)
                {
                    try
                    {
                        if (tempAssembly.CodeBase == null)
                        {
                            continue;
                        }

                        var tempUri = new UriBuilder(tempAssembly.CodeBase);
                        var tempPath = tempUri.Uri.AbsolutePath;
                        loadedTempAssembly = Assembly.LoadFrom(tempPath);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                
                var uri = new UriBuilder(loadedTempAssembly.CodeBase);
                var assemblyPath = uri.Uri.AbsolutePath;
                if (!assemblyPath.StartsWith(NetFxFolderPath, StringComparison.OrdinalIgnoreCase) &&
                    !GacedAssemblyList.Contains(assemblyPath) &&
                    !CheckIfAssemblyPathHasKnownToken(assemblyPath))
                {
                    if (!assemblyPath.StartsWith(SitePath, StringComparison.OrdinalIgnoreCase))
                    {
                        GacedAssemblyList.Add(assemblyPath);
                    }

                    GetReferencedAssemblies(loadedTempAssembly);
                }
            }
        }
    }
}
