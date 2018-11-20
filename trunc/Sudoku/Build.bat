Set AppName="Sudoku"

if not defined ProgramFiles(x86) goto x86
rem 64Bit Systems
  Set X86ProgramFiles=%ProgramFiles(x86)%
rem Set MsBuildPath="%windir%\Microsoft.NET\Framework\v4.0.30319\MsBuild.exe"
Set MsBuildPath="%X86ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MsBuild.exe"
rem only 64 bit projects  Set MsBuildPath="%X86ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\amd64\MsBuild.exe"
rem Set MsBuildPath="%windir%\Microsoft.NET\Framework64\v4.0.30319\MsBuild.exe"
  goto x86End

:x86
rem 32Bit Systems
  Set X86ProgramFiles=%ProgramFiles%
rem Set MsBuildPath="%windir%\Microsoft.NET\Framework\v4.0.30319\MsBuild.exe"
Set MsBuildPath="%ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MsBuild.exe"
:x86End

Set Wix3xPath=%X86ProgramFiles%\WiX Toolset v3.11\bin
rem cd %AppName%
%MsBuildPath% %AppName%.sln /p:Configuration=Debug /p:Platform="Any CPU"
%MsBuildPath% %AppName%.sln /p:Configuration=Release /p:Platform="Any CPU"
rem cd ..
"%Wix3xPath%\candle.exe" -nologo %AppName%.wxs -out %AppName%.wixobj  -ext WixUIExtension  -ext WixNetFxExtension
"%Wix3xPath%\light.exe" -nologo %AppName%.wixobj -out %AppName%.msi  -ext WixUIExtension  -ext WixNetFxExtension
pause
