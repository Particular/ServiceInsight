include .\psake_extensions.ps1

properties {
  $base_dir  = resolve-path ..
  $lib_dir = "$base_dir\packages"
  $build_dir = "$base_dir\build" 
  $buildartifacts_dir = "$build_dir\" 
  $sln_file = "$base_dir\NServiceBus.Profiler.sln" 
  $version = "1.0.0.0"
  $humanReadableversion = "v1.0-BETA"
  $tools_dir = "$base_dir\Tools"
  $mspec = "$lib_dir\Machine.Specifications.0.5.11\tools\mspec-x86-clr4.exe"
  $nsis = "C:\Program Files (x86)\NSIS\makensis.exe"
  $release_dir = "$base_dir\Release"
  $uploadCategory = "NServiceBus.Profiler"
  $uploadScript = "$base_dir\PublishBuild.build"
  $include_pdb_files = $false
  $cleanup_build_folder = $false
  $test_assembly_name = "NServiceBus.ServiceInsight.Tests";
}

task default -depends Compile

task Clean { 
  remove-item -force -recurse $buildartifacts_dir -ErrorAction SilentlyContinue 
  remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue 
} 

task Init -depends Clean { 
	
	$commit = Commit-Hash
	Write-Host "Commit hash is $comit"
	
	Generate-Assembly-Info `
		-file "$base_dir\NServiceBus.Profiler.Desktop\Properties\AssemblyInfo.cs" `
		-title "ServiceInsight for NServicebus" `
		-description "ServiceInsight for NServicebus" `
		-company "NServiceBus" `
		-product "ServiceInsight for NServicebus $humanReadableversion ($commit)" `
		-version $version `
		-clsCompliant "false" `
		-copyright "Copyright 2010-2013 NServiceBus. All rights reserved" `
		-supportEmail "h.eskandari@gmail.com" `
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
  
  exec { invoke-expression "$mspec $build_dir\$test_assembly_name.dll" }  
  
  cd $old
}

task Zip -depends Test {

	& $tools_dir\7z.exe a -tzip `
	  $release_dir\ServiceInsight-$humanReadableversion.zip `
      $build_dir\NServiceBus.ServiceInsight.Desktop.exe `
      $build_dir\NServiceBus.ServiceInsight.Desktop.exe.config `
      $build_dir\NServiceBus.ServiceInsight.Core.* `
      $build_dir\NServiceBus.ServiceInsight.Common.*
	  
	if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute ZIP command"
    }
	
	if ($cleanup_build_folder -eq $true) {
	    remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue 
	}
}
