properties {
	$ProductVersion = "1.0"
	$PatchVersion = "0"
	$BuildNumber = if($env:BUILD_NUMBER -ne $null) { $env:BUILD_NUMBER } else { "0" }
	$PreRelease = ""
}

$baseDir = Split-Path (Resolve-Path $MyInvocation.MyCommand.Path)
$toolsDir = "$baseDir\Scripts"
$setupProjectFile = "$baseDir\Setup\Profiler.aip"
$setupModuleOutPutDir = "$baseDir\Setup\Output Package"

include $toolsDir\buildutils.ps1

task default -depends Init, BuildSetup

task Init {

    # Install path for Advanced Installer
    $AdvancedInstallerPath = ""
    $AdvancedInstallerPath = Get-RegistryValue "HKLM:\SOFTWARE\Wow6432Node\Caphyon\Advanced Installer\" "Advanced Installer Path" 
    $script:AdvinstCLI = $AdvancedInstallerPath + "\bin\x86\AdvancedInstaller.com"
    
}

task BuildSetup {  
    
	if($PreRelease -eq "") {
		$archive = "Profiler.$ProductVersion.$PatchVersion" 
	} else {
		$archive = "Profiler.$ProductVersion.$PatchVersion-$PreRelease$BuildNumber"
	}

	# edit Advanced Installer Project	  
	exec { &$script:AdvinstCLI /edit $setupProjectFile /SetVersion "$ProductVersion.$PatchVersion" -noprodcode }	
	exec { &$script:AdvinstCLI /edit $setupProjectFile /SetPackageName "$archive.exe" -buildname DefaultBuild }
	
	# Build setup with Advanced Installer	
	exec { &$script:AdvinstCLI /rebuild $setupProjectFile }
}

