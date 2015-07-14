[CmdletBinding()]
param(
	[Parameter(Mandatory=$true)][string] $features,
	[Parameter(Mandatory=$true)][string] $connectedServiceName,
	[Parameter(Mandatory=$true)][string] $branchName,
	[Parameter(Mandatory=$true)][string] $groupName,
	[Parameter(Mandatory=$true)][string] $clearGroup,
	[Parameter(Mandatory=$true)][string] $language,
	[Parameter(Mandatory=$true)][string] $augurkLocation,
	[Parameter(Mandatory=$true)][string] $treatWarningsAsErrors
)

# Make sure that we stop processing when an error occurs
$ErrorActionPreference = Stop

$clearGroupBool = [System.Convert]::ToBoolean($clearGroup)
$treatWarningsAsErrorsBool = [System.Convert]::ToBoolean($treatWarningsAsErrors)

Write-Verbose "Entering script PublishFeaturesToAugurk.ps1"
Write-Verbose "Features = $features"
Write-Verbose "Connected Service Name = $connectedServiceName"
Write-Verbose "Branch Name = $branchName"
Write-Verbose "Group Name = $groupName"
Write-Verbose "Clear Group = $clearGroupBool"
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
	$message = "Could not find augurk.exe. If you don't have Augurk cCommand line tools installed, install the NuGet package Augurk.CommandLine."
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

# Get the endpoint to Augurk
$connectedService = Get-ServiceEndpoint -Name "$connectedServiceName" -Context $distributedTaskContext
$augurkUri = $connectedService.Url

# Make sure that the branch name doesn't contain a slash
if ($branchName.Contains("/")) {
	# And if it does, just use the last section
	$branchName = Split-Path $branchName -Leaf
}

# Determine the command line arguments to pass to the tool
$arguments = @("publish", "--featureFiles=$($featureFiles -join ',')", "--branchName=$branchName", "--groupName=$groupName", "--url=$augurkUri")
if ($clearGroup) {
	$arguments += @("--clearGroup")	
}

# Invoke the tool
Write-Output "& $augurk $arguments"
& $augurk $arguments
	
Write-Verbose "Leaving script PublishFeaturesToAugurk.ps1" 