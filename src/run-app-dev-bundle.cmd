cd PTV.Application.Web
set ASPNETCORE_ENVIRONMENT=Development
start "PTV Web App" dotnet run -c Debug 42621
start yarn prod:sourcemap
cd..
