// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;
using AzureAppServiceMigrationAssistant.Helpers;
using Application = System.Windows.Forms.Application;

namespace AzureAppServiceMigrationAssistant
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            if (!IsAdministrator())
            {
                var info = new ProcessStartInfo(Assembly.GetExecutingAssembly().GetName().Name);
                info.Verb = "runas";
                Process.Start(info);
                Application.Exit();
            }
            else
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                    Application.Run(new MainForm());
                }
                catch (Exception ex)
                {
                    TraceHelper.Tracer.WriteTrace(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(((Exception) e.ExceptionObject).ToString());
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                    .IsInRole(WindowsBuiltInRole.Administrator);
        } 
    }
}