
echo off
echo =====================================================================================================
echo Install ProApp_FiwareSummit_3x
echo
echo Authors:
echo roman.trojan@msgis.com 20231001-
echo =====================================================================================================

rem powershell.exe -command "& {Get-ItemProperty -Path 'HKLM:\SOFTWARE\ESRI\ArcGISPro' -Name 'Version'}" ^| Select-Object -ExpandProperty Version
Set ProVer=
for /f "tokens=1,2 delims=: " %%a in ('powershell.exe -command "& {Get-ItemProperty -Path 'HKLM:\SOFTWARE\ESRI\ArcGISPro' -Name 'Version'}"') do (
	rem echo %%a - %%b
	if "%%a" == "Version" (
		set "ProVer=%%b"
	)
)
rem echo %ProVer%

rem wmic datafile where name="C:\\Program Files\\ArcGIS\\Pro\\bin\\ArcGISPro.exe" get Version /value
for /f "tokens=1,2 delims==" %%a in ('wmic datafile where "name='C:\\Program Files\\ArcGIS\\Pro\\bin\\ArcGISPro.exe'" get Version /value') do (
	rem echo %%a - %%b
	if "%%a" == "Version" (
		set "ProVer=%%b"
	)
)
rem echo %ProVer%

Set desktopVersion=3.2.49743
Set BaseVer=3.3.42
Set AddInxVer=3.4.17
Set MsiVer=(none)
Set InstVer=%AddInxVer% ArcPro_%desktopVersion% Base_%BaseVer% MSI_%MsiVer% (%username% %computername% ArcGISPro_%ProVer%)

rem -----------------------------------------------------------------------------------------------------
rem Testing %date% %time% by %username% on %computername%
rem -----------------------------------------------------------------------------------------------------
Set FileNameInfo=ReadMe FiwareSummit
Set PathFileInfo=.\Info FiwareSummit\%FileNameInfo%.txt
Set TargetPath=FiwareSummit_%AddInxVer%
Set PathDevelop=.\%TargetPath%
Set PathTest=\\md.local\p$\MS\Testhouse\arcgispro\FIWARE\FiwareSummit-arcgispro\%TargetPath%
if exist %PathDevelop% (
	rmdir /S /Q %PathDevelop%
)
if not exist %PathDevelop% (
	mkdir %PathDevelop%
)
if exist %PathTest% (
	rmdir /S /Q %PathTest%
)
if not exist %PathTest% (
	mkdir %PathTest%
)

copy /Y "%PathFileInfo%" %PathTest%\"%FileNameInfo% %InstVer%".txt
copy /Y "%PathFileInfo%" %PathDevelop%\"%FileNameInfo% %InstVer%".txt
xcopy /I /Y /R .\bin\Release\net6.0-windows\*.esriAddinX %PathTest%
xcopy /I /Y /R .\bin\Release\net6.0-windows\*.esriAddinX %PathDevelop%

:End
