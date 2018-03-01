@echo off
set ASPNETCORE_ENVIRONMENT=Development
echo Starting PTV Console app..
title PTV Launcher
cd PTV.DataImport.Console
start "PTV Console app" dotnet run -c Debug
cd..
