echo off
echo Run download transifex translations
dotnet run pipe ../../src/PTV.Application.Web/wwwroot/localization -cmd dc
