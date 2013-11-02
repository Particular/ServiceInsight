include .\psake_extensions.ps1
include .\buildutils.ps1

properties {
  $base_dir  = resolve-path ..
  $lib_dir = "$base_dir\packages"
  $build_dir = "$base_dir\build" 
  $buildartifacts_dir = "$build_dir\" 
  $sln_file = "$base_dir\NServiceBus.Profiler.sln" 
  $version = "1.0.0.0"
  $humanReadableversion = Get-HumanReadable-Format
  $tools_dir = "$base_dir\Tools"
  $testrunner = "$lib_dir\NUnit.Runners.2.6.3\tools\nunit-console-x86.exe"
  $nsis = "C:\Program Files (x86)\NSIS\makensis.exe"
  $release_dir = "$base_dir\Release"
  $uploadCategory = "NServiceBus.Profiler"
  $uploadScript = "$base_dir\PublishBuild.build"
  $include_pdb_files = $false
  $cleanup_build_folder = $false
  $test_assembly_name = "Particular.ServiceInsight.Tests";
}

task default -depends Compile

task Clean { 
  remove-item -force -recurse $buildartifacts_dir -ErrorAction SilentlyContinue 
  remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue 
} 

task Init -depends Clean { 

    Write-Host "Humman readable product version is $humanReadableversion"
		
	Generate-Assembly-Info `
		-file "$base_dir\NServiceBus.Profiler.Desktop\Properties\AssemblyInfo.cs" `
		-title "ServiceInsight for NServiceBus" `
		-description "ServiceInsight for NServiceBus" `
		-company "Particular Software Incorporated" `
		-product "ServiceInsight for NServiceBus $humanReadableversion" `
		-version $version `
		-infoVersion "$humanReadableversion" `
		-clsCompliant "false" `
		-internalsVisibleTo $test_assembly_name `
		-copyright "Copyright © 2013 Particular Software Incorporated. All rights reserved" `
		-supportWeb "http://www.particular.net"
		
	new-item $release_dir -itemType directory -ErrorAction SilentlyContinue 
	new-item $buildartifacts_dir -itemType directory 
} 

task Compile -depends Init { 
  exec { msbuild /p:OutDir=$buildartifacts_dir $sln_file  }
}

task Test -depends Compile {
  $old = pwd
  cd $build_dir
  
  exec { invoke-expression "$testrunner $build_dir\$test_assembly_name.dll" }  
  
  cd $old
}

task Zip -depends Test {

	& $tools_dir\7z.exe a -tzip `
	  $release_dir\ServiceInsight-$humanReadableversion.zip `
      $build_dir\Particular.ServiceInsight.exe `
      $build_dir\Particular.ServiceInsight.pdb
	  
	if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute ZIP command"
    }
	
	if ($cleanup_build_folder -eq $true) {
	    remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue 
	}
}
