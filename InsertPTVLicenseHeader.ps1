<#
Before running the script close the solution in VIsual Studio because otherwise some of the files can be locked and the adding of header fails
To execute the script run it from command prompt (from the same folder where the script is) using command: powershell -ExecutionPolicy ByPass -File .\InsertPTVLicenseHeader.ps1
#>
# read the license header file and append the comment chars to the resulting header string
$mitLicenseHeader = "/**`r`n{0} */" -f (Get-Content ".\LICENSE" | %{" * $_"} | Out-String)

function AddCsharpHeader([string]$licenseHeader)
{	
	#Filter files getting only .cs ones and exclude specific file extensions
	#the result needs to be piped to be able to remove anything under any obj folder or anything below the IdentityServer4 folder as there is no our code
	# -EA SilentlyCOntinue is used to hide the error message that will come from web projects node_modules subfolders as there too long paths
	$filteredFiles = Get-ChildItem .\src, .\test -Filter *.cs -Exclude *.Designer.cs,T4MVC.cs -Recurse -EA SilentlyContinue | ?{ $_.fullname -notmatch "\\IdentityServer4\\?" -and $_.fullname -notmatch "\\obj\\?" }
	
	Write-Output ("C# files count: {0}." -f $filteredFiles.count)
	
	$addedFiles = 0
	$skippedFiles = 0
	
	foreach ($originalFile in $filteredFiles)
	{
		# get the existing source files content
		$fileContent = (Get-Content $originalFile)
		
		#check if the 1st line has comment tag '/*' and then check if the 2nd line contains ' MIT ' => we already have the license header added
		if ($fileContent.length -gt 1 -and $fileContent[0] -eq "/**" -and $fileContent[1] -match " MIT ")
		{
			Write-Output ("Skipping file '{0}', license header already added." -f $originalFile.FullName)
			$skippedFiles++
			continue
		}
		
		#write header to file
		Set-Content $originalFile $licenseHeader -encoding UTF8
		
		#write the file original content
		Add-Content $originalFile $fileContent
		
		$addedFiles++
	}
	
	Write-Output "Added header to $addedFiles file(s) and skipped $skippedFiles file(s)."
}

function AddJavaScriptHeader([string]$licenseHeader)
{
	#Filter files getting only .js and .jsx files
	# -Include used as -Filter takes only one string. According to docs the -Invole should correctly match to the filename instead of path because we are using -Recurse
	$filteredFiles = Get-ChildItem .\src\PTV.Application.Web\Scripts, .\src\PTV.Application.Web\wwwroot\js\App -Include *.js,*.jsx -Recurse
	
	Write-Output ("JS and JSX files count: {0}." -f $filteredFiles.count)
	
	$addedFiles = 0
	$skippedFiles = 0
	 
	foreach ($originalFile in $filteredFiles)
	{
		# get the existing source files content
		$fileContent = (Get-Content $originalFile)
		
		#check if the 1st line has comment tag '/*' and then check if the 2nd line contains ' MIT ' => we already have the license header added
		if ($fileContent.length -gt 1 -and $fileContent[0] -eq "/**" -and $fileContent[1] -match " MIT ")
		{
			Write-Output ("Skipping file '{0}', license header already added." -f $originalFile.FullName)
			$skippedFiles++
			continue
		}
		
		# try to get the existing source files encoding
		[byte[]]$encodingBytes = (Get-Content -Encoding byte -ReadCount 4 -TotalCount 4 -Path $originalFile.FullName)
		
		$useEncoding = $null
		
		if ($encodingBytes -ne $null)
		{	
			if ( $encodingBytes[0] -eq 0xef -and $encodingBytes[1] -eq 0xbb -and $encodingBytes[2] -eq 0xbf ) {
				$useEncoding = "UTF8" 
			}
			#for now ignore all other encodings, we have source files in UTF8 and in western european code page
<#			elseif ($encodingBytes[0] -eq 0xfe -and $encodingBytes[1] -eq 0xff) {
				Write-Output 'Encoding: Unicode'
			} elseif ($encodingBytes[0] -eq 0 -and $encodingBytes[1] -eq 0 -and $encodingBytes[2] -eq 0xfe -and $encodingBytes[3] -eq 0xff) {
				Write-Output 'Encoding: UTF32'
			} elseif ($encodingBytes[0] -eq 0x2b -and $encodingBytes[1] -eq 0x2f -and $encodingBytes[2] -eq 0x76) {
				Write-Output 'Encoding: UTF7'
			} else {
				Write-Output 'Encoding: ASCII'
			}
			#>
		}
		
		#write header to file, use console encoding if no BOM was in the file otherwise use UTF8 (currently only that is checked in the previous code
		if ($useEncoding)
		{
			Set-Content $originalFile $licenseHeader -encoding $useEncoding
		}
		else
		{
			Set-Content $originalFile $licenseHeader
		}
		
		#write the file original content
		Add-Content $originalFile $fileContent
		
		$addedFiles++
	}
	
	Write-Output "Added header to $addedFiles file(s) and skipped $skippedFiles file(s)."
}

AddCsharpHeader $mitLicenseHeader

AddJavaScriptHeader $mitLicenseHeader