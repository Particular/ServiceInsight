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

function Generate-Assembly-Info
{
param(
	[string]$clsCompliant = "true",
	[string]$title, 
	[string]$description, 
	[string]$company, 
	[string]$product, 
	[string]$copyright, 
	[string]$version,
	[string]$file = $(throw "file is a required parameter."),
	[string]$supportWeb = $(throw "file is a required parameter.")
)
  $asmInfo = "using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ExceptionHandler;

[assembly: CLSCompliant($clsCompliant)]
[assembly: ComVisible(false)]
[assembly: AssemblyTitle(""$title"")]
[assembly: AssemblyDescription(""$description"")]
[assembly: AssemblyCompany(""$company"")]
[assembly: AssemblyProduct(""$product"")]
[assembly: AssemblyCopyright(""$copyright"")]
[assembly: AssemblyVersion(""$version"")]
[assembly: AssemblyInformationalVersion(""$version"")]
[assembly: AssemblyFileVersion(""$version"")]
[assembly: AssemblyDelaySign(false)]
[assembly: SupportWebUrl(""$supportWeb"")]
[assembly: InternalsVisibleTo(""$test_assembly_name"")]
"

	$dir = [System.IO.Path]::GetDirectoryName($file)
	if ([System.IO.Directory]::Exists($dir) -eq $false)
	{
		Write-Host "Creating directory $dir"
		[System.IO.Directory]::CreateDirectory($dir)
	}
	Write-Host "Generating assembly info file: $file"
	Write-Output $asmInfo > $file
}