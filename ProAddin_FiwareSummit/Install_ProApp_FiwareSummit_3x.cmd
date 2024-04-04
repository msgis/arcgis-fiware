
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

Set InstVer=ArcPro_3.2.49743 AddInX_3.4.14 Common_3.3.39 MSI_

rem -----------------------------------------------------------------------------------------------------
rem Testing %date% %time% by %username% on %computername%
rem -----------------------------------------------------------------------------------------------------
Set PathTest="\\md.local\p$\MS\Testhouse\arcgispro\FiwareSummit-arcgispro\FiwareSummit %InstVer% (%username% %computername% ArcGISPro_%ProVer%)"
if exist %PathTest% (
	rmdir /S /Q %PathTest%
)
if not exist %PathTest% (
	mkdir %PathTest%
)

xcopy /I /Y /R ".\Info FiwareSummit\ReadMe FiwareSummit.txt" %PathTest%
Set PathAddInX=.
xcopy /I /Y /R %PathAddInX%\bin\Release\net6.0-windows\*.esriAddinX %PathTest%

:End
