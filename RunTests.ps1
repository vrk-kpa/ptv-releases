Remove-Item *.trx
Get-ChildItem -Path .\test\*.Tests\ | ForEach-Object { $xml = "$($_.Name)TestResult.xml"; $log= "$($_.Name).Error.log"; Remove-Item "$_\TestResults\*.trx" -ErrorAction Ignore ; dotnet test "$_\$($_.Name).csproj" --logger:trx 2> $log; Copy-Item "$_\TestResults\*.trx" . -force}
exit 0