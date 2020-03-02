cd PTV.Application.Web
set ASPNETCORE_ENVIRONMENT=Development
set DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
start "PTV Web App" dotnet run -c Debug 42621
cd..
