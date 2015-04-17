//------------------------------------------------------------------------------
// <copyright file="PublishManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Web.Deployment;
using CompatCheckAndMigrate.Controls;
using System.Windows.Forms;

namespace CompatCheckAndMigrate.Helpers
{
    internal class PublishManager
    {
        private const int Interval = 30 * 1000; // 30 seconds
        private Queue<PublishOperation> _pendingPublishOperations;
        private Queue<PublishOperation> _completedOperations;
        private int _totalOperations;

        private Thread _controllerThread;

        internal PublishManager()
        {
            MainForm.WriteTrace("Initializing PublishManager");
            _pendingPublishOperations = new Queue<PublishOperation>();
            _completedOperations = new Queue<PublishOperation>();
            _totalOperations = 0;
            MainForm.WriteTrace("Max Threads is {0}", Helper.MaxConcurrentThreads);
            ThreadPool.SetMaxThreads(Helper.MaxConcurrentThreads, Helper.MaxConcurrentThreads);
        }

        internal void Enqueue(PublishOperation operation)
        {
            MainForm.WriteTrace("Enqueueing {0}", operation.LocalSite.SiteName);
            // no contention since enqueueing happens before processing has started
            _pendingPublishOperations.Enqueue(operation);
        }

        internal void StartProcessing()
        {
            _totalOperations = _pendingPublishOperations.Count;
            MainForm.WriteTrace("Total Operations at start: {0}", _totalOperations);
            _controllerThread = new Thread(Controller);
            _controllerThread.Start();
        }

        internal void WaitForOperations()
        {
            while (_completedOperations.Count < _totalOperations)
            {
                MainForm.WriteTrace("Waiting for operations to complete. Total Operations: {0}, Completed Operations: {1}", _totalOperations, _completedOperations.Count);
                Thread.Sleep(1000);
            }

            while (_completedOperations.Count > 0)
            {
                var operation = _completedOperations.Dequeue();
                MainForm.WriteTrace("Checking status for {0}", operation.LocalSite.SiteName);
                if (operation is PublishContentOperation)
                {
                    MainForm.WriteTrace("Setting content status {0} for {1}", operation.PublishStatus, operation.LocalSite.SiteName);
                    operation.LocalSite.ContentPublishState = operation.PublishStatus;
                }
                else
                {
                    MainForm.WriteTrace("Setting db status {0} for {1}", operation.PublishStatus, operation.LocalSite.SiteName);
                    operation.LocalSite.DbPublishState = operation.PublishStatus;
                }

                operation.Dispose();
            }
        }

        private void Controller()
        {
            while (_pendingPublishOperations.Count > 0)
            {
                PublishOperation operation = _pendingPublishOperations.Dequeue();

                if (operation is PublishContentOperation)
                {
                    MainForm.WriteTrace("Scheduling content for: {0}", operation.LocalSite.SiteName);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(PublishContent), operation);
                }
                else if (operation is PublishDbOperation)
                {
                    MainForm.WriteTrace("Scheduling db for: {0}", operation.LocalSite.SiteName);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(PublishDatabase), operation);
                }
            }
        }

        private void PublishContent(object operationData)
        {
            var operation = operationData as PublishContentOperation;
            if (operation == null)
            {
                MainForm.WriteTrace("Publish: Thread {0} did not find any operations to process although got scheduled", Thread.CurrentThread.ManagedThreadId);
                return;
            }

            try
            {
                MainForm.WriteTrace("Starting calculation for: {0} in Thread {1}", operation.LocalSite.SiteName, Thread.CurrentThread.ManagedThreadId);
                if (operation.Publish(true))
                {
                    // set size succeeded. The source and destination are valid. Start actual sync
                    MainForm.WriteTrace("Starting actual publish for: {0} in Thread {1}", operation.LocalSite.SiteName, Thread.CurrentThread.ManagedThreadId);
                    operation.Publish(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                lock (_completedOperations)
                {
                    MainForm.WriteTrace("Enqueueing completed operation for: {0} in Thread {1}", operation.LocalSite.SiteName, Thread.CurrentThread.ManagedThreadId);
                    _completedOperations.Enqueue(operation);
                }
            }
        }

        private void PublishDatabase(object operationData)
        {
            var operation = operationData as PublishDbOperation;
            if (operation == null)
            {
                MainForm.WriteTrace("Publish: Thread {0} did not find any operations to process although got scheduled", Thread.CurrentThread.ManagedThreadId);
                return;
            }

            try
            {
                MainForm.WriteTrace("Starting db publish for: {0} in Thread {1}", operation.LocalSite.SiteName, Thread.CurrentThread.ManagedThreadId);
                operation.Publish(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                lock (_completedOperations)
                {
                    MainForm.WriteTrace("Enqueueing completion for: {0} in Thread {1}", operation.LocalSite.SiteName, Thread.CurrentThread.ManagedThreadId);
                    _completedOperations.Enqueue(operation);
                }
            }
        }
    }
}
