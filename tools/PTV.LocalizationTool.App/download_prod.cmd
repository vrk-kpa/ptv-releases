echo off
echo Run download transifex translations
dotnet run transifex:project=prod download -o prod
