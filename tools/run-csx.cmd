@echo Off

setlocal

set TOOLS_DIR=%~dp0

rem change working directory to the repository root
pushd %TOOLS_DIR%\..

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
if not exist %TOOLS_DIR%\.nuget md %TOOLS_DIR%\.nuget
copy /Y %NUGET_CACHE_DIR%\NuGet.exe %TOOLS_DIR%\.nuget\NuGet.exe > nul

rem restore packages for build script
%TOOLS_DIR%\.nuget\NuGet.exe restore %TOOLS_DIR%\packages.config -PackagesDirectory %TOOLS_DIR%\packages -Verbosity quiet

rem run build script
"%TOOLS_DIR%\packages\Microsoft.Net.Compilers.2.2.0\tools\csi.exe" %*

rem return to original working directory
popd
