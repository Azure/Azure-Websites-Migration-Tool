// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;
using System.Data.SqlClient;

namespace CompatCheckAndMigrate.Helpers
{
    [Serializable]
    public class Database
    {
        //TOD: What if there is no name provided

        private readonly string _connectionStringName;
        private readonly string _providerName;
        internal string DBConnectionString { get; set; }
        internal SqlConnectionStringBuilder DbConnectionStringBuilder { get; set; }

        public Database(string providerName, string name)
        {
            _providerName = providerName;
            _connectionStringName = name;
        }

        internal Database(string providerName, string name, string dbConnectionString)
        {
            DBConnectionString = dbConnectionString;
            _providerName = providerName;
            _connectionStringName = name;
            if (!dbConnectionString.ToLower().StartsWith("metadata="))
            {
                DbConnectionStringBuilder = new SqlConnectionStringBuilder(dbConnectionString);
            }
            else
            {
                DbConnectionStringBuilder = new SqlConnectionStringBuilder();
            }
        }

        public string ProviderName
        {
            get { return _providerName; }
        }

        public string ConnectionStringName
        {
            get { return _connectionStringName; }
        }
    }
}