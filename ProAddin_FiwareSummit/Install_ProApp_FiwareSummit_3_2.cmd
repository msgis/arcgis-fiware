
echo off
echo =====================================================================================================
echo Install ProApp_FiwareSummit_3_2
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
Set BaseVer=3.3.43
Set AddInxVer=3.4.18
Set MsiVer=(none)

rem -----------------------------------------------------------------------------------------------------
echo Versioning %AddInxVer% by %username% on %computername%
echo %date% %time%
rem 3.3.19/20240904/msGIS_FiwareReader_rt_100: Reduce install path length.
rem -----------------------------------------------------------------------------------------------------
Set FileNameInfo=ReadMe FiwareSummit
Set PathFileInfo=.\Info FiwareSummit\%FileNameInfo%.txt
Set TargetPath=FiwareSummit_%AddInxVer%-ArcGISPro_%ProVer%
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

Set FileVersion=InfoVersion_%AddInxVer%.txt
Set PathFileVersion=%PathDevelop%\%FileVersion%
echo Setup=%AddInxVer%>> %PathFileVersion%
echo Desktop_%desktopVersion%>> %PathFileVersion%
echo ArcGISPro_%ProVer%>> %PathFileVersion%
echo Base_%BaseVer%>> %PathFileVersion%
echo MSI_%MsiVer%>> %PathFileVersion%
echo User=%username%>> %PathFileVersion%
echo Computer=%computername%>> %PathFileVersion%
echo Date=%date%>> %PathFileVersion%
echo Time=%time%>> %PathFileVersion%
copy /Y "%PathFileVersion%" %PathTest%\"%FileVersion%"

copy /Y "%PathFileInfo%" %PathDevelop%\"%FileNameInfo%".txt
copy /Y "%PathFileInfo%" %PathTest%\"%FileNameInfo%".txt
xcopy /I /Y /R .\bin\Release\net6.0-windows\*.esriAddinX %PathDevelop%
xcopy /I /Y /R .\bin\Release\net6.0-windows\*.esriAddinX %PathTest%

:End
