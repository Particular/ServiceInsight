function Get-RegistryValue($key, $value) {
    (Get-ItemProperty $key $value -ErrorAction SilentlyContinue).$value
}

#$baseDir = "%teamcity.build.checkoutDir%"
#$version = "%GitVersion.MajorMinorPatch%"
#$packageVersion = "%GitVersion.ClassicVersionWithTag%"

Function CreateInstaller
{
    param(
        [parameter(Position=0,Mandatory=1)] [string]$baseDir,
        [parameter(Position=1,Mandatory=1)] [string]$version,
        [parameter(Position=2,Mandatory=1)] [string]$packageVersion
    )

    #until we figure out why AI looks in the wrong dir
    Copy-Item .\binaries\* .\src\setup\binaries

    $AdvancedInstallerPath = Get-RegistryValue "HKLM:\SOFTWARE\Wow6432Node\Caphyon\Advanced Installer\" "Advanced Installer Path" 

    $script:AdvinstCLI = $AdvancedInstallerPath + "bin\x86\AdvancedInstaller.com"

    $setupProjectFile = "$baseDir\src\Setup\ServiceInsight.aip"

    $packageName = "Particular.ServiceInsight-$packageVersion.exe"

    # edit Advanced Installer Project   
    &$script:AdvinstCLI /edit $setupProjectFile /SetVersion $version
    &$script:AdvinstCLI /edit $setupProjectFile /SetPackageName $packageName -buildname DefaultBuild
        
    # Build setup with Advanced Installer 
    &$script:AdvinstCLI /rebuild $setupProjectFile
}

CreateInstaller