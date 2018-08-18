$ToolsDir = $PSScriptRoot

# change working directory to the repository root
Push-Location $ToolsDir\..

$NuGetVersion = "4.1.0"

# determine nuget cache dir
$NuGetCacheDir = "$($Env:LOCALAPPDATA)\.nuget\v$NuGetVersion"
$NuGetCacheFile = "$NuGetCacheDir\NuGet.exe"

# download nuget to cache dir
$NuGetUrl = "https://dist.nuget.org/win-x86-commandline/v$NuGetVersion/NuGet.exe"
if (!(Test-Path $NuGetCacheFile)) {
    if (!(Test-Path $NuGetCacheDir)) {
        New-Item -ItemType Directory $NuGetCacheDir | Out-Null
    }
    Write-Host "Downloading '$NuGetUrl' to '$NuGetCacheFile'..."
    Invoke-WebRequest $NuGetUrl -OutFile $NuGetCacheFile
}

# copy nuget locally
$NuGetDir = "$ToolsDir\.nuget"
$NuGetExe = "$NuGetDir\NuGet.exe"
if (!(Test-Path $NuGetDir)) {
    New-Item -ItemType Directory $NuGetDir | Out-Null
}
Copy-Item $NuGetCacheFile $NuGetExe

# restore packages for build script
$PackagesConfig = "$ToolsDir\packages.config"
$PackagesDir = "$ToolsDir\packages"
& $NuGetExe install $PackagesConfig -OutputDirectory $PackagesDir -Verbosity quiet -ExcludeVersion

# run build script
$CsiExe = "$PackagesDir\Microsoft.Net.Compilers\tools\csi.exe"
& $CsiExe $args

# return to original working directory
Pop-Location
