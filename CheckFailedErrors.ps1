$exitCode = 0;

Get-ChildItem -Path .\* -Filter *.Error.log | ForEach-Object {
    $content = Get-Content $_.FullName;
    If (($content) -ne $Null) {
	    $exitCode = 1;
        $content;
    }
}

exit $exitCode