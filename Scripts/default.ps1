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
  $mspec = "$lib_dir\Machine.Specifications.0.5.6.0\tools\mspec-x86-clr4.exe"
  $nsis = "C:\Program Files (x86)\NSIS\makensis.exe"
  $release_dir = "$base_dir\Release"
  $uploadCategory = "NServiceBus.Profiler"
  $uploadScript = "$base_dir\PublishBuild.build"
  $include_pdb_files = $false
  $cleanup_build_folder = $false
  $test_assembly_name = "NServiceBus.Profiler.Tests";
}

task default -depends Compile

task Clean { 
  remove-item -force -recurse $buildartifacts_dir -ErrorAction SilentlyContinue 
  remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue 
} 

task Init -depends Clean { 
	
	Generate-Assembly-Info `
		-file "$base_dir\NServiceBus.Profiler.Desktop\Properties\AssemblyInfo.cs" `
		-title "NServiceBus Profiler" `
		-description "NServiceBus Profiler" `
		-company "NServiceBus" `
		-product "NServiceBus Profiler v$version" `
		-version $version `
		-clsCompliant "false" `
		-copyright "Copyright © NServiceBus 2007-2011" `
		-supportEmail "h.eskandari@gmail.com" `
		-supportWeb "http://www.hightech.ir/Products/QueueManager"
		
	new-item $release_dir -itemType directory -ErrorAction SilentlyContinue 
	new-item $buildartifacts_dir -itemType directory 
} 

task Compile -depends Init { 
  exec { msbuild /p:OutDir=$buildartifacts_dir $sln_file  }
}

task CopyPlugins -depends Compile {
  Write-Host "Copying plugin files..."
  
  new-item $buildartifacts_dir\Plugins -itemType directory
  
  copy-item $buildartifacts_dir\NServiceBus.Profiler.HexViewer.* $buildartifacts_dir\Plugins
  copy-item $buildartifacts_dir\NServiceBus.Profiler.Bus.* $buildartifacts_dir\Plugins
  copy-item $buildartifacts_dir\NServiceBus.Profiler.XmlViewer.* $buildartifacts_dir\Plugins
  copy-item $buildartifacts_dir\NServiceBus.Profiler.JsonViewer.* $buildartifacts_dir\Plugins
  
  if($include_pdb_files -eq $false) {
     remove-item $buildartifacts_dir\*.pdb
	 remove-item $buildartifacts_dir\Plugins\*.pdb
  }
  
}

task Test -depends Compile, CopyPlugins {
  $old = pwd
  cd $build_dir
  
  exec { invoke-expression "$mspec $build_dir\$test_assembly_name.dll" }  
  
  cd $old
}

task Zip -depends Test {

	& $tools_dir\7z.exe a -tzip `
	  $release_dir\NSBProfiler-$humanReadableversion.zip `
      $build_dir\NServiceBus.Profiler.Desktop.exe `
      $build_dir\NServiceBus.Profiler.Desktop.exe.config `
      $build_dir\NServiceBus.Profiler.Core.* `
      $build_dir\NServiceBus.Profiler.Common.* `
      $build_dir\Plugins\ 
	  
	if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute ZIP command"
    }
	
	if ($cleanup_build_folder -eq $true) {
	    remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue 
	}
}

task Release -depends Test {
	Create-Setup `
		-setupScript "$base_dir\SolutionItems\Installer.nsi" `
		-nsis $nsis
		
	move-item $base_dir\SolutionItems\Install.exe $release_dir\NSBProfiler-$humanReadableversion.exe
	
    if ($cleanup_build_folder -eq $true) {
	    remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue 
	}
}

task Upload -depends Release {
	if (Test-Path $uploadScript ) {
		$log = git log -n 1 --oneline		
		msbuild $uploadScript /p:Category=$uploadCategory "/p:Comment=$log" "/p:File=$release_dir\NSBProfiler-$humanReadableversion-Build-$env:ccnetnumericlabel.zip"
		
		if ($lastExitCode -ne 0) {
			throw "Error: Failed to publish build"
		}
	}
	else {
		Write-Host "could not find upload script $uploadScript, skipping upload"
	}
}