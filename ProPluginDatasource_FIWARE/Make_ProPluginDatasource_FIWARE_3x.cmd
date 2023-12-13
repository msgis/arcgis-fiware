
echo off
echo =====================================================================================================
echo Make ProPluginDatasource_FIWARE_3x
echo
echo Authors:
echo roman.trojan@msgis.com 20231213-
echo =====================================================================================================

Set MakeInfo="MakeInfo.txt"
echo on > %MakeInfo%
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
echo Start make ProPluginDatasource_FIWARE_3x on %date% %time% by %username% on %computername%>> %MakeInfo%
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
rem Get latest source
rem -----------------------------------------------------------------------------------------------------
git pull

rem -----------------------------------------------------------------------------------------------------
REM ProPluginDatasource_FIWARE_3x
rem -----------------------------------------------------------------------------------------------------
rem devenv.exe ProPluginDatasource_FIWARE_3x.sln /rebuild Release /out %MakeInfo%
MsBuild.exe msGIS.ProPluginDatasource_FIWARE_3x.csproj -t:Clean -p:Configuration=Release >> %MakeInfo%
MSBuild.exe ProPluginDatasource_FIWARE_3x.sln -m -t:restore -property:Configuration=Release >> %MakeInfo%
MSBuild.exe ProPluginDatasource_FIWARE_3x.sln -m -t:rebuild -property:Configuration=Release >> %MakeInfo%

rem -----------------------------------------------------------------------------------------------------
rem Manage for setup
rem -----------------------------------------------------------------------------------------------------
rem del /Q bin\*.xxx

:End
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
echo Make finished on %date% %time%>> %MakeInfo%
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
