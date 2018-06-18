// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Web.Administration;

namespace AzureAppServiceMigrationTool.Helpers
{
    public class ConfigReader : IISInfoReader
    {
        private const string AuthenticationSection = "system.webServer/security/authentication/{0}";
        private const string AnonymousAuth = "anonymousAuthentication";
        private const string BasicAuth = "basicAuthentication";
        private const string ClientServerAuth = "clientCertificateMappingAuthentication";
        private const string DigestAuth = "digestAuthentication";
        private const string IISClientServerMappingAuth = "iisClientCertificateMappingAuthentication";
        private const string WindowsAuth = "windowsAuthentication";
        private const string EnabledPropertyName = "enabled";
        private const string RootedAttributeFormat = "configSchema/sectionSchema[@name='{0}']/attribute[@name='{1}']";
        private const string RootedElementFormat = "configSchema/sectionSchema[@name='{0}']/element[@name='{1}']";

        private const string RootedElementCollectionFormat =
            "configSchema/sectionSchema[@name='{0}']/collection[@addElement='{1}']";

        private const string RootedElementCollectionClearFormat =
            "configSchema/sectionSchema[@name='{0}']/collection[@clearElement='{1}']";

        private const string RootedElementCollectionRemoveFormat =
            "configSchema/sectionSchema[@name='{0}']/collection[@removeElement='{1}']";

        private const string AttributeFormat = "{0}/attribute[@name='{1}']";
        private const string ElementFormat = "{0}/element[@name='{1}']";
        private const string ElementCollectionFormat = "{0}/collection[@addElement='{1}']";
        private const string ElementCollectionClearFormat = "{0}/collection[@clearElement='{1}']";
        private const string ElementCollectionRemoveFormat = "{0}/collection[@removeElement='{1}']";
        private const string RootConfigErrorName = "RootConfig";
        string tempSections = "";

        private static readonly List<string> AuthTypes = new List<string>
        {
            AnonymousAuth,
            WindowsAuth,
            BasicAuth,
            ClientServerAuth,
            DigestAuth,
            IISClientServerMappingAuth
        };

        private readonly string _appHostConfigPath;
        private bool _isLocal;

        public ConfigReader(string migrationId, double iisVersion, RemoteSystemInfo remoteSystemInfo)
            : base(migrationId, iisVersion, remoteSystemInfo != null ? remoteSystemInfo.OSVersion : Environment.OSVersion.Version.ToString(), remoteSystemInfo != null ? remoteSystemInfo.ComputerName : Environment.MachineName)
        {
            _appHostConfigPath = remoteSystemInfo != null ? remoteSystemInfo.RemoteAppHostConfigPath : string.Empty;
            _isLocal = remoteSystemInfo == null;
            Initialize(remoteSystemInfo);
        }

        public ConfigReader(string migrationId, double iisVersion)
            : this(migrationId, iisVersion, null)
        {
        }

        private ServerManager GetServerManager()
        {
            if (string.IsNullOrEmpty(_appHostConfigPath))
            {
                return new ServerManager();
            }

            return new ServerManager(_appHostConfigPath);
        }

        private static void SetAuthPropertiesForSite(Configuration config, ref Site site)
        {
            foreach (string authType in AuthTypes)
            {
                var authSection = config.GetSection(string.Format(AuthenticationSection, authType), site.SiteName);
                if ((bool)authSection[EnabledPropertyName])
                {
                    switch (authType)
                    {
                        case AnonymousAuth:
                            site.EnabledAuthType = EnabledAuthenticationType.Anonymous;
                            return;
                        case WindowsAuth:
                            site.EnabledAuthType = EnabledAuthenticationType.Windows;
                            return;
                        case BasicAuth:
                            site.EnabledAuthType = EnabledAuthenticationType.Basic;
                            return;
                        case ClientServerAuth:
                            site.EnabledAuthType = EnabledAuthenticationType.ClientCertificate;
                            return;
                        case IISClientServerMappingAuth:
                            site.EnabledAuthType = EnabledAuthenticationType.IisClientCertificate;
                            return;
                        case DigestAuth:
                            site.EnabledAuthType = EnabledAuthenticationType.Digest;
                            return;
                    }
                }
            }
        }

