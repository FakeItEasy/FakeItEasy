@echo Off

rem change working directory to the repository root
pushd %~dp0
setlocal

rem options
set NUGET_VERSION=4.1.0

rem determine nuget cache dir
set NUGET_CACHE_DIR=%LocalAppData%\.nuget\v%NUGET_VERSION%

rem download nuget to cache dir
set NUGET_URL=https://dist.nuget.org/win-x86-commandline/v%NUGET_VERSION%/NuGet.exe
if not exist %NUGET_CACHE_DIR%\NuGet.exe (
  if not exist %NUGET_CACHE_DIR% md %NUGET_CACHE_DIR%
  echo Downloading '%NUGET_URL%'' to '%NUGET_CACHE_DIR%\NuGet.exe'...
  @powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest '%NUGET_URL%' -OutFile '%NUGET_CACHE_DIR%\NuGet.exe'"
)

rem copy nuget locally
if not exist .nuget md .nuget
copy /Y %NUGET_CACHE_DIR%\NuGet.exe .nuget\NuGet.exe > nul

rem restore packages for build script
.nuget\NuGet.exe restore .\packages.config -PackagesDirectory .\packages -Verbosity quiet

rem run build script
".\packages\Microsoft.Net.Compilers.2.2.0\tools\csi.exe" .\build.csx %*

rem return to original working directory
popd
