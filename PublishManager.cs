//------------------------------------------------------------------------------
// <copyright file="PublishManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Web.Deployment;
using Microsoft.Web.Hosting;
using Microsoft.Web.Hosting.Tracing;

namespace PublishFailOverService
{
    internal class PublishManager
    {
        private const int Interval = 30 * 1000; // 30 seconds
        private const int LogTruncateInterval = 60; // 60 minutes
        private const int DiagnosticLogIntervalInMinutes = 1; // 1 minute
        private AutoResetEvent _exitEvent;
        private AutoResetEvent _continueEvent;
        private WaitHandle[] _waitHandleArray;
        private static bool _operationInitialized = false;
        private Queue<Operation> _pendingPublishOperations;
        private Queue<Operation> _pendingDeleteOperations;
        private Queue<Operation> _completedOperations;
        private HashSet<string> _operationsInProgress;

        private Thread _controllerThread;
        private Thread _updaterThread;

        private const string WellKnownGuid = "5C608716-46B5-4AF4-A878-BFDBCCDF6A40";
        private const string ContentPathParameterName = "DRSyncContentPathParameter_" + WellKnownGuid;
        private const string SetAclParameterName = "DRSyncSetAclParameter_" + WellKnownGuid;
        private const string FileServerIPParameterName = "DRSyncFileServerIPParameter_" + WellKnownGuid;
        private const string QueryServerIPParameterName = "DRSyncQueryServerIPParameter_" + WellKnownGuid;
        private const string ContentPathParameterDefaultValue = "$1{" + FileServerIPParameterName + "}$3";
        private static Regex IPReplaceRegex = new Regex(@"(^\\\\)([0-9\.]*)(\\)", RegexOptions.IgnoreCase);

        // These can be hard coded for English because these are not public facing parameters. 
        // They are for internal use only
        private const string ContentPathParameterDescription = "DR Sync Content Path Parameter";
        private const string SetAclParameterDescription = "DR Sync SetAcl Parameter";
        private const string FileServerIPParameterDescription = "DR Sync File Server IP Parameter";
        private const string QueryServerIPParameterDescription = "DR Sync Query IP Parameter";

        internal PublishManager(string serviceName)
        {
            _exitEvent = new AutoResetEvent(false);
            _continueEvent = new AutoResetEvent(false);
            _waitHandleArray = new WaitHandle[] { _exitEvent, _continueEvent };

            _pendingPublishOperations = new Queue<Operation>();
            _completedOperations = new Queue<Operation>();
            _pendingDeleteOperations = new Queue<Operation>();

            _operationsInProgress = new HashSet<string>();
            PublishHelper.LogVerboseInformation("PublishManager initialized");
            ServicePointManager.ServerCertificateValidationCallback += CertificateHelper.CertificateValidationCallback;
        }

        internal void StartProcessing()
        {
            _controllerThread = new Thread(Controller);
            _controllerThread.Start();

            _updaterThread = new Thread(Updater);
            _updaterThread.Start();
        }

        internal void StopProcessing()
        {
            _exitEvent.Set();
            // Sleep for a second for threads to detect exit event and exit
            Thread.Sleep(1000);
        }

        private void Updater()
        {
            DateTime initialTruncationTime = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(LogTruncateInterval));
            DateTime initialDiagnosticLogTime = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(DiagnosticLogIntervalInMinutes));

