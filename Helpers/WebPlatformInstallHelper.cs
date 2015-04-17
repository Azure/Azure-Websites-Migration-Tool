using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CompatCheckAndMigrate.Controls;
using Microsoft.Web.PlatformInstaller;

namespace CompatCheckAndMigrate.Helpers
{
    public abstract class InstallHelper : IDisposable
    {
        private StreamWriter _writer;
        public abstract void BeginInstall(InstallerControl control);
        public abstract List<string> ProductList { get; }
        public bool IsRebootNeeded { get; protected set; }

        public InstallHelper()
        {
            _writer = new StreamWriter(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "webpi.log"));
            _writer.AutoFlush = true;
        }

        public void LogInformation(string format, params object[] args)
        {
            string message = format;
            try
            {
                message = string.Format(format, args);
            }
            catch
            {
                // in case wrong format was used. At least log the format string
            }

            _writer.WriteLine(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // this call should never be called as the Dispose should have been called, but leaving it as it just to be safe
        ~InstallHelper()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _writer.Dispose();
            }
        }
    }

    public class WebPlatformInstallHelper : InstallHelper
    {
        private const string WebPiFeedUrl = @"http://www.microsoft.com/web/webpi/5.0/webproductlist.xml";
        private InstallManager _installManager;
        private bool _isInstallChainComplete = false;
        private InstallerControl _installControl;
        private List<string> _productList;
        private bool _hasInstallers;

        public void SetupInstall(string[] productList)
        {
            _hasInstallers = false;
            _installControl = null;
            _installManager = new InstallManager();
            _productList = new List<string>();
            _installManager.InstallerStatusUpdated += new EventHandler<InstallStatusEventArgs>(InstallerStatusUpdated);
            _installManager.InstallCompleted += new EventHandler<EventArgs>(InstallerCompletedCallback);

            var productManager = new ProductManager();
            // Load feed specifying that enclosure files are needed.                    
            productManager.Load(new Uri(WebPiFeedUrl), true, true, false, Path.GetTempPath());

            var productsToInstall = new List<Product>();
            var uniqueInstallerList = new Dictionary<string, Installer>(StringComparer.OrdinalIgnoreCase);
            var languageOfInstallers = productManager.GetLanguage(Thread.CurrentThread.CurrentCulture.ToString());
            var english = productManager.GetLanguage("en");

            foreach (var productName in productList)
            {
                Product product = productManager.GetProduct(productName);
                LogInformation("Initial Product to install: {0}", product.ProductId);
                if (!product.IsInstalled(true))
                {
                    AddProductWithDependencies(product, productsToInstall);

                    foreach (Product productToInstall in productsToInstall)
                    {
                        Installer currentInstaller = productToInstall.GetInstaller(languageOfInstallers);
                        if (currentInstaller == null)
                        {
                            currentInstaller = productToInstall.GetInstaller(english);
                        }

                        if (currentInstaller != null)
                        {
                            if (!uniqueInstallerList.ContainsKey(currentInstaller.Product.ProductId))
                            {
                                uniqueInstallerList[currentInstaller.Product.ProductId] = currentInstaller;
                                LogInformation("Adding product: {0}", currentInstaller.Product.ProductId);
                                _productList.Add(currentInstaller.Product.ProductId);
                            }
                            _hasInstallers = true;
                        }
                    }
                }
            }

            if (_hasInstallers)
            {
                _installManager.Load(uniqueInstallerList.Values);
            }
        }

        public override List<string> ProductList
        {
            get { return _productList; }
        }

        private void WaitForInstallationToComplete()
        {
            while (!_isInstallChainComplete)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        public override void BeginInstall(InstallerControl control)
        {
            if (_hasInstallers)
            {
                _installControl = control;
                _installManager.StartInstallation();

                WaitForInstallationToComplete();
            }
        }

        private static void AddProductWithDependencies(Product product, List<Product> productsToInstall)
        {
            if (!productsToInstall.Contains(product))
            {
                productsToInstall.Add(product);
            }

            ICollection<Product> missingDependencies = product.AllDependencies;
            if (missingDependencies != null)
            {
                foreach (Product dependency in missingDependencies.Where(s => !s.IsApplication))
                {
                    if (dependency.ProductId.IndexOf(MigrationConstants.ProductToSkip, StringComparison.OrdinalIgnoreCase) ==0)
                    {
                        continue;
                    }

                    if (!dependency.IsInstalled(true))
                    {
                        if (!dependency.External && !dependency.ExternalWarningShown && !dependency.IsIisComponent)
                        {
                            productsToInstall.Add(dependency);
                        }
                    }
                }
            }
        }

        private void InstallerStatusUpdated(object sender, InstallStatusEventArgs e)
        {
            string productName = e.InstallerContext.Id;
            switch (e.InstallerContext.InstallationState)
            {
                case InstallationState.Canceled:
                case InstallationState.DependencyFailed:
                case InstallationState.DownloadFailed:
                    _installControl.UpdateInstallStatus(productName, "Failed to Download");
                    LogInformation("Failed to download: {0}", productName);
                    break;
                case InstallationState.Downloading:
                    _installControl.UpdateInstallStatus(productName, "Downloading");
                    LogInformation("Downloading: {0}", productName);
                    break;
                case InstallationState.Downloaded:
                    _installControl.UpdateInstallStatus(productName, "Download complete");
                    LogInformation("Download complete: {0}", productName);
                    break;
                case InstallationState.Installing:
                    _installControl.UpdateInstallStatus(productName, "Installing");
                    LogInformation("Installing: {0}", productName);
                    break;
                case InstallationState.InstallCompleted:
                    _installControl.UpdateInstallStatus(productName, "Installed", true);
                    LogInformation("Installed: {0}", productName);
                    break;
            }

            // Reboot-related events
            if (e.InstallerContext.ReturnCode.Status == InstallReturnCodeStatus.SuccessRebootRequired ||
                e.InstallerContext.ReturnCode.Status == InstallReturnCodeStatus.FailureRebootRequired)
            {
                this.IsRebootNeeded = true;
                _installControl.UpdateInstallStatus(productName, "Reboot required", true);
                LogInformation("Reboot required: {0}", productName);
            }
        }

        private void InstallerCompletedCallback(object sender, EventArgs e)
        {
            _isInstallChainComplete = true;
            LogInformation("marking complete");
            _installControl.MarkAllProductsComplete();
        }
    }
}
