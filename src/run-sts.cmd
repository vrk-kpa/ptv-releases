@echo off
set ASPNETCORE_ENVIRONMENT=Development
echo Starting PTV Identity user managment..
title PTV Launcher
cd PTV.IdentityUserManager
start "PTV Identity user managment" dotnet run -c Debug
cd..
