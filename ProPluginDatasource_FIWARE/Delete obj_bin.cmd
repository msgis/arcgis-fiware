
echo off
echo =====================================================================================================
echo Delete [obj, bin] directories to clean compile system
echo
echo Authors:
echo roman.trojan@msgis.com 20190411-
echo =====================================================================================================

Set StartDir="."
Set DirsList=Temp_DirsList.txt
dir /O-G /B /S %StartDir%> %DirsList%

Set DelDir1=\bin
Set DelDir2=\obj
setlocal EnableDelayedExpansion
for /F "eol=* tokens=* delims=*" %%i in ('findstr "%DelDir1% %DelDir2%" %DirsList%') do (
	set FileLine=%%i
	if exist "!FileLine!" (
		set CheckDel=!FileLine:~-4!
		if "!CheckDel!" == "%DelDir1%" (
			echo rmdir /S /Q "!FileLine!"
			rmdir /S /Q "!FileLine!"
		)
		if "!CheckDel!" == "%DelDir2%" (
			echo rmdir /S /Q "!FileLine!"
			rmdir /S /Q "!FileLine!"
		)
	)
)
del %DirsList%
