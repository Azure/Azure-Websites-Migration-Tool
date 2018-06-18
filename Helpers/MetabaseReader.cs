// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace AzureAppServiceMigrationTool.Helpers
{
    public class MetabaseReader : IISInfoReader
    {
        private const string AnonymousAuth = "AuthAnonymous";
        private const string BasicAuth = "AuthBasic";
        private const string MD5Auth = "AuthMD5";
        private const string NTLMAuth = "AuthNTLM";
        private const string PassportAuth = "AuthPassport";
        private const string NameSpaceAgnosticXPathFormat = @"//*[local-name()='{0}']";
        private static Regex SiteLocationRegex = new Regex(@"/LM/W3SVC/(\d{1,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex ServerScriptMapRegex = new Regex(@"\.aspx,([^\s]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string ExactAttributeXPath =
            @"//*[local-name()='IIsWebVirtualDir']['{1}' = @*[local-name()='{0}']]";
        private const string HasAttributeXPath =
            @"//*[local-name()='IIsWebVirtualDir'][@*[local-name()='{0}']]";
        private const string ContainsAttributeXPath =
            @"//*[local-name()='IIsWebVirtualDir'][contains(@*[local-name()='{0}'],'{1}')]";

        private bool _isLocal = true;

        private static void AddBinding(string bindings, string protocol, ref Site site)
        {
            if (!string.IsNullOrEmpty(bindings))
            {
                var bindingArray = bindings.Split(new char[] {'\n', '\r', '\t'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var binding in bindingArray)
                {
                    site.Add(new Binding(binding.Trim(), protocol));
                }
            }
        }

        private static double GetNetFxVersion(XmlNode node, double? defaultVersion)
        {
            var attribute = node.Attributes["ScriptMaps"];
            if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
            {
                var m = ServerScriptMapRegex.Match(attribute.Value);
                if (m.Success && !string.IsNullOrEmpty(m.Groups[1].Value))
                {
                    var netFxFilePath = m.Groups[1].Value;
                    if (netFxFilePath.IndexOf("v2.0", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        return 2.0;
                    }

                    if (netFxFilePath.IndexOf("v4.0", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        return 4.0;
                    }
                }
            }

            return defaultVersion.GetValueOrDefault(2.0);
        }

        private static List<string> GetDefaultDocList(XmlNode node, List<string> serverDocList )
        {
            var attribute = node.Attributes["DefaultDoc"];
            if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
            {
                var docList = attribute.Value;
                var docArray = docList.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                if (docArray.Any())
                {
                    return docArray.ToList();
                }
            }

            return serverDocList;
        }

        private static EnabledAuthenticationType GetAuthenticationType(XmlNode node, EnabledAuthenticationType defaultAuthenticationType)
        {
            var attribute = node.Attributes["AuthFlags"];
            if (attribute == null || string.IsNullOrEmpty(attribute.Value))
            {
                return defaultAuthenticationType;
            }

            var authFlagValue = attribute.Value;
            var authArray = authFlagValue.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (authArray.Length == 1 &&
                authFlagValue.IndexOf(AnonymousAuth, StringComparison.OrdinalIgnoreCase) > -1)
            {
                return EnabledAuthenticationType.Anonymous;
            }

            return EnabledAuthenticationType.Windows;
        }

        public MetabaseReader(string migrationId, double iisVersion)
            :this(migrationId, iisVersion, null)
        {
        }

        public MetabaseReader(string migrationId, double iisVersion, RemoteSystemInfo remoteSystemInfo)
            : base(migrationId, iisVersion, remoteSystemInfo != null ? remoteSystemInfo.OSVersion : Environment.OSVersion.Version.ToString(), remoteSystemInfo != null ? remoteSystemInfo.ComputerName : Environment.MachineName)
        {
            _isLocal = remoteSystemInfo == null;
            var document = new XmlDocument();
//#if DEBUG
//            document.Load(@"E:\vsprojects\metabase.xml");
//#else
            string metabasePath = _isLocal
                ? Path.Combine(Environment.SystemDirectory, @"inetsrv\metabase.xml")
                : remoteSystemInfo.RemoteMetabasePath;
            document.Load(metabasePath);
//#endif

            var enable32Biton64 = false;
            var defaultAppPoolNode = document.SelectSingleNode(string.Format(NameSpaceAgnosticXPathFormat, "IIsApplicationPools"));
            var enable32bitAttribute = defaultAppPoolNode.Attributes["Enable32BitAppOnWin64"];
            if (enable32bitAttribute != null && !string.IsNullOrEmpty(enable32bitAttribute.Value))
            {
                enable32Biton64 = Convert.ToBoolean(enable32bitAttribute.Value);
            }

            XmlNode defaultServiceNode =
                document.SelectSingleNode(string.Format(NameSpaceAgnosticXPathFormat, "IIsWebService"));
            var netFxVersion =
                GetNetFxVersion(defaultServiceNode, null);
            var defaultAuthenticationType =
                GetAuthenticationType(
                    defaultServiceNode, EnabledAuthenticationType.Anonymous);
            var defaultDocList = GetDefaultDocList(defaultServiceNode, null);
            var iis5IsolationMode = false;
            var isolationModeAttribute = defaultServiceNode.Attributes["IIs5IsolationModeEnabled"];
            if (isolationModeAttribute != null && !string.IsNullOrEmpty(isolationModeAttribute.Value))
            {
                iis5IsolationMode = Convert.ToBoolean(isolationModeAttribute.Value);
            }
            
            var appPoolMap = new Dictionary<string, ApplicationPool>(StringComparer.OrdinalIgnoreCase);
            var appPoolNodes = document.SelectNodes(string.Format(NameSpaceAgnosticXPathFormat, "IIsApplicationPool"));
            foreach (XmlNode appPoolNode in appPoolNodes)
            {
                string name = string.Empty;
                var attribute = appPoolNode.Attributes["Location"];
                if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
                {
                    name = attribute.Value.Replace("/LM/W3SVC/AppPools/", string.Empty);
                    appPoolMap[name] = new ApplicationPool(true, netFxVersion, enable32Biton64, name);
                }
            }

            var siteNodes = document.SelectNodes(string.Format(NameSpaceAgnosticXPathFormat, "IIsWebServer"));
            foreach (XmlNode siteNode in siteNodes)
            {
                var attribute = siteNode.Attributes["Location"];
                if (attribute != null)
                {
                    var m = SiteLocationRegex.Match(attribute.Value);
                    if (m.Success)
                    {
                        long siteId = Convert.ToInt64(m.Groups[1].Value);
                        string siteName = siteNode.Attributes["ServerComment"].Value;
                        var site = new Site(siteName, siteId);
                        string siteRootPath = attribute.Value + "/ROOT";
                        string lowerCaseSiteRootPath = attribute.Value + "/root";
                        var localAttribute = siteNode.Attributes["ServerBindings"];
                        if (localAttribute != null && !string.IsNullOrEmpty(localAttribute.Value))
                        {
                            AddBinding(localAttribute.Value, "http", ref site);
                        }

                        localAttribute = siteNode.Attributes["SecureBindings"];
                        if (localAttribute != null && !string.IsNullOrEmpty(localAttribute.Value))
                        {
                            AddBinding(localAttribute.Value, "https", ref site);
                        }

                        XmlNode currentSiteNode =
                            document.SelectSingleNode(string.Format(ExactAttributeXPath, "Location",
                                siteRootPath));
                        if (currentSiteNode == null)
                        {
                            currentSiteNode =
                            document.SelectSingleNode(string.Format(ExactAttributeXPath, "Location",
                                lowerCaseSiteRootPath));
                        }

                        if (currentSiteNode != null)
                        {
                            string physicalPath = currentSiteNode.Attributes["Path"].Value;
                            if (!_isLocal)
                            {
                                physicalPath = remoteSystemInfo.GetRemotePath(physicalPath);
                            }
                            var d = new GacAssemblyDetector(physicalPath);
                            site.Add(d.GetGacedAssemblies());
                            var tempAttribute = currentSiteNode.Attributes["AppPoolId"];
                            site.PhysicalPath = physicalPath;
                            if(tempAttribute == null || string.IsNullOrEmpty(tempAttribute.Value))
                            {
                                site.AppPoolName = "DefaultAppPool";
                            }
                            else
                            {
                                site.AppPoolName = tempAttribute.Value;
                            }

                            site.DefaultDocuments = GetDefaultDocList(currentSiteNode, defaultDocList);

                            site.EnabledAuthType = GetAuthenticationType(currentSiteNode, defaultAuthenticationType);
                            ApplicationPool appPool;
                            if (appPoolMap.TryGetValue(site.AppPoolName, out appPool))
                            {
                                appPool.NetFxVersion = GetNetFxVersion(currentSiteNode, netFxVersion);
                            }

                            GetDatabaseInfo(ref site, physicalPath, remoteSystemInfo != null ? remoteSystemInfo.ComputerName : string.Empty);
                        }

                        var appNodes = document.SelectNodes(string.Format(ContainsAttributeXPath, "Location", siteRootPath));
                        foreach (XmlNode appNode in appNodes)
                        {
                            var tempAttribute = appNode.Attributes["Path"];
                            if (tempAttribute == null)
                            {
                                // its just a directory
                                continue;
                            }

                            var physicalPath = tempAttribute.Value;
                            tempAttribute = appNode.Attributes["AppFriendlyName"];
                            if (tempAttribute == null)
                            {
                                // its just a vdir and not an app. No AppPool Settings
                                continue;
                            }

                            if (!_isLocal)
                            {
                                physicalPath = remoteSystemInfo.GetRemotePath(physicalPath);
                            }
                            var g = new GacAssemblyDetector(physicalPath);
                            site.Add(g.GetGacedAssemblies());
                            GetDatabaseInfo(ref site, physicalPath, remoteSystemInfo != null ? remoteSystemInfo.ComputerName : string.Empty);
                            
                            string appName = tempAttribute.Value;
                            string appAppPoolName = string.Empty;
                            if (string.IsNullOrEmpty(appName))
                            {
                                appName = Path.GetFileName(physicalPath);
                            }

                            tempAttribute = appNode.Attributes["AppPoolId"];
                            if (tempAttribute != null)
                            {
                                appAppPoolName = tempAttribute.Value;
                            }

                            if (string.IsNullOrEmpty(appAppPoolName))
                            {
                                appAppPoolName = "DefaultAppPool";
                            }

                            ApplicationPool appPool;
                            if (appPoolMap.TryGetValue(appAppPoolName, out appPool))
                            {
                                appPool.NetFxVersion = GetNetFxVersion(appNode, netFxVersion);
                            }

                            var app = new Application(appName, physicalPath)
                            {
                                AppPoolName = appAppPoolName,
                            };

                            site.Add(app);
                        }

                        site.IIS5CompatMode = iis5IsolationMode;
                        Server.Sites.Add(site);
                    }
                }
            }

            foreach (var keyValue in appPoolMap)
            {
                Server.AppPools.Add(keyValue.Value);
            }
        }
    }
}