[CmdletBinding()]
param(
	[Parameter(Mandatory=$true)][string] $features,
	[Parameter(Mandatory=$true)][string] $connectedServiceName,
	[Parameter(Mandatory=$true)][string] $productName,
	[Parameter(Mandatory=$true)][string] $groupName,
	[Parameter(Mandatory=$true)][string] $version,
	[Parameter(Mandatory=$true)][string] $language,
	[Parameter(Mandatory=$true)][string] $augurkLocation,
	[Parameter(Mandatory=$true)][string] $treatWarningsAsErrors
)

# Make sure that we stop processing when an error occurs
$ErrorActionPreference = "Stop"

$treatWarningsAsErrorsBool = [System.Convert]::ToBoolean($treatWarningsAsErrors)

Write-Verbose "Entering script PublishFeaturesToAugurk.ps1"
Write-Verbose "Features = $features"
Write-Verbose "Connected Service Name = $connectedServiceName"
Write-Verbose "Product Name = $productName"
Write-Verbose "Group Name = $groupName"
Write-Verbose "Version = $version"
Write-Verbose "Language = $language"
Write-Verbose "Augurk Location = $augurkLocation"
Write-Verbose "Treat Warnings As Errors = $treatWarningsAsErrorsBool"
	
# Import the Task.Common dll that has all the cmdlets we need for Build
Import-Module "Microsoft.TeamFoundation.DistributedTask.Task.Common"
	
# Ensure that the augurk location has been properly set
if (!$augurkLocation.EndsWith("augurk.exe", "OrdinalIgnoreCase"))
{
	throw "augurk.exe location must end with 'augurk.exe'."
}

# Locate the augurk tool, it is part of the Augurk.CommandLine NuGet package
if ($augurkLocation.Contains("*") -or $augurkLocation.Contains("?"))
{
	Write-Verbose "Find-Files -SearchPattern $augurkLocation"
	$augurkExecutables = Find-Files -SearchPattern $augurkLocation
	Write-Verbose "augurkExecutables = $augurkExecutables"

	$augurk = $augurkExecutables | Select-Object -First 1
}
else
{
	if (Test-Path -Path $augurkLocation -Type Leaf)
	{
		$augurk = $augurkLocation
	}
}

if (!$augurk)
{
	$message = "Could not find augurk.exe. If you don't have Augurk command line tools installed, install the NuGet package Augurk.CommandLine."
	if ($treatWarningsAsErrors)
	{
		Write-Error $message
	}
	else
	{
		Write-Warning $message
		return
	} 
}

# Resolve the set of solutions we need to process
if ($features.Contains("*") -or $features.Contains("?")) {
	Write-Verbose "Pattern found in features parameter."
	Write-Verbose "Find-Files -SearchPattern $features"
	$featureFiles = Find-Files -SearchPattern $features
	Write-Verbose "featureFiles = $featureFiles"
} else {
	Write-Verbose "No pattern found in features parameter."
	$featureFiles = ,$features 
}

# Replace variables in the version parameter
while ($version -match "\$\((\w*\.\w*)\)") {
	$variableValue = Get-TaskVariable -Context $distributedTaskContext -Name $Matches[1]
	$version = $version.Replace($Matches[0], $variableValue)
	Write-Verbose "Substituting variable $($Matches[0]) with value $variableValue for parameter $version"
}

# Get the endpoint to Augurk
$connectedService = Get-ServiceEndpoint -Name "$connectedServiceName" -Context $distributedTaskContext
$augurkUri = $connectedService.Url

# Determine the command line arguments to pass to the tool
$arguments = @("publish", "--featureFiles=$($featureFiles -join ',')", "--productName=$productName", "--groupName=$groupName", "--version=$version", "--url=$augurkUri")

# Invoke the tool
Write-Output "& $augurk $arguments"
& $augurk $arguments
	
Write-Verbose "Leaving script PublishFeaturesToAugurk.ps1" 