@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

REM Build
"%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" Gsof.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
"%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" Gsof.sln /p:Configuration="%config%"-Net45 /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM Package
mkdir Build
call nuget pack "Gsof.Xaml\Gsof.Xaml.csproj" -o Build %version%