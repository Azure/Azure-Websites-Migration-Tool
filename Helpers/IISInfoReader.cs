// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Configuration;
using System.Data.Common;
using System.Data.EntityClient;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.Sql;

namespace CompatCheckAndMigrate.Helpers
{
    public abstract class IISInfoReader
    {
        private readonly IISServer _server;
        private static Regex ReplaceLocalHostRegex = new Regex(@"^localhost[\\]*", RegexOptions.IgnoreCase);
        private static Regex ReplaceLocalDotNotationRegex = new Regex(@"^\.[\\]*", RegexOptions.IgnoreCase);

        protected IISInfoReader(string migrationID, double iisVersion, string osVersion, string name)
        {
            _server = new IISServer(migrationID, iisVersion, osVersion, name);
        }

        public IISServer Server
        {
            get { return _server; }
        }

        private static string SetAppropriateServerName(string datasource, string remoteComputerName)
        {
            if (string.IsNullOrEmpty(datasource) || string.IsNullOrEmpty(remoteComputerName))
            {
                return datasource;
            }

            if (datasource == "." || string.Equals(datasource, MigrationConstants.LocalhostName, StringComparison.OrdinalIgnoreCase))
            {
                return remoteComputerName;
            }

            if (datasource.StartsWith(".\\"))
            {
                return remoteComputerName + datasource.Substring(1);
            }

            if (datasource.StartsWith(MigrationConstants.LocalhostName + "\\", StringComparison.OrdinalIgnoreCase))
            {
                return remoteComputerName + datasource.Substring(MigrationConstants.LocalhostName.Length);
            }

            return datasource;
        }

        protected static void GetDatabaseInfo(ref Site site, string physicalPath, string remoteComputerName)
        {
            try
            {
                string virtualSite = "/website";
                var map = new WebConfigurationFileMap();
                map.VirtualDirectories.Add(virtualSite, new VirtualDirectoryMapping(Environment.ExpandEnvironmentVariables(physicalPath), true));

                var configuration = WebConfigurationManager.OpenMappedWebConfiguration(map, virtualSite);
                if (configuration != null)
                {
                    ConfigurationSection configSection = configuration.GetSection("connectionStrings");
                    if (configSection != null)
                    {
                        foreach (ConnectionStringSettings connectionStringSetting in configuration.ConnectionStrings.ConnectionStrings)
                        {
                            string name = connectionStringSetting.Name;
                            string providerName = connectionStringSetting.ProviderName;
                            string dbConnectionString = connectionStringSetting.ConnectionString;
                            if (string.IsNullOrEmpty(dbConnectionString))
                            {
                                continue;
                            }
                            
                            if (dbConnectionString.Contains("metadata"))
                            {
                                // TODO: check other EF scenarios
                                // this is an entity framework connection string, so we can't migrate it.
                                // We use this to validate later on however
                                // we assume the site has a normal connection string as well
                                site.Add(new Database(providerName, name, dbConnectionString) { ParentSite = site });
                                continue;
                            }

                            var builder = new SqlConnectionStringBuilder(dbConnectionString);
                            if (!string.IsNullOrEmpty(builder.AttachDBFilename) && name == "LocalSqlServer")
                            {
                                // we ignore this since it is MOST LIKELY the default values from the machine.config connection string from .NET framework
                                continue;
                            }

                            try
                            {
                                var dbConn = new DbConnectionStringBuilder { ConnectionString = dbConnectionString };
                                // dbConn.ConnectionString = dbConnectionString;
                                if (dbConn.ContainsKey("Provider") && (dbConn["Provider"].ToString() == "SQLOLEDB" || dbConn["Provider"].ToString().Contains("SQLNCLI")))
                                {
                                    dbConn.Remove("Provider");
                                }

                                var sqlConn = new SqlConnectionStringBuilder(dbConn.ConnectionString);


                                //sqlConn.ConnectionString = dbConnectionString;

                                if (!string.IsNullOrEmpty(sqlConn.AttachDBFilename) && name == "LocalSqlServer")
                                {
                                    // we ignore this since it is MOST LIKELY the default values from the machine.config connection string from .NET framework
                                    continue;
                                }

                                if (!string.IsNullOrEmpty(remoteComputerName))
                                {
                                    sqlConn.DataSource = SetAppropriateServerName(sqlConn.DataSource, remoteComputerName);
                                }

                                site.Add(new Database(providerName, name, sqlConn.ConnectionString) { ParentSite = site });
                            }
                            catch (System.ArgumentException e)
                            {
                                MessageBox.Show(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                site.Errors.Add(ex.Message);
                // MessageBox.Show(ex.ToString());
            }
        }
    }
}