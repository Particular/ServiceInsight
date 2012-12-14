; Setting compressor
SetCompressor lzma

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "NSB Profiler"
!define PROCESS_NAME "NServiceBus.Profiler"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_PUBLISHER "NServiceBus"
!define PRODUCT_WEB_SITE "http://www.hightech.ir/Products/QueueManager"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\${PRODUCT_NAME}.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; Plugins
!addplugindir "..\Tools\NSIS\";

; MUI 1.67 compatible ------
!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "WordFunc.nsh"

;Request application privileges for Windows Vista
RequestExecutionLevel admin

!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "InstallerHeader.bmp"
!define MUI_ABORTWARNING
!define MUI_ICON "Installer.ico"
!define MUI_UNICON "Installer.ico"
!define MUI_FINISHPAGE_RUN "$INSTDIR\NServiceBus.Profiler.Desktop.exe"

;Framework version
!define MIN_FRA_MAJOR "4"
!define MIN_FRA_MINOR "0"
!define MIN_FRA_BUILD "*"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "Agreement.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro VersionCompare
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "Install.exe"
InstallDir "$PROGRAMFILES\NSB Profiler"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

Section -CopyApplicationFiles
  SetOutPath "$INSTDIR"
  SetOverwrite off
  
  #Create folders
  CreateDirectory "$INSTDIR\Plugins"
  CreateDirectory "$INSTDIR\Logs"
 
  #copy executables
  File "..\build\NServiceBus.Profiler.Desktop.exe"
  File "..\build\NServiceBus.Profiler.Desktop.exe.config"
  File "..\build\log4net.config"
  File "..\build\NServiceBus.Profiler.Common.dll"
  File "..\build\NServiceBus.Profiler.Core.dll"
  
  SetOutPath "$INSTDIR\Plugins"  
  File "..\build\Plugins\NServiceBus.Profiler.Bus.dll"
  File "..\build\Plugins\NServiceBus.Profiler.HexViewer.dll"
  File "..\build\Plugins\NServiceBus.Profiler.XmlViewer.dll"
  
  #copy text files and documentations
  SetOutPath "$INSTDIR"  
  File "Agreement.txt"
  
  #copy installer files
  File "Web.ico"

  CreateDirectory "$SMPROGRAMS\NServiceBus Profiler"
  CreateShortCut "$SMPROGRAMS\NServiceBus Profiler\NSB Profiler.lnk" "$INSTDIR\NServiceBus.Profiler.Desktop.exe"
  CreateShortCut "$DESKTOP\NSB Profiler.lnk" "$INSTDIR\NServiceBus.Profiler.Desktop.exe"
SectionEnd

Section -AdditionalIcons
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\NServiceBus Profiler\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url" "" "$INSTDIR\Web.ico"
  CreateShortCut "$SMPROGRAMS\NServiceBus Profiler\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\NServiceBus.Profiler.Desktop.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\NServiceBus.Profiler.Desktop.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

Section Uninstall
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\*.exe"
  Delete "$INSTDIR\*.dll"
  Delete "$INSTDIR\*.txt"
  Delete "$INSTDIR\*.config"
  Delete "$INSTDIR\*.ico"
  Delete "$INSTDIR\Plugins\*.*"
  Delete "$INSTDIR\Logs\*.*"

  Delete "$SMPROGRAMS\NServiceBus Profiler\Uninstall.lnk"
  Delete "$SMPROGRAMS\NServiceBus Profiler\Website.lnk"
  Delete "$DESKTOP\NSB Profiler.lnk"
  Delete "$SMPROGRAMS\NServiceBus Profiler\NSB Profiler.lnk"

  RMDir "$SMPROGRAMS\NServiceBus Profiler"
  RMDir "$INSTDIR\Plugins"
  RMDir "$INSTDIR\Logs"
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

Function .onInit
  Call UninstallExistingVersion
  Call CheckDotNetPrerequisite
  Call CheckNotRunning
FunctionEnd

Function UninstallExistingVersion

  ReadRegStr $R0 HKLM \
  "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
  "UninstallString"
  StrCmp $R0 "" done
 
  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION \
  "${PRODUCT_NAME} is already installed. $\n$\nClick OK to remove the \
  previous version or Cancel to cancel the installation." \
  IDOK uninst
  Abort
 
;Run the uninstaller
uninst:
  ClearErrors
  ExecWait '$R0 _?=$INSTDIR' ;Do not copy the uninstaller to a temp file
 
  IfErrors no_remove_uninstaller done
    ;You can either use Delete /REBOOTOK in the uninstaller or add some code
    ;here to remove the uninstaller. Use a registry key to check
    ;whether the user has chosen to uninstall. If you are using an uninstaller
    ;components page, make sure all sections are uninstalled.
no_remove_uninstaller:
  
 
done:
 
FunctionEnd

;Checking if the process is running and quit
Function CheckNotRunning
  Processes::FindProcess ${PROCESS_NAME}
  StrCmp $R0 "1" AbortInstallation ContinueInstallation
  
  AbortInstallation:
  Quit
  
  ContinueInstallation:
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

