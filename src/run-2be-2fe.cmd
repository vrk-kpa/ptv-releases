cd PTV.Application.Api
set ASPNETCORE_ENVIRONMENT=Development
start "dotnet UI API" dotnet run
cd ../PTV.Application.Web
set ASPNETCORE_ENVIRONMENT=Development
start "dotnet WEB" dotnet run
cd OldApp
if exist node_modules (start "react OLD" cmd /c "yarn dev") else (start "react OLD" cmd /c "yarn && yarn dev")
cd ../ClientApp
if exist node_modules (npm start) else (npm i && npm start)
