<#
cmd command: powershell -ExecutionPolicy ByPass -File .\PTV_List_NPM.ps1
#>

# temp all licenses csv file
$tmpAllLicensesFile = "c:\temp\tmp-ptv-npm-license-crawler.csv"


Write-Host "Getting all licenses for PTV using npm-license-crawler."
Write-Host ""

# write npm-license-crawler csv output to temp file
npm-license-crawler --csv $tmpAllLicensesFile

# read the temp csv and sort by module name desc so that largest version is always first (so that we can later always pick the first result item)
$allLicensesCsv = Import-Csv $tmpAllLicensesFile | sort "module name" -Descending


Write-Host "All npm licenses crawled. Filtering direct npm packages for web and authenitcation server projects."
Write-Host ""


# get only web project direct npm packages using license-report (csv has eight columns and no header)
# add header row to the in memory csv presentation so that we have nice property names for needed columns
$tmpDirectLicenses = license-report --output=csv --package=.\src\PTV.Application.Web\package.json | ConvertFrom-Csv -Header "A", "B", "Name", "C", "D", "License", "Git", "Version"

# collect to this array all used direct web project licenses
$webDirectLicenses = [System.Collections.ArrayList]@()

foreach ($licRow in $tmpDirectLicenses)
{
	# find the matching license by the name
    # for example gulp-cssmin is matched to gulp-cssmin@0.1.7 by splitting the module name using @ character
	# version number not used because for example currently web project package.json has babel-cli: ^6.8.0 definition
	# but used version is actually 6.10.1
	$matched = $allLicensesCsv | where {$_."module name".split("@")[0] -like $licRow.Name}
	
	$webDirectLicenses.Add($matched[0])  | Out-Null #pipe returned item index to null
	
	#Write-Host $matched[0]
	#Write-Host ""
}


# get only authenticationserver project direct npm packages using license-report (csv has eight columns and no header)
# add header row to the in memory csv presentation so that we have nice property names for needed columns
$tmpDirectLicenses = license-report --output=csv --package=.\src\AuthenticationServer\package.json | ConvertFrom-Csv -Header "A", "B", "Name", "C", "D", "License", "Git", "Version"

# collect to this array all used direct authenticationserver licenses
$authDirectLicenses = [System.Collections.ArrayList]@()

foreach ($licRow in $tmpDirectLicenses)
{
	# find the matching license by the name
    # for example gulp-cssmin is matched to gulp-cssmin@0.1.7 by splitting the module name using @ character
	# version number not used because for example currently web project package.json has babel-cli: ^6.8.0 definition
	# but used version is actually 6.10.1
	$matched = $allLicensesCsv | where {$_."module name".split("@")[0] -like $licRow.Name}
	
	$authDirectLicenses.Add($matched[0])  | Out-Null #pipe returned item index to null
	
	#Write-Host $matched[0]
	#Write-Host ""
}


# create csv file for web direct references
# using unicode (could also use UTF8 with BOM) encoding so that the text import wizard in Excel is started when importing the file
$webCsvFile = "c:\temp\ptv-web-npm-packages.csv"
$webDirectLicenses | Export-Csv $webCsvFile -notype -encoding "Unicode"
Write-Host ""
Write-Host ("Wrote web project npm package listing to: {0}." -f $webCsvFile)
Write-Host ""


# create csv file for authentication server direct references
# using unicode (could also use UTF8 with BOM) encoding so that the text import wizard in Excel is started when importing the file
$authCsvFile = "c:\temp\ptv-authenticationserver-npm-packages.csv"
$authDirectLicenses | Export-Csv $authCsvFile -notype -encoding "Unicode"
Write-Host ""
Write-Host ("Wrote AuthenticationServer npm package listing to: {0}." -f $authCsvFile)

Write-Host ""
Write-Host "Done."