;Checking if the process is running and ask user to quit
Function un.onInit
  Processes::FindProcess ${PROCESS_NAME}
  StrCmp $R0 "1" ExitUninstall ContinueUninstall

  ExitUninstall:
  MessageBox MB_ICONINFORMATION|MB_OK "Application is running. Close the application and retry."
  Quit

  ContinueUninstall:
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Function CheckDotNetPrerequisite
 
  ;Save the variables in case something else is using them
  Push $0
  Push $1
  Push $2
  Push $3
  Push $4
  Push $R1
  Push $R2
  Push $R3
  Push $R4
  Push $R5
  Push $R6
  Push $R7
  Push $R8
 
  StrCpy $R5 "0"
  StrCpy $R6 "0"
  StrCpy $R7 "0"
  StrCpy $R8 "0.0.0"
  StrCpy $0 0
 
  loop:
 
  ;Get each sub key under "SOFTWARE\Microsoft\NET Framework Setup\NDP"
  EnumRegKey $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP" $0
  StrCmp $1 "" done ;jump to end if no more registry keys
  IntOp $0 $0 + 1
  StrCpy $2 $1 1 ;Cut off the first character
  StrCpy $3 $1 "" 1 ;Remainder of string
 
  ;Loop if first character is not a 'v'
  StrCmpS $2 "v" start_parse loop
 
  ;Parse the string
  start_parse:
  StrCpy $R1 ""
  StrCpy $R2 ""
  StrCpy $R3 ""
  StrCpy $R4 $3
 
  StrCpy $4 1
 
  parse:
  StrCmp $3 "" parse_done ;If string is empty, we are finished
  StrCpy $2 $3 1 ;Cut off the first character
  StrCpy $3 $3 "" 1 ;Remainder of string
  StrCmp $2 "." is_dot not_dot ;Move to next part if it's a dot
 
  is_dot:
  IntOp $4 $4 + 1 ; Move to the next section
  goto parse ;Carry on parsing
 
  not_dot:
  IntCmp $4 1 major_ver
  IntCmp $4 2 minor_ver
  IntCmp $4 3 build_ver
  IntCmp $4 4 parse_done
 
  major_ver:
  StrCpy $R1 $R1$2
  goto parse ;Carry on parsing
 
  minor_ver:
  StrCpy $R2 $R2$2
  goto parse ;Carry on parsing
 
  build_ver:
  StrCpy $R3 $R3$2
  goto parse ;Carry on parsing
 
  parse_done:
 
  IntCmp $R1 $R5 this_major_same loop this_major_more
  this_major_more:
  StrCpy $R5 $R1
  StrCpy $R6 $R2
  StrCpy $R7 $R3
  StrCpy $R8 $R4
 
  goto loop
 
  this_major_same:
  IntCmp $R2 $R6 this_minor_same loop this_minor_more
  this_minor_more:
  StrCpy $R6 $R2
  StrCpy $R7 R3
  StrCpy $R8 $R4
  goto loop
 
  this_minor_same:
  IntCmp R3 $R7 loop loop this_build_more
  this_build_more:
  StrCpy $R7 $R3
  StrCpy $R8 $R4
  goto loop
 
  done:
 
  ;Have we got the framework we need?
  IntCmp $R5 ${MIN_FRA_MAJOR} max_major_same fail end
  max_major_same:
  IntCmp $R6 ${MIN_FRA_MINOR} max_minor_same fail end
  max_minor_same:
  IntCmp $R7 ${MIN_FRA_BUILD} end fail end
 
  fail:
  StrCmp $R8 "0.0.0" no_framework
  goto wrong_framework
 
  no_framework:
  MessageBox MB_OK|MB_ICONINFORMATION "Required .NET Framework runtime was not found.$\n$\n\
         This software requires Windows Framework version \
         ${MIN_FRA_MAJOR}.${MIN_FRA_MINOR}.${MIN_FRA_BUILD} or higher.$\n$\n\
         Make sure you install it before running the application."
  goto install_dotnet
 
  wrong_framework:
  MessageBox MB_OK|MB_ICONINFORMATION "Required .NET Framework runtime was not found.$\n$\n\
         This software requires Windows Framework version \
         ${MIN_FRA_MAJOR}.${MIN_FRA_MINOR}.${MIN_FRA_BUILD} or higher.$\n$\n\
         Suitable version will be installed as a part of this setup."
  goto install_dotnet
 
  install_dotnet:
  IfFileExists "..\Prerequisite\dotnetfx.exe" 0 +2
  MessageBox MB_OK|MB_ICONINFORMATION "Prerequisite folder was not found."
  ;ExecWait '"$TEMP\dotnetfx.exe" /q /c:"install /q"' 
 
  end:
 
  ;Pop the variables we pushed earlier
  Pop $R8
  Pop $R7
  Pop $R6
  Pop $R5
  Pop $R4
  Pop $R3
  Pop $R2
  Pop $R1
  Pop $4
  Pop $3
  Pop $2
  Pop $1
  Pop $0
 
FunctionEnd

