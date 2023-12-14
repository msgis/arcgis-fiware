
echo off
echo =====================================================================================================
echo Make ProPluginDatasource_EntityFile_3x
echo
echo Authors:
echo roman.trojan@msgis.com 20231213-
echo =====================================================================================================

rem Build specific components if the dependency is set to reference (instead of project).
Set BuildProApp_Common=0

Set MakeInfo="MakeInfo.txt"
echo on > %MakeInfo%
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
echo Start make ProPluginDatasource_EntityFile_3x on %date% %time% by %username% on %computername%>> %MakeInfo%
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
goto :Main

rem -----------------------------------------------------------------------------------------------------
rem Manage to compile
rem -----------------------------------------------------------------------------------------------------
:Delete_ObjBin
	rem attrib -r bin\*.* /S
	rem del /S /Q bin\*.*
	rem del /S /Q obj
	if exist obj (
		rmdir /S /Q obj
	)
	if exist bin (
		rmdir /S /Q bin
	)
goto :eof
exit /b

rem -----------------------------------------------------------------------------------------------------
REM Main
rem -----------------------------------------------------------------------------------------------------
:Main
call :Delete_ObjBin
git pull

set MyDir=%CD%
cd %MyDir%

rem -----------------------------------------------------------------------------------------------------
REM ProApp_Common_FIWARE_3x
rem -----------------------------------------------------------------------------------------------------
cd ..\..\..\ProApp-Common
call :Delete_ObjBin
git pull
if "%BuildProApp_Common%" == 1 (
	MsBuild.exe msGIS.ProApp_Common_FIWARE_3x.csproj -t:Clean -p:Configuration=Release >> %MakeInfo%
	MSBuild.exe ProApp_Common_FIWARE_3x.sln -m -t:restore -property:Configuration=Release >> %MakeInfo%
	MSBuild.exe ProApp_Common_FIWARE_3x.sln -m -t:rebuild -property:Configuration=Release >> %MakeInfo%
)
cd %MyDir%

rem -----------------------------------------------------------------------------------------------------
REM ProPluginDatasource_EntityFile_3x
rem -----------------------------------------------------------------------------------------------------
rem devenv.exe ProPluginDatasource_EntityFile_3x.sln /rebuild Release /out %MakeInfo%
MsBuild.exe msGIS.ProPluginDatasource_EntityFile_3x.csproj -t:Clean -p:Configuration=Release >> %MakeInfo%
MSBuild.exe ProPluginDatasource_EntityFile_3x.sln -m -t:restore -property:Configuration=Release >> %MakeInfo%
MSBuild.exe ProPluginDatasource_EntityFile_3x.sln -m -t:rebuild -property:Configuration=Release >> %MakeInfo%

rem -----------------------------------------------------------------------------------------------------
rem Manage for setup
rem -----------------------------------------------------------------------------------------------------
rem del /Q bin\*.xxx

:End
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
echo Make finished on %date% %time%>> %MakeInfo%
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
