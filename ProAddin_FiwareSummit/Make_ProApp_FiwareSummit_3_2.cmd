
echo off
echo =====================================================================================================
echo Make ProApp_FiwareSummit_3_2
echo
echo Authors:
echo roman.trojan@msgis.com 20231001-
echo =====================================================================================================

rem Build specific components if the dependency is set to reference (instead of project).
Set BuildProApp_Common=0

Set MakeInfo="MakeInfo.txt"
echo on > %MakeInfo%
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
echo Start make ProApp_FiwareSummit_3_2 on %date% %time% by %username% on %computername%>> %MakeInfo%
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
REM ProApp_Base_FIWARE_3_2
rem -----------------------------------------------------------------------------------------------------
cd ..\ProAddin_Base_FIWARE
call :Delete_ObjBin
git pull
if "%BuildProApp_Common%" == 1 (
	MsBuild.exe msGIS.ProApp_Base_FIWARE_3_2.csproj -t:Clean -p:Configuration=Release >> %MakeInfo%
	MSBuild.exe ProApp_Base_FIWARE_3_2.sln -m -t:restore -property:Configuration=Release >> %MakeInfo%
	MSBuild.exe ProApp_Base_FIWARE_3_2.sln -m -t:rebuild -property:Configuration=Release >> %MakeInfo%
)
cd %MyDir%

rem -----------------------------------------------------------------------------------------------------
REM ProApp_FiwareSummit_3_2
rem -----------------------------------------------------------------------------------------------------
MsBuild.exe msGIS.ProApp_FiwareSummit_3_2.csproj -t:Clean >> %MakeInfo%
rem devenv.exe ProApp_FiwareSummit_3_2.sln /rebuild Release /out %MakeInfo%
MSBuild.exe ProApp_FiwareSummit_3_2.sln -m -t:restore -property:Configuration=Release >> %MakeInfo%
MSBuild.exe ProApp_FiwareSummit_3_2.sln -m -t:rebuild -property:Configuration=Release >> %MakeInfo%

rem -----------------------------------------------------------------------------------------------------
rem Manage for setup
rem -----------------------------------------------------------------------------------------------------
rem del /Q bin\*.xxx

:End
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
echo Make finished on %date% %time%>> %MakeInfo%
echo ----------------------------------------------------------------------------------------------------->> %MakeInfo%
