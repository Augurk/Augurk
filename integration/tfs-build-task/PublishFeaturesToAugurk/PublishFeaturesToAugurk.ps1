[CmdletBinding()]
param(
	[Parameter(Mandatory=$true)][string] $features,
	[Parameter(Mandatory=$true)][string] $connectedServiceName,
	[Parameter(Mandatory=$true)][string] $branchName,
	[Parameter(Mandatory=$true)][string] $groupName,
	[Parameter(Mandatory=$true)][string] $clearGroup,
	[Parameter(Mandatory=$true)][string] $language
)

$clearGroupBool = [System.Convert]::ToBoolean($clearGroup)

Write-Verbose "Entering script PublishFeaturesToAugurk.ps1"
Write-Verbose "Features = $features"
Write-Verbose "Connected Service Name = $connectedServiceName"
Write-Verbose "Branch Name = $branchName"
Write-Verbose "Group Name = $groupName"
Write-Verbose "Clear Group = $clearGroupBool"
Write-Verbose "Language = $language"
	
# Import the Task.Common dll that has all the cmdlets we need for Build
Import-Module "Microsoft.TeamFoundation.DistributedTask.Task.Common"

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
$augurkExe = "$PSScriptRoot\tool\augurk.exe"
$arguments = @("publish", "--featureFiles=$($featureFiles -join ',')", "--branchName=$branchName", "--groupName=$groupName", "--url=$augurkUri")
if ($clearGroup) {
	$arguments += @("--clearGroup")	
}

# Invoke the tool
Write-Output "& $augurkExe $arguments"
& $augurkExe $arguments
	
Write-Verbose "Leaving script PublishFeaturesToAugurk.ps1" 