            while (true)
            {
                int eventId = WaitHandle.WaitAny(_waitHandleArray, Interval);
                if (eventId == 0)
                {
                    //If exitevent is signalled, the process is exiting.
                    break;
                }

                #region Update Operations

                PublishHelper.LogVerboseInformation("UpdaterThread: Found {0} completed operations to update", _completedOperations.Count);
                if (_completedOperations.Count > 0)
                {
                    lock (_completedOperations)
                    {
                        while (_completedOperations.Count > 0)
                        {
                            var operation = _completedOperations.Dequeue();
                            PublishHelper.UpdatePublishOperation(operation.OperationId, operation.SiteName, operation.Status);
                        }
                    }
                }

                #endregion

                #region Remove Stale Operations

                if ((DateTime.UtcNow - initialTruncationTime).TotalMinutes > LogTruncateInterval)
                {
                    initialTruncationTime = DateTime.UtcNow;
                    PublishHelper.LogVerboseInformation("LogTruncaterThread: Removing Stale Operations");
                    PublishHelper.RemoveStalePublishOperations();
                }

                #endregion

                #region Log Diagnostic Data

                if ((DateTime.UtcNow - initialDiagnosticLogTime).TotalMinutes > DiagnosticLogIntervalInMinutes)
                {
                    initialDiagnosticLogTime = DateTime.UtcNow;
                    PublishHelper.LogOperationCount();
                }

                #endregion
            }
        }

        private void Controller()
        {
            while (true)
            {
                PublishHelper.LogVerboseInformation("ControllerThread: Waiting for operations");
                int eventId = WaitHandle.WaitAny(_waitHandleArray, Interval);
                if (eventId == 0)
                {
                    //If exitevent is signalled, the process is exiting.
                    break;
                }

                if (!PublishHelper.ShouldBeginProcessingPublishOperation)
                {
                    PublishHelper.LogVerboseInformation("ControllerThread: BeginProcessing is currently off");
                    _continueEvent.Reset();
                    continue;
                }

                int maxConcurrentOperations = PublishHelper.MaxConcurrentSyncOperations;

                ThreadPool.SetMaxThreads(maxConcurrentOperations, maxConcurrentOperations);

                if (!_operationInitialized)
                {
                    _operationInitialized = true;
                    PublishHelper.LogVerboseInformation("ControllerThread: Initializing publish operation");
                }

                #region Schedule Operations

                _continueEvent.Reset();
                PublishOperation publishOperation;
                try
                {
                    while (PublishHelper.GetNextPublishOperation(out publishOperation))
                    {
                        if (_exitEvent.WaitOne(0))
                        {
                            //If this is signalled, the process is exiting
                            return;
                        }

                        string physicalPath = string.Empty;
                        bool siteInvalid = false;
                        try
                        {
                            // if the site was added or updated and then deleted, the add and update
                            // operations get always scheduled before the delete. 
                            // so if the site is deleted, this will throw object not found exception.
                            // set the object as invalid so that this is never attempted again.
                            physicalPath = publishOperation.PhysicalPath;
                        }
                        catch (WebHostingObjectNotFoundException)
                        {
                            siteInvalid = true;
                        }

                        WaitCallback callback = null;
                        Operation operation = new Operation(
                            publishOperation.OperationId,
                            publishOperation.SiteName,
                            physicalPath,
                            PublishHelper.PublishUrl,
                            PublishHelper.AdminCredential);
                        int currentOperationCount = 0;

                        if (siteInvalid)
                        {
                            operation.Status = PublishOperationStatus.SourceOrDestinationInvalid;
                            lock (_completedOperations)
                            {
                                PublishHelper.LogVerboseInformation("ControllerThread: Queuing completed operation for site: {0} as it does not exist", operation.SiteName);
                                _completedOperations.Enqueue(operation);
                            }
                            continue;
                        }

                        PublishHelper.LogVerboseInformation("ControllerThread: Adding Operation {0} for {1} to the queue", operation.SiteName, publishOperation.SiteState);
                        if (publishOperation.SiteState == SiteState.Deleted)
                        {
                            lock (_pendingDeleteOperations)
                            {
                                _pendingDeleteOperations.Enqueue(operation);
                                currentOperationCount += _pendingDeleteOperations.Count;
                            }

                            callback = new WaitCallback(DeleteContent);
                        }
                        else
                        {
                            lock (_pendingPublishOperations)
                            {
                                _pendingPublishOperations.Enqueue(operation);
                                currentOperationCount += _pendingPublishOperations.Count;
                            }

                            callback = new WaitCallback(Publish);
                        }

                        AntaresEventProvider.EventWritePublishFailOverServiceOperationQueued(operation.SiteName);
                        ThreadPool.QueueUserWorkItem(callback);
                        AntaresEventProvider.EventWritePublishFailOverServiceDebugEvent(String.Format("ControllerThread: Current in queue: {0}, maxCount: {1}", currentOperationCount, maxConcurrentOperations));
                        if (currentOperationCount >= maxConcurrentOperations)
                        {
                            /// Dont queue more operations than the max count to let other publishers pick these operations up.
                            /// This does not mean that we will wait for the entire interval. As soon as a thread finishes, the continue event will 
                            /// get signalled and new operations will get picked up till maxcount operations are queued again.
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    AntaresEventProvider.EventWritePublishFailOverServiceUnableToGetNextOperation(ex.ToString());
                }

                #endregion
            }
        }

        private string GetFileServerIP(ref string physicalPath)
        {
            string ipValue = IPReplaceRegex.Match(physicalPath).Groups[2].Value;
            physicalPath = IPReplaceRegex.Replace(physicalPath, ContentPathParameterDefaultValue);
            return ipValue;
        }

        /// <summary>
        /// Web Deploy does not parameterize or run any rules (parameterization is one of the rules) when deletion is used.
        /// So to work around that deletecontent does a simple publish of empty content to the server and gets the IP that is hosting the volume.
        /// After finding the IP the deletion is called on that content path. This will effectively delete "\\someipaddress\volumename\guidfolder"
        /// </summary>
        /// <param name="unusedState">This is not used. This is the signature for the waitcallback expected by threadpool.</param>
        private void DeleteContent(object unusedState)
        {
            Operation operation = null;

            try
            {
                lock (_pendingDeleteOperations)
                {
                    operation = _pendingDeleteOperations.Dequeue();
                }

                if (operation == null)
                {
                    PublishHelper.LogVerboseInformation("DeleteContent: Thread {0} did not find any operations to process", Thread.CurrentThread.ManagedThreadId);
                    return;
                }

                operation.ThreadID = Thread.CurrentThread.ManagedThreadId;
                operation.Status = PublishOperationStatus.Error;
                DeploymentBaseOptions srcBaseOptions = new DeploymentBaseOptions();
                AntaresEventProvider.EventWritePublishFailOverServiceProgressInformation(operation.ThreadID, operation.SiteName);

                string remoteIP = string.Empty;
                bool errorOcurred = false;

                using (DeploymentObject depObj = DeploymentManager.CreateObject(DeploymentWellKnownProvider.ContentPath, Path.GetTempPath(), srcBaseOptions))
                {
                    try
                    {
                        DeploymentBaseOptions destBaseOptions = new DeploymentBaseOptions();
                        destBaseOptions.ComputerName = operation.PublishUrl;
                        string modifiedPhysicalPath = operation.PhysicalPath;
                        string ipDefaultValue = GetFileServerIP(ref modifiedPhysicalPath);

                        var queryServerIPParameter = new DeploymentSyncParameter(
                            QueryServerIPParameterName,
                            QueryServerIPParameterDescription,
                            string.Empty,
                            DeploymentWellKnownTag.PhysicalPath.ToString());
                        var queryServerIPParameterEntry = new DeploymentSyncParameterEntry(
                            DeploymentSyncParameterEntryKind.ProviderPath,
                            DeploymentWellKnownProvider.ContentPath.ToString(),
                            string.Empty,
                            string.Empty);
                        queryServerIPParameter.Add(queryServerIPParameterEntry);

                        var contentPathParameter = new DeploymentSyncParameter(
                            ContentPathParameterName,
                            ContentPathParameterDescription,
                            modifiedPhysicalPath,
                            DeploymentWellKnownTag.PhysicalPath.ToString());
                        var contentParamEntry = new DeploymentSyncParameterEntry(
                            DeploymentSyncParameterEntryKind.ProviderPath,
                            DeploymentWellKnownProvider.ContentPath.ToString(),
                            string.Empty,
                            string.Empty);
                        contentPathParameter.Add(contentParamEntry);

                        depObj.SyncParameters.Add(contentPathParameter);
                        depObj.SyncParameters.Add(queryServerIPParameter);

                        destBaseOptions.UserName = operation.AdminCredential.UserName;
                        destBaseOptions.Password = operation.AdminCredential.Password;
                        destBaseOptions.AuthenticationType = "basic";

                        depObj.SyncTo(destBaseOptions, new DeploymentSyncOptions());
                    }
                    catch (Exception e)
                    {
                        bool unhandledException = false;
                        // In cases where the site was deleted on the source before it was ever synced to the destination
                        // mark as the destination invalid and set the status so that its never attempted again.
                        if (e is DeploymentDetailedException && ((DeploymentDetailedException)e).ErrorCode == DeploymentErrorCode.ERROR_INVALID_PATH)
                        {
                            string[] messageArray = e.Message.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            if (messageArray.Length > 0)
                            {
                                remoteIP = messageArray[0];
                            }
                            else
                            {
                                errorOcurred = true;
                            }
                        }
                        else if (e is DeploymentException)
                        {
                            errorOcurred = true;
                        }
                        else
                        {
                            // its not a deployment exception. This is an exception not handled by web deploy such as duplicate key exception.
                            // This needs to be retried.
                            unhandledException = true;
                            errorOcurred = true;
                        }

                        if (errorOcurred)
                        {
                            AntaresEventProvider.EventWritePublishFailOverServiceFailedToPublishSite(operation.SiteName, e.ToString());
                            operation.Status = unhandledException ? PublishOperationStatus.Error : PublishOperationStatus.SourceOrDestinationInvalid;
                        }
                    }
                }

                if (!errorOcurred)
                {
                    string[] pathParts = operation.PhysicalPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    string remotePath = string.Concat(@"\\", remoteIP, @"\", pathParts[1], @"\", pathParts[2]);

                    using (DeploymentObject depObj = DeploymentManager.CreateObject(DeploymentWellKnownProvider.Auto, string.Empty, srcBaseOptions))
                    {
                        try
                        {
                            DeploymentBaseOptions destBaseOptions = new DeploymentBaseOptions();
                            destBaseOptions.ComputerName = operation.PublishUrl;
                            destBaseOptions.UserName = operation.AdminCredential.UserName;
                            destBaseOptions.Password = operation.AdminCredential.Password;
                            destBaseOptions.AuthenticationType = "basic";
                            destBaseOptions.Trace += new EventHandler<DeploymentTraceEventArgs>(WebDeployPublishTrace);
                            destBaseOptions.TraceLevel = System.Diagnostics.TraceLevel.Verbose;

                            var syncOptions = new DeploymentSyncOptions();
                            syncOptions.DeleteDestination = true;

                            depObj.SyncTo(DeploymentWellKnownProvider.ContentPath, remotePath, destBaseOptions, syncOptions);
                            operation.Status = PublishOperationStatus.Completed;
                            AntaresEventProvider.EventWritePublishFailOverServicePublishComplete(operation.SiteName);
                        }
                        catch (Exception e)
                        {
                            var ex = e as DeploymentDetailedException;
                            if (ex != null && ex.ErrorCode == DeploymentErrorCode.FileOrFolderNotFound)
                            {
                                operation.Status = PublishOperationStatus.SourceOrDestinationInvalid;
                            }
                            else
                            {
                                AntaresEventProvider.EventWritePublishFailOverServiceFailedToPublishSite(operation.SiteName, e.ToString());
                            }
                        }
                    }
                }
            }
            finally
            {
                lock (_completedOperations)
                {
                    PublishHelper.LogVerboseInformation("DeleteContent: Thread {0} queuing completed operation for site: {1}", operation.ThreadID, operation.SiteName);
                    _completedOperations.Enqueue(operation);
                }

                _continueEvent.Set();
                PublishHelper.LogVerboseInformation("DeleteContent: Thread {0} exiting.", Thread.CurrentThread.ManagedThreadId);
            }
        }

        private void Publish(object unusedState)
        {
            Operation operation = null;

            try
            {
                lock (_pendingPublishOperations)
                {
                    operation = _pendingPublishOperations.Dequeue();
                }

                if (operation == null)
                {
                    PublishHelper.LogVerboseInformation("Publish: Thread {0} did not find any operations to process", Thread.CurrentThread.ManagedThreadId);
                    return;
                }

                // This was not required earlier but now that every site will be synced twice for every incoming publish (Look at the postsync override in publishextender), 
                // there are chances that same operation might get scheduled in concurrent threads which is not a supported web deploy scenario.
                bool canContinue = true;
                lock (_operationsInProgress)
                {
                    if (_operationsInProgress.Contains(operation.SiteName))
                    {
                        PublishHelper.LogVerboseInformation("Publish: An operation for {0} is already in progress. Thread {1} will mark it as not started to be scheduled later.", operation.SiteName, Thread.CurrentThread.ManagedThreadId);
                        operation.Status = PublishOperationStatus.NotStarted;
                        canContinue = false;
                    }
                    else
                    {
                        _operationsInProgress.Add(operation.SiteName);
                    }
                }

                if (!canContinue)
                {
                    lock (_completedOperations)
                    {
                        _completedOperations.Enqueue(operation);
                    }
                    return;
                }

                operation.ThreadID = Thread.CurrentThread.ManagedThreadId;
                operation.Status = PublishOperationStatus.Error;

                DeploymentBaseOptions srcBaseOptions = new DeploymentBaseOptions();

                AntaresEventProvider.EventWritePublishFailOverServiceProgressInformation(operation.ThreadID, operation.SiteName);

                using (DeploymentObject depObj = DeploymentManager.CreateObject(DeploymentWellKnownProvider.Manifest, PublishHelper.GetPublishManifest(operation.PhysicalPath), srcBaseOptions))
                {
                    try
                    {
                        DeploymentBaseOptions destBaseOptions = new DeploymentBaseOptions();
                        destBaseOptions.ComputerName = operation.PublishUrl;
                        string modifiedPhysicalPath = operation.PhysicalPath;
                        string ipDefaultValue = GetFileServerIP(ref modifiedPhysicalPath);

                        var fileServerIPParameter = new DeploymentSyncParameter(
                            FileServerIPParameterName,
                            FileServerIPParameterDescription,
                            ipDefaultValue,
                            string.Empty);
                        var fileServerIPEntry = new DeploymentSyncParameterEntry(
                            DeploymentSyncParameterEntryKind.ProviderPath,
                            DeploymentWellKnownProvider.ContentPath.ToString(),
                            string.Empty,
                            string.Empty);
                        fileServerIPParameter.Add(fileServerIPEntry);

                        var contentPathParameter = new DeploymentSyncParameter(
                            ContentPathParameterName,
                            ContentPathParameterDescription,
                            modifiedPhysicalPath,
                            DeploymentWellKnownTag.PhysicalPath.ToString());
                        var contentParamEntry = new DeploymentSyncParameterEntry(
                            DeploymentSyncParameterEntryKind.ProviderPath,
                            DeploymentWellKnownProvider.ContentPath.ToString(),
                            string.Empty,
                            string.Empty);
                        contentPathParameter.Add(contentParamEntry);

                        var setAclParameter = new DeploymentSyncParameter(
                            SetAclParameterName,
                            SetAclParameterDescription,
                            modifiedPhysicalPath,
                            DeploymentWellKnownTag.SetAcl.ToString());
                        var setAclParamEntry = new DeploymentSyncParameterEntry(
                            DeploymentSyncParameterEntryKind.ProviderPath,
                            DeploymentWellKnownProvider.SetAcl.ToString(),
                            string.Empty,
                            string.Empty);
                        setAclParameter.Add(setAclParamEntry);

                        depObj.SyncParameters.Add(fileServerIPParameter);
                        depObj.SyncParameters.Add(contentPathParameter);
                        depObj.SyncParameters.Add(setAclParameter);

                        destBaseOptions.UserName = operation.AdminCredential.UserName;
                        destBaseOptions.Password = operation.AdminCredential.Password;
                        destBaseOptions.AuthenticationType = "basic";
                        destBaseOptions.Trace += new EventHandler<DeploymentTraceEventArgs>(WebDeployPublishTrace);
                        destBaseOptions.TraceLevel = System.Diagnostics.TraceLevel.Verbose;

                        depObj.SyncTo(destBaseOptions, new DeploymentSyncOptions());
                        operation.Status = PublishOperationStatus.Completed;
                        AntaresEventProvider.EventWritePublishFailOverServicePublishComplete(operation.SiteName);
                    }
                    catch (Exception e)
                    {
                        AntaresEventProvider.EventWritePublishFailOverServiceFailedToPublishSite(operation.SiteName, e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                if ((e is DeploymentDetailedException
                        && ((DeploymentDetailedException)e).ErrorCode == DeploymentErrorCode.FileOrFolderNotFound) ||
                        e is WebHostingObjectNotFoundException)
                {
                    operation.Status = PublishOperationStatus.SourceOrDestinationInvalid;
                }

                AntaresEventProvider.EventWritePublishFailOverServiceFailedToGetSourceSite(operation.SiteName, e.ToString());
            }
            finally
            {
                lock (_completedOperations)
                {
                    PublishHelper.LogVerboseInformation("Publish: Thread {0} qeuing completed operation for site: {1}", operation.ThreadID, operation.SiteName);
                    _completedOperations.Enqueue(operation);
                }

                lock (_operationsInProgress)
                {
                    _operationsInProgress.Remove(operation.SiteName);
                }

                _continueEvent.Set();
                PublishHelper.LogVerboseInformation("Publish: Thread {0} exiting", Thread.CurrentThread.ManagedThreadId);
            }
        }

        private static void WebDeployPublishTrace(object sender, DeploymentTraceEventArgs e)
        {
            AntaresEventProvider.EventWritePublishFailOverServiceDebugEvent(e.Message);
        }
    }
}
