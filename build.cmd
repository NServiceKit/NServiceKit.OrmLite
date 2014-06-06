@echo off

.\src\.nuget\nuget install

SETLOCAL EnableExtensions EnableDelayedExpansion
set _major=0&set _minor=0&set _build=0
FOR /F "tokens=2-4 delims=." %%i IN ('dir /b nuget-packages\NServiceKit.*') DO (
	if %%i GTR !_major! (set _major=%%i&set _minor=%%j&set _build=%%k)
	if %%i==!_major! if %%j GTR !_minor! (set _minor=%%j&set _build=%%k)
	if %%i==!_major! if %%j==!_minor! if %%k GTR !_build! (set _build=%%k)
)
ENDLOCAL & SET NSERVICEKIT_VERSIONED_PATH=NServiceKit.%_major%.%_minor%.%_build%

SET NSERVICEKIT=%~dp0nuget-packages\%NSERVICEKIT_VERSIONED_PATH%\
COPY "%NSERVICEKIT%lib\net35\*.*" .\lib\


:START_BUILD

if "%BUILD_NUMBER%" == "" (
   set BUILD_NUMBER=%APPVEYOR_BUILD_NUMBER%
)

set target=%1
if "%target%" == "" (
   set target=UnitTests
)

if "%target%" == "NuGetPack" (
	if "%BUILD_NUMBER%" == "" (
	 	echo BUILD_NUMBER environment variable is not set.
		exit;
	)
)

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Build\Build.proj /target:%target% /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false