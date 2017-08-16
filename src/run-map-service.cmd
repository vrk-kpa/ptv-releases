@echo off
set ASPNETCORE_ENVIRONMENT=Development
echo Starting PTV Map Server..
title PTV Launcher
cd PTV.MapServer
start "PTV MapServer" dotnet run -c Debug 42623
cd..
