::Copyright © 2017 SOFTINUX. All rights reserved.
::Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

@echo off

cd %~dp0 
IF "%1" == "/?" GOTO Help
IF "%1" == "-h" GOTO Help
IF "%1" == "clean" GOTO Clean
IF "%1" == "build" GOTO Build
IF "%1" == "copyexts" GOTO CopyDeps
IF "%1" == "copydeps" GOTO CopyExts

GOTO Clean

:Clean
echo ###################
echo CLEAN SOLUTION
echo ###################
dotnet clean
IF "%1" == "clean" GOTO End

:Build
echo ###################
echo BUILD SOLUTION
echo ###################
dotnet build /property:GenerateFullPaths=true
IF "%1" == "build" GOTO End

:CopyDeps
::source https://stackoverflow.com/a/6258198
echo ###################
echo Copy Dependencies
echo ###################
set dst_folder=.\WebApplication\bin\Debug\netcoreapp2.0\
if not exist "%dst_folder%" GOTO End
for /f "tokens=*" %%i in (dependencies.txt) DO (
    xcopy /S/E/Y "%%i" "%dst_folder%"
)
IF "%1" == "copydeps" GOTO End

:CopyExts
echo ###################
echo Copy extensions
echo ###################
set dst_folder=.\WebApplication\Extensions\
if not exist "%dst_folder%" mkdir "%dst_folder%"
for /f "tokens=*" %%i in (extensions.txt) DO (
    xcopy /S/E/Y "%%i" "%dst_folder%"
)

:Help
echo Avaliable parameter is :
echo     - clean : clean solution
echo     - build : only build solution
echo     - copydeps : only copy dependencies (defined in dependencies.txt)
echo     - copyexts : only copy extensions (defined in extensions.txt)
echo.
echo with no parameter or unsupported parameter, build proccess is:
echo     - clean solution
echo     - build solution
echo     - copy dependencies
echo     - copy extensions

GOTO End

:End