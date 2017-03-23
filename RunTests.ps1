
Get-ChildItem -Path .\test\*.Tests\ | ForEach-Object { $xml = "$($_.Name)TestResult.xml"; $log= "$($_.Name).Error.log"; dotnet test $_.FullName -xml $xml 2> $log }

exit 0