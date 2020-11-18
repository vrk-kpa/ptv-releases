cd PTV.Application.Api
set ASPNETCORE_ENVIRONMENT=Development
set DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
start "PTV UI Api App" dotnet run -c Debug 42623
cd..
