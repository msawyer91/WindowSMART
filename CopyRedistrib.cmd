@echo off
cd
echo Copying x64 installer...
echo.
copy WindowSMARTInstallerX64\bin\Release\WindowSMARTInstaller.msi "WindowSMARTSetup x64 redistributable\msi"
echo.
echo Copying x86 installer...
echo.
copy WindowSMARTInstallerX86\bin\Release\WindowSMARTInstaller.msi "WindowSMARTSetup x86 redistributable\msi"
echo.
echo Copying Setup.exe...
echo.
copy WindowSMARTSetup\bin\Release\Setup.exe "WindowSMARTSetup x64 redistributable"
copy WindowSMARTSetup\bin\Release\Setup.exe "WindowSMARTSetup x86 redistributable"
echo.
echo.
echo.
echo copying web deployment files...
copy WindowSMARTInstallerX64\bin\Release\WindowSMARTInstaller.msi Redistrib\WindowSMARTInstaller_x64.msi
copy WindowSMARTInstallerX86\bin\Release\WindowSMARTInstaller.msi Redistrib\WindowSMARTInstaller_x86.msi
copy WindowSMARTLightweightInstallerX64\bin\x64\Release\WindowSMARTLightweightInstaller.msi Redistrib\WindowSMARTLightweightInstaller_x64.msi
copy WindowSMARTLightweightInstallerX86\bin\Release\WindowSMARTLightweightInstaller.msi Redistrib\WindowSMARTLightweightInstaller_x86.msi
copy HomeServerSMART2013Wssx\bin\Release\HomeServerSMART2013.wssx Redistrib\HomeServerSMART2013.wssx
echo Danke!
pause