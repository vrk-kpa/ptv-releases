cd PTV.TaskScheduler
set ASPNETCORE_ENVIRONMENT=Development
start "PTV Scheduler" dotnet run -c Debug 42622
cd..
