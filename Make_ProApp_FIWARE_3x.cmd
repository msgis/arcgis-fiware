
echo off
echo =====================================================================================================
echo Make ProApp_FIWARE_3x
echo
echo Authors:
echo roman.trojan@msgis.com 20231001-
echo =====================================================================================================

rem Build Common if the dependency is set to reference (instead of project).
Set BuildCommon=0

Set MakeInfo="MakeInfo.txt"
echo on > %MakeInfo%
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
echo Start make ProApp_FIWARE_3x on %date% %time% by %username% on %computername%>> %MakeInfo%
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%

rem -----------------------------------------------------------------------------------------------------
rem Manage to compile
rem -----------------------------------------------------------------------------------------------------
rem attrib -r bin\*.* /S
rem del /S /Q bin\*.*
rem del /S /Q obj
if exist obj (
	rmdir /S /Q obj
)
if exist bin (
	rmdir /S /Q bin
)

rem -----------------------------------------------------------------------------------------------------
REM ProApp_Common_FIWARE_3x
rem -----------------------------------------------------------------------------------------------------
set MyDir=%CD%
cd %MyDir%
cd ..\..\ProApp-Common
git pull
if "%BuildCommon%" == 1 (
	MsBuild.exe msGIS.ProApp_Common_FIWARE_3x.csproj -t:Clean -p:Configuration=Release >> %MakeInfo%
	MSBuild.exe ProApp_Common_FIWARE_3x.sln -m -t:restore -property:Configuration=Release >> %MakeInfo%
	MSBuild.exe ProApp_Common_FIWARE_3x.sln -m -t:rebuild -property:Configuration=Release >> %MakeInfo%
)
cd %MyDir%

rem -----------------------------------------------------------------------------------------------------
REM ProApp_FIWARE_3x
rem -----------------------------------------------------------------------------------------------------
git pull
MsBuild.exe msGIS.ProApp_FIWARE_3x.csproj -t:Clean >> %MakeInfo%
rem devenv.exe ProApp_FIWARE_3x.sln /rebuild Release /out %MakeInfo%
MSBuild.exe ProApp_FIWARE_3x.sln -m -t:restore -property:Configuration=Release >> %MakeInfo%
MSBuild.exe ProApp_FIWARE_3x.sln -m -t:rebuild -property:Configuration=Release >> %MakeInfo%

rem -----------------------------------------------------------------------------------------------------
rem Manage for setup
rem -----------------------------------------------------------------------------------------------------
rem del /Q bin\*.xxx

:End
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
echo Make finished on %date% %time%>> %MakeInfo%
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
