<#
cmd command: powershell -ExecutionPolicy ByPass -File .\PTV_List_Bower.ps1
#>

$newJsonFile = "c:\temp\PTV_BOWER_LICENSES.csv"

# don't use recurse we want only the project level bower.json files (node modules also containt bower.json but we don't want those)
$filteredFiles = Get-ChildItem .\*\*\* -Filter bower.json

# unique packages we are going to write to the out file
$packageList = [System.Collections.ArrayList]@()
$addedPackageNames = [System.Collections.ArrayList]@()

# variable to hold the combined new JSON files properties
#$newObjectProps = [Ordered]@{}
# store property name aka package name keys here because the ordered hashtable doesn't support ContainsKey method
#$newObjectKeys = @{}

Write-Host "Found bower.json files:"
Write-Host ""

foreach ($originalFile in $filteredFiles)
{	
	Write-Host $originalFile.FullName
	# uncomment below to write more "debug" info about found bower.json files
	<#Write-Host ("FileName: {0}" -f $originalFile.Name)
	Write-Host ("BaseName: {0}" -f $originalFile.BaseName)
	Write-Host ("DirectoryName: {0}" -f $originalFile.DirectoryName)
	Write-Host ""
	#>
	
	# change to the bower.json directory and then run the npm bower-license
	Push-Location $originalFile.DirectoryName
	[Environment]::CurrentDirectory = $PWD
	
	# get the output from bower-license and convert it to PS JSON object
	$tmpJsonInfo = bower-license -e json | Out-String | ConvertFrom-Json
	
	# package names are as properties in the JSON, so take those that we can enumerate the packages 
	# and store package names for next bower.json file to compare have we already added that package to the list
	$packageNames = $tmpJsonInfo.psobject.properties.name
	
	# enumerate the package name properties
	foreach ($pn in $packageNames)
	{		
		# check that we haven't already added the package
		if(!$addedPackageNames.Contains("$pn"))
		{
			# add the package name to the list of added
			$addedPackageNames.Add("$pn") | Out-Null #pipe returned item index to null
			
			# package name
			$name = $null
			# package version
			$version = $null
			
			# split the original package name and create name and version variables
			$splitted = $pn.split("@")
			
			$name = $splitted[0]
			
			if ($splitted.length -gt 1)
			{
				$version = $splitted[1]
			}
			
			# get the JSON property for current package and then select needed info to a new object
			$pgk = $tmpJsonInfo."$pn" | Select @{n='Name';e={$name}}, @{n='Version';e={$version}},@{n='Licenses';e={$_.licenses -join ", "}}, @{n='Repository url';e={$_.repository.url}}, homepage
			
			# create new object out of the JSON property
			$packageList.Add($pgk) | Out-Null #pipe returned item index to null
		}
	}
	
	# change back to PS script directory
	Pop-Location
	[Environment]::CurrentDirectory = $PWD
}

# write the packages to a csv file
$packageList | Export-Csv $newJsonFile -notype -encoding "Unicode"

Write-Host ""
Write-Host ("Wrote bower package listing to: {0}." -f $newJsonFile)
