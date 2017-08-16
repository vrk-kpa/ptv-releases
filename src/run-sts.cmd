@echo off
set ASPNETCORE_ENVIRONMENT=Development
echo Starting PTV Authentication server..
title PTV Launcher
cd AuthenticationServer
start "PTV Auth Server" dotnet run -c Debug
cd..
