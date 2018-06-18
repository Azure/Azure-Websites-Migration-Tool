// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;
using System.IO;

namespace AzureAppServiceMigrationTool.Helpers
{
    public static class TraceHelper
    {
        public static Tracer Tracer { get; set; }
    }

    public class Tracer
    {
        public string TraceFile { get; set; }

        public Tracer()
        {
            var path = Path.GetTempPath();
            string traceFilename = "MigrationTrace-" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
            TraceFile = Path.Combine(path, traceFilename);
        }

        public void WriteTrace(string format, params object[] args)
        {
            try
            {
                using (StreamWriter streamWriter = File.AppendText(this.TraceFile))
                {
                    var message = format;
                    try
                    {
                        message = string.Format(format, args);
                    }
                    catch
                    {
                    }

                    streamWriter.WriteLine(DateTime.Now + " : " + message);
                }
            }
            catch
            {
                // eat. program shouldn't crash on trace write file load error....
            }
        }
    }
}
