echo off

REM Manually Deploying a ClickOnce Application
REM http://msdn.microsoft.com/en-us/library/xc3tc5xx.aspx

REM Use ClickOnce to Deploy Applications That Can Run on Multiple Versions of the .NET Framework
REM http://msdn.microsoft.com/en-us/library/ee517334.aspx

set Update=0
set Version=1.5.0.0
set Name=CompatCheckAndMigrate
set Product=Azure Websites Migration Assistant
set Publisher="Microsoft"
set PublishingLocation=https://www.movemetothecloud.net
set SupportURL=https://www.movemetothecloud.net
set Tools="C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools"
set Mage=%Tools%\mage.exe
set CertPath="AzureWebsitesMigrationAssistant_TemporaryKey.pfx"
set CertPwd=""

echo Create ClickOnce package for first time.

echo 1. Create a directory where you will store your ClickOnce deployment files.
md %Name%

echo 2. In the deployment directory you just created, create a version subdirectory. (The version of your deployment can be distinct from the version of your application)
md %Name%\%Version%

echo 3. Copy all of your application files to the version subdirectory, including executable files, assemblies, resources, and data files. If necessary, you can create additional subdirectories that contain additional files.
xcopy %Name%.exe %Name%\%Version% /y
xcopy %Name%.exe.config %Name%\%Version% /y
xcopy *.dll %Name%\%Version% /y

echo 4. Create the application manifest with a call to Mage.exe. The following statement creates an application manifest for code compiled to run on the Intel x86 processor.
pushd %Name%\%Version%
%Mage% -New Application -Processor msil -ToFile "%Name%.exe.manifest" -Name "%Name%" -Version %Version% -FromDirectory . 

echo 4.1 Change the application manifest to mark dependent assemblies as .NET Framework assemblies
%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe -Command " & { $content = Get-Content %Name%.exe.manifest; $content | Foreach-Object { $_ -replace '<dependentAssembly dependencyType=\"preRequisite\" allowDelayedBinding=\"true\">', '<dependentAssembly dependencyType=\"preRequisite\" allowDelayedBinding=\"true\" group=\"framework\">' } | Set-Content %Name%.exe.manifest -Force }"

echo 4.2 Update the version number of the <assemblyIdentity> element for Microsoft.Windows.CommonLanguageRuntime to the version number for the .NET Framework that is the lowest common denominator. For example, if the application targets .NET Framework 3.5 and .NET Framework 4, use the 2.0.50727.0 version number
%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe -Command " & { $content = Get-Content %Name%.exe.manifest; $content | Foreach-Object { $_ -replace '<assemblyIdentity name=\"Microsoft.Windows.CommonLanguageRuntime\" version=\"4.0.30319.0\" />', '<assemblyIdentity name=\"Microsoft.Windows.CommonLanguageRuntime\" version=\"2.0.50727.0\" />' } | Set-Content %Name%.exe.manifest -Force }"

popd

echo 5. Sign the application manifest with your Authenticode certificate. Replace mycert.pfx with the path to your certificate file. Replace passwd with the password for your certificate file.
%Mage% -Sign "%Name%\%Version%\%Name%.exe.manifest" -CertFile %CertPath%

echo 5.1. Rename files to .deploy
pushd %Name%\%Version%
ren CompatCheckAndMigrate.exe CompatCheckAndMigrate.exe.deploy
ren CompatCheckAndMigrate.exe.config CompatCheckAndMigrate.exe.config.deploy
REM ren System.Web.Extensions.dll System.Web.Extensions.dll.deploy
popd

echo 6. Generate the deployment manifest with a call to Mage.exe. By default, Mage.exe will mark your ClickOnce deployment as an installed application, so that it can be run both online and offline. To make the application available only when the user is online, use the -Install option with a value of false. If you use the default, and users will install your application from a Web site or file share, make sure that the value of the -ProviderUrl option points to the location of the application manifest on the Web server or share.
%Mage% -New Deployment -Processor msil -Install true -Publisher %Publisher% -SupportURL "%SupportURL%" -ProviderUrl "%PublishingLocation%/%Name%.application" -AppManifest "%Name%\%Version%\%Name%.exe.manifest" -ToFile "%Name%.application"
	
echo 7. Replace strings
echo 7.1. Add mapFileExtensions
%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe -Command " & { $content = Get-Content %Name%.application; $content | Foreach-Object { $_ -replace '<deployment install=\"true\">', '<deployment install=\"true\" mapFileExtensions=\"true\">' } | Set-Content %Name%.application -Force }"

echo 7.2. Replace product name
%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe -Command " & { $content = Get-Content %Name%.application; $content | Foreach-Object { $_ -replace 'asmv2:product=\"%Name%\"', 'asmv2:product=\"%Product%\"' } | Set-Content %Name%.application -Force }"

echo 7.3. Replace expiration configuration
%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe -Command " & { $content = Get-Content %Name%.application; $content | Foreach-Object { $_ -replace '<expiration maximumAge=\"0\" unit=\"days\" />', '<beforeApplicationStartup />' } | Set-Content %Name%.application -Force }"       

echo 7.4. Replace supported versions
%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe -Command " & { $content = Get-Content %Name%.application; [System.Text.RegularExpressions.Regex]::Replace($content, '<compatibleFrameworks xmlns=\"urn:schemas-microsoft-com:clickonce.v2\">.*</compatibleFrameworks>', '<compatibleFrameworks xmlns=\"urn:schemas-microsoft-com:clickonce.v2\"><framework targetVersion=\"4.0\" profile=\"Client\" supportedRuntime=\"4.0.30319\" /><framework targetVersion=\"4.0\" profile=\"Full\" supportedRuntime=\"4.0.30319\" /><framework targetVersion=\"3.5\" profile=\"Client\" supportedRuntime=\"2.0.50727\" /><framework targetVersion=\"3.5\" supportedRuntime=\"2.0.50727\" /><framework targetVersion=\"3.0\" supportedRuntime=\"2.0.50727\" /></compatibleFrameworks>') | Set-Content %Name%.application -Force }"
	
echo 8. Sign .application file
if "%Update%"=="0" (	
	%Mage% -Sign "%Name%.application" -CertFile %CertPath%
) else (
rem #  -CertFile %CertPath%
	%Mage% -Update "%Name%.application" -AppManifest "%Name%\%Version%\%Name%.exe.manifest" -Version %Version% -Publisher %Publisher%
)

echo 9. Copy .application file to version folder (optionally)
xcopy %Name%.application %Name%\%Version% /y

pause
