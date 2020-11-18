echo off
echo Run download transifex translations
dotnet run transifex:WorkingFolder="../../src/PTV.Application.Web/wwwroot/localization" pipe -cmd dc
