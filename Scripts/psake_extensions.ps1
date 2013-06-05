function Commit-Hash
{
	Write-Host "Getting commit hash..."
	$Hash = $env:build_vcs_number
	
	if(!$Hash)
	{
		Return Get-Git-Commit
	}
	else
	{
		Return $Hash.substring(0, 7)
	}
}

function Get-Git-Commit
{
	$gitLog = git log --oneline -1
	return $gitLog.Split(' ')[0]
}

function Create-Setup
{
param(
	[string]$setupScript = $(throw "path to NSIS script is required"),
	[string]$nsis = $(throw "installation path to NSIS is required")
)

	Write-Host "Creating setup file using script at $setupScript"
	& $nsis $setupScript
}

