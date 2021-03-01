echo off
echo Run remove translated texts
dotnet run clean -s downloaded -o downloadedEmpty -tr

echo Run update changed key by previous text
dotnet run copy -s downloadedEmpty -o merged -d old -k keyChangedNew

echo Run update default text for transifex files
dotnet run update -s merged -o def -d default

echo Run clean transifex files
dotnet run clean -s def -o clean