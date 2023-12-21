
echo off
echo =====================================================================================================
echo Install ProApp_FiwareTest_3x
echo
echo Authors:
echo roman.trojan@msgis.com 20231001-
echo =====================================================================================================

Set InstVer=ArcPro_3.2.49743 AddInX_3.3.06 PluginDatasource_FiwareTest_3.3.06 Common_3.3.32 MSI_

rem -----------------------------------------------------------------------------------------------------
rem Testing %date% %time% by %username% on %computername%
rem -----------------------------------------------------------------------------------------------------
Set PathTest="\\md.local\p$\MS\Testhouse\arcgispro\FIWARE-arcgispro\%InstVer% (%username% %computername%)"
if exist %PathTest% (
	rmdir /S /Q %PathTest%
)
if not exist %PathTest% (
	mkdir %PathTest%
)

xcopy /I /Y /R ".\Info FIWARE\ReadMe FIWARE.txt" %PathTest%
Set PathAddInX=.
xcopy /I /Y /R %PathAddInX%\bin\Release\net6.0-windows\*.esriAddinX %PathTest%
Set PathPluginDatasource_EntityFile=..\ProPluginDatasource_EntityFile
xcopy /I /Y /R %PathPluginDatasource_EntityFile%\bin\Release\net6.0-windows\*.esriPlugin %PathTest%
Set PathPluginDatasource_FiwareHttpClient=..\ProPluginDatasource_FiwareHttpClient
xcopy /I /Y /R %PathPluginDatasource_FiwareHttpClient%\bin\Release\net6.0-windows\*.esriPlugin %PathTest%

:End
