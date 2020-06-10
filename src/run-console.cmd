@echo off
set ASPNETCORE_ENVIRONMENT=Development
set DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
echo Starting PTV Console app..
title PTV Launcher
cd PTV.DataImport.Console
start "PTV Console app" dotnet run -c Debug
cd..