        private static void CheckSessionState(string pathToConfig, IISServer server)
        {
            if (string.IsNullOrEmpty(pathToConfig))
            {
                return;
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(pathToConfig);
                var xmlNodeList =
                    doc.SelectNodes(
                        "//*[local-name()='sessionState']");
                foreach (XmlNode node in xmlNodeList)
                {
                    foreach (XmlAttribute attrib in node.Attributes)
                    {
                        if (attrib.Name.Equals("mode") && !attrib.Value.Equals("InProc") && !attrib.Value.Equals("Off"))
                        {
                            string message = "";
                            if (attrib.Value.Equals("StateServer"))
                            {
                                message = string.Format("Session StateServer is not supported on Azure Web Apps");

                            }
                            else if (attrib.Value.Equals("SQLServer"))
                            {
                                message = string.Format("SQL Server session state needs to be configured separately");

                            }
                            else
                            {
                                message = string.Format("Custom session provider may or may not work on Azure Web Apps");

                            }
                            server.SchemaCheckErrors.Add(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // dont throw. Its ok if we dont get default document list. Let migration continue
                MessageBox.Show(ex.ToString());
                TraceHelper.Tracer.WriteTrace(ex.ToString());
            }
        }
        private static List<string> GetDefaultDocuments(string pathToConfig)
        {
            var docList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(pathToConfig))
            {
                return new List<string>();
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(pathToConfig);
                var xmlNodeList =
                    doc.SelectNodes(
                        "//*[local-name()='defaultDocument']/*[local-name()='files']/*[local-name()='add']/@value");
                foreach (XmlNode node in xmlNodeList)
                {
                    if (!docList.ContainsKey(node.Value))
                    {
                        docList[node.Value] = node.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                // dont throw. Its ok if we dont get default document list. Let migration continue
                MessageBox.Show(ex.ToString());
                TraceHelper.Tracer.WriteTrace(ex.ToString());
            }

            return docList.Values.ToList();
        }

        private void Initialize(RemoteSystemInfo remoteSystemInfo)
        {
            CheckConfig(Server, remoteSystemInfo);
            var userApphostConfig = Path.Combine(Environment.SystemDirectory,
                            @"inetsrv\config\applicationhost.config");
            var rootDefaultDocList = GetDefaultDocuments(userApphostConfig);
            CheckSessionState(userApphostConfig, Server);

            using (ServerManager manager = GetServerManager())
            {
                var config = manager.GetApplicationHostConfiguration();
                foreach (var appPool in manager.ApplicationPools)
                {
                    var isClassicMode = appPool.ManagedPipelineMode == ManagedPipelineMode.Classic ? true : false;
                    double netFxRuntimeVersion = 0;
                    if (!Double.TryParse(appPool.ManagedRuntimeVersion.TrimStart(new char[] { 'v', 'V' }),
                        out netFxRuntimeVersion))
                    {
                        netFxRuntimeVersion = 2.0;
                    }

                    Server.AppPools.Add(
                        new ApplicationPool(
                        isClassicMode,
                        netFxRuntimeVersion,
                        appPool.Enable32BitAppOnWin64,
                        appPool.Name));
                }

                foreach (var smSite in manager.Sites)
                {
                    string physicalPath = smSite.Applications["/"].VirtualDirectories["/"].PhysicalPath;
                    if (remoteSystemInfo != null)
                    {
                        physicalPath = remoteSystemInfo.GetRemotePath(physicalPath);
                    }
                    else
                    {
                        physicalPath = Environment.ExpandEnvironmentVariables(physicalPath);
                    }
                    var site = new Site(smSite.Name, smSite.Id);
                    site.PhysicalPath = physicalPath;
                    SetAuthPropertiesForSite(config, ref site);

                    if (Directory.Exists(physicalPath))
                    {
                        var webconfig = Path.Combine(physicalPath, "web.config");
                        if (File.Exists(webconfig))
                        {
                            CheckSessionState(webconfig, Server);
                            var localDefaultDocument = GetDefaultDocuments(webconfig);
                            if (localDefaultDocument.Count > 0)
                            {
                                site.DefaultDocuments = localDefaultDocument.Union(rootDefaultDocList,
                                    StringComparer.OrdinalIgnoreCase).ToList();
                            }
                            else
                            {
                                site.DefaultDocuments = rootDefaultDocList;
                            }
                        }
                    }

                    foreach (var application in smSite.Applications)
                    {
                        if (application.Path == "/")
                        {
                            site.AppPoolName = application.ApplicationPoolName;
                            var gacAssembly = new GacAssemblyDetector(physicalPath);
                            site.Add(gacAssembly.GetGacedAssemblies());

                            foreach (var vdir in smSite.Applications["/"].VirtualDirectories)
                            {
                                if (vdir.Path != "/")
                                {
                                    string vdirPhysicalPath = vdir.PhysicalPath;

                                    if (remoteSystemInfo != null)
                                    {
                                        vdirPhysicalPath = remoteSystemInfo.GetRemotePath(vdirPhysicalPath);
                                    }

                                    site.Add(new VirtualDirectory(vdir.Path, vdirPhysicalPath));
                                }
                            }

                            continue;
                        }

                        if (application.VirtualDirectories.Count > 0)
                        {
                            string appPhysicalPath = application.VirtualDirectories[0].PhysicalPath;
                            if (remoteSystemInfo != null && appPhysicalPath != "/")
                            {
                                appPhysicalPath = remoteSystemInfo.GetRemotePath(appPhysicalPath);
                            }

                            List<VirtualDirectory> listVdirs = new List<VirtualDirectory>();
                            foreach (var vdir in application.VirtualDirectories)
                            {
                                if (vdir.Path != "/")
                                {
                                    string vdirPhysicalPath = vdir.PhysicalPath;

                                    if (remoteSystemInfo != null)
                                    {
                                        vdirPhysicalPath = remoteSystemInfo.GetRemotePath(vdirPhysicalPath);
                                    }

                                    listVdirs.Add(new VirtualDirectory(vdir.Path, vdirPhysicalPath));
                                }
                            }

                            site.Add(new Application(application.Path, appPhysicalPath)
                            {
                                AppPoolName = application.ApplicationPoolName, VDirs = listVdirs
                            });

                            if (appPhysicalPath == "/")
                            {
                                continue;
                            }

                            var gacAssemblyForApp =
                                new GacAssemblyDetector(appPhysicalPath);
                            site.Add(gacAssemblyForApp.GetGacedAssemblies());
                        }
                    }

                    foreach (var binding in smSite.Bindings)
                    {
                        site.Add(new Binding(binding.BindingInformation, binding.Protocol));
                    }

                    site.IisVersion = this.Server.IISVersion;
                    site.OsVersion = this.Server.OSVersion;
                    GetDatabaseInfo(ref site, physicalPath, remoteSystemInfo != null ? remoteSystemInfo.ComputerName : string.Empty);
                    Server.Sites.Add(site);
                }
            }
        }

        private void CheckConfig(IISServer server, RemoteSystemInfo remoteSystemInfo)
        {
            string configSectionName = "configSections";
            string ftpServerSection = "system.ftpServer";
            string NameAttribute = "name";
            string xpathFormat = "configuration/";

            var antaresSchema = new XmlDocument();
            antaresSchema.Load(Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream("AzureAppServiceMigrationTool.Resources.IIS_MergedSchema.xml"));
            var customerAppHost = new XmlDocument();

            var userApphostConfig = Path.Combine(Environment.SystemDirectory,
                            @"inetsrv\config\applicationhost.config");
            if (remoteSystemInfo != null)
            {
                userApphostConfig = remoteSystemInfo.RemoteAppHostConfigPath;
            }
            customerAppHost.Load(userApphostConfig);

            var antaresConfigSections = new HashSet<string>(StringComparer.Ordinal);

            XmlNodeList nodes = antaresSchema.SelectNodes("configSchema/sectionSchema");
            foreach (XmlNode node in nodes)
            {
                antaresConfigSections.Add(node.Attributes[NameAttribute].Value);
            }

            var customerConfigSections = new List<string>();
            nodes = customerAppHost.SelectNodes("//section");
            foreach (XmlNode node in nodes)
            {
                XmlNode tempNode = node;
                string sectionPath = tempNode.Attributes[NameAttribute].Value;
                while (!string.Equals(tempNode.ParentNode.Name, configSectionName, StringComparison.OrdinalIgnoreCase))
                {
                    tempNode = tempNode.ParentNode;
                    sectionPath = tempNode.Attributes[NameAttribute].Value + "/" + sectionPath;
                }

                if (!sectionPath.StartsWith(ftpServerSection, StringComparison.OrdinalIgnoreCase))
                {
                    customerConfigSections.Add(sectionPath);
                }
            }

            foreach (string customerSection in customerConfigSections)
            {
                if (!antaresConfigSections.Contains(customerSection))
                {
                    string message = string.Format("Section definition not supported for {0}", customerSection);
                    server.SchemaCheckErrors.Add(message);
                }

                var nodeList = customerAppHost.SelectNodes(string.Concat(xpathFormat, customerSection));
                foreach (XmlNode node in nodeList)
                {
                    CheckAttributes(customerSection, antaresSchema, node, server);
                    CheckElements(customerSection, antaresSchema, node, server);
                }
            }
        }

        private void CheckAttributes(string sectionPath, XmlDocument antaresDoc, XmlNode node, IISServer server)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                var formatStr = string.Empty;
                if (sectionPath.StartsWith("configSchema/"))
                {
                    formatStr = AttributeFormat;
                }
                else
                {
                    formatStr = RootedAttributeFormat;
                }

                string attributeSectionPath = string.Format(formatStr, sectionPath, attribute.Name);
                if (antaresDoc.SelectNodes(attributeSectionPath).Count < 1 && !attribute.Name.Equals("lockAttributes"))
                {
                    string message = string.Format("Attribute {0} not supported for {1}", attribute.Name, sectionPath);
                    server.SchemaCheckErrors.Add(message);
                }
            }
        }

        private void CheckElements(string sectionPath, XmlDocument antaresDoc, XmlNode node, IISServer server)
        {
            var map = new ElementCountMap();
            foreach (XmlNode childNode in node.ChildNodes)
            {
                map.Add(childNode.Name);
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {
                var formatStrCollection = string.Empty;
                var formatStr = string.Empty;
                bool isCollection = map[childNode.Name] > 1;
                if (sectionPath.StartsWith("configSchema/"))
                {
                    formatStr = ElementFormat;
                    if (childNode.Name.Equals("clear"))
                    {
                        formatStrCollection = ElementCollectionClearFormat;
                    }
                    else if (childNode.Name.Equals("remove"))
                    {
                        formatStrCollection = ElementCollectionRemoveFormat;
                    }
                    else
                    {
                        formatStrCollection = ElementCollectionFormat;
                    }
                }
                else
                {
                    if (childNode.Name.Equals("clear"))
                    {
                        formatStrCollection = RootedElementCollectionClearFormat;
                    }
                    else if (childNode.Name.Equals("remove"))
                    {
                        formatStrCollection = RootedElementCollectionRemoveFormat;
                    }
                    else
                    {
                        formatStrCollection = RootedElementCollectionFormat;
                    }
                    formatStr = RootedElementFormat;
                }

                var childSectionPath = string.Empty;
                var childSectionPathSingle = string.Format(formatStr, sectionPath, childNode.Name);
                var childSectionPathColl = string.Format(formatStrCollection, sectionPath, childNode.Name);
                var sectionFound = false;
                if (antaresDoc.SelectNodes(childSectionPathSingle).Count > 0)
                {
                    sectionFound = true;
                    childSectionPath = childSectionPathSingle;
                }
                else if (antaresDoc.SelectNodes(childSectionPathColl).Count > 0)
                {
                    sectionFound = true;
                    childSectionPath = childSectionPathColl;
                }

                if (sectionFound)
                {
                    CheckAttributes(childSectionPath, antaresDoc, childNode, server);
                    CheckElements(childSectionPath, antaresDoc, childNode, server);
                }
                else
                {
                    var message = string.Format("Missing element {0} for path {1}", childNode.Name, childSectionPath);
                    server.SchemaCheckErrors.Add(message);
                }
            }
        }
    }
}