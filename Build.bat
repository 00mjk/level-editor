@echo off
set NETFX_VERSION=4.0.30319
set BUILD="%WINDIR%\Microsoft.NET\Framework\v%NETFX_VERSION%\MSBuild.exe"

set SOLUTION_NAME=LevelEditor

%BUILD% %SOLUTION_NAME%.sln /t:Rebuild /p:Configuration=Debug /p:PlatformTarget=AnyCPU
%BUILD% %SOLUTION_NAME%.sln /t:Rebuild /p:Configuration=Release /p:PlatformTarget=AnyCPU

set err=%errorlevel%

del %SOLUTION_NAME%.sln.cache

if %err% neq 0 pause
