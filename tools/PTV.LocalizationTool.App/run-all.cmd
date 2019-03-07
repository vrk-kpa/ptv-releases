echo off
echo Run update changed key by previous text
dotnet run copy -s downloaded -o merged -d downloaded -k keyChangedNew

echo Run update default text for transifex files
dotnet run update -s merged -o def -d default

echo Run clean transifex files
dotnet run clean -s def -o clean