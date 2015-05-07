// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

namespace CompatCheckAndMigrate.Helpers
{
    public static class MigrationConstants
    {
        public const string MigrationId = "AzureMigrationID";
        public const string ServerNode = "iisServer";
        public const string SiteElement = "site";
        public const string AppElement = "application";
        public const string DBElement = "database";
        public const string BindingElement = "binding";
        public const string AppPoolElement = "appPool";
        public const string Name = "name";
        public const string ProviderName = "providerName";
        public const string ConnectionString = "connectionString";
        public const string BindingInfo = "bindingInfo";
        public const string Protocol = "protocol";
        public const string CertificateStoreName = "certificateStoreName";
        public const string CertificateHash = "certificateHash";
        public const string ClassicPipeline = "classicPipelineMode";
        public const string Enable32BitOn64 = "enable32BitOn64";
        public const string NetFxVersion = "netFxVersion";
        public const string UnsupportedAuthenticationTypes = "UnsupportedAuthenticationTypes";
        public const string OpeningBrace = "{";
        public const string ClosingBrace = "}";
        public const string OpeningBracket = "[";
        public const string ClosingBracket = "]";
        public const string Comma = ",";
        public const string NetFolderPath = @"HKEY_LOCAL_MACHINE\Software\Microsoft\.NetFramework";
        public const string NetInstallRootKey = "InstallRoot";
        public const string MigrationSyncManifest = "azmaMigration";
        public const string ManifestPath = "path";
        public const string ProductToSkip = "VWD11_Only";
        public const string IISPath = @"SOFTWARE\Microsoft\InetStp";
        public const string IERootPath = @"Software\microsoft\internet explorer";
        public const string IEEmulationPath = IERootPath + @"\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
        public const string MigrationIDPath = @"SOFTWARE\Microsoft\MigrateToAzure";
        public const int DefaultMaxPerProcessorThreadCount = 6;
        public const string LocalhostName = "localhost";


        public static string[] MigrationDependencyProducts = {"wdeploy", "dacfx"};
        public static string[] MigrationDependencyProductsPost2003 = { "WASProcessModel", "WASConfigurationAPI", "wdeploy", "dacfx" };

        public const string WebDeployMSI64 =
            "http://download.microsoft.com/download/A/5/0/A502BE57-7848-42B8-97D5-DEB2069E2B05/WebDeploy_amd64_en-US.msi";

        public const string WebDeployMSI32 =
            "http://download.microsoft.com/download/A/5/0/A502BE57-7848-42B8-97D5-DEB2069E2B05/WebDeploy_x86_en-US.msi";

        public const string WebPIUrl64 = @"http://download.microsoft.com/download/C/F/F/CFF3A0B8-99D4-41A2-AE1A-496C08BEB904/WebPlatformInstaller_amd64_en-US.msi";

        public const string DacfxUrlFormat = "http://go.microsoft.com/fwlink/?LinkID={0}&CLCID=0x409";
        
        public static int[] DacFxMsiArray64bit = { 239643, 239644, 239635, 239631, };
        public static int[] DacFxMsiArray32bit = { 239643, 239644, 239634, 239630, };
        
    }
}