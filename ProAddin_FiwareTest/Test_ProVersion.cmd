echo off
echo =====================================================================================================
echo Test ProVersion
echo
echo Authors:
echo roman.trojan@msgis.com 20240111-
echo =====================================================================================================

rem powershell.exe -command "& {Get-ItemProperty -Path 'HKLM:\SOFTWARE\ESRI\ArcGISPro' -Name 'Version'}" ^| Select-Object -ExpandProperty Version
Set ProVer=
for /f "tokens=1,2 delims=: " %%a in ('powershell.exe -command "& {Get-ItemProperty -Path 'HKLM:\SOFTWARE\ESRI\ArcGISPro' -Name 'Version'}"') do (
	rem echo %%a
	if "%%a" == "Version" (
		set "ProVer=%%b"
	)
)
echo %ProVer%

:End