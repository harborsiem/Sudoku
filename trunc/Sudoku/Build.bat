Set AppName="Sudoku"

rem 64Bit Systems
rem Set MsBuildPath="%windir%\Microsoft.NET\Framework64\v4.0.30319\MsBuild.exe"
rem Set MsBuildPath="%ProgramFiles%\MsBuild\14.0\bin\amd64\MsBuild.exe"

rem 32Bit Systems
rem Set MsBuildPath="%windir%\Microsoft.NET\Framework\v4.0.30319\MsBuild.exe"
Set MsBuildPath="%ProgramFiles%\MsBuild\14.0\bin\MsBuild.exe"
Set Wix35Path="%ProgramFiles%\Windows Installer XML v3.5\bin
rem cd %AppName%
%MsBuildPath% %AppName%.sln /p:Configuration=Debug /p:Platform="Any CPU"
%MsBuildPath% %AppName%.sln /p:Configuration=Release /p:Platform="Any CPU"
rem cd ..
%Wix35Path%\candle.exe" -nologo %AppName%.wxs -out %AppName%.wixobj  -ext WixUIExtension  -ext WixNetFxExtension
%Wix35Path%\light.exe" -nologo %AppName%.wixobj -out %AppName%.msi  -ext WixUIExtension  -ext WixNetFxExtension
pause
