# Treat all errors as terminating
$ErrorActionPreference = "Stop"

$ToolsDir = $PSScriptRoot
$RepoRoot = "$ToolsDir\.."
$NuGetVersion = "4.1.0"
$NuGetCacheDir = "$($Env:LOCALAPPDATA)\.nuget\v$NuGetVersion"
$NuGetCacheFile = "$NuGetCacheDir\NuGet.exe"
$NuGetDir = "$ToolsDir\.nuget"
$NuGetExe = "$NuGetDir\NuGet.exe"
$PackagesDir = "$ToolsDir\packages"
$PackagesConfig = "$ToolsDir\packages.config"
$PackagesHashFile = "$ToolsDir\packages.config.sha1"
$CsiExe = "$PackagesDir\Microsoft.Net.Compilers\tools\csi.exe"

function EnsureDirectoryExists($path)
{
    if (!(Test-Path $path -PathType Container)) {
        New-Item -ItemType Directory  $path | Out-Null
    }
}

function DownloadNuGetToCache()
{
    $NuGetUrl = "https://dist.nuget.org/win-x86-commandline/v$NuGetVersion/NuGet.exe"
    if (!(Test-Path $NuGetCacheFile)) {
        EnsureDirectoryExists($NuGetCacheDir)
        Write-Host "Downloading '$NuGetUrl' to '$NuGetCacheFile'..."
        Invoke-WebRequest $NuGetUrl -OutFile $NuGetCacheFile
    }
}

function CopyNuGetLocally()
{
    EnsureDirectoryExists($NuGetDir)
    Copy-Item $NuGetCacheFile $NuGetExe
}

function ReadPreviousPackagesHash()
{
    if (Test-Path $PackagesHashFile)
    {
        (Get-Content $PackagesHashFile).Trim()
    }
    else
    {
        ""
    }
}

function WritePackagesHash($hash)
{
    Set-Content $PackagesHashFile $hash
}

function ComputePackagesHash()
{
    (Get-FileHash -Algorithm SHA1 -Path $PackagesConfig).Hash
}

function ClearPackagesDir()
{
    if (Test-Path -Path $PackagesDir -PathType Container)
    {
        Write-Host "$PackagesConfig has changed, clearing packages to force reinstall..."
        Remove-Item -Recurse -Force $PackagesDir
    }
}

function ClearPackagesIfChanged()
{
    # check if packages.config have changed
    $PreviousPackagesHash = ReadPreviousPackagesHash
    $PackagesHash = ComputePackagesHash
    if ($PackagesHash -ne $PreviousPackagesHash) {
        # if packages.config has changed, clear packages and store new hash
        ClearPackagesDir
        WritePackagesHash($PackagesHash)
    }
}

function RestorePackages()
{
    & $NuGetExe install $PackagesConfig -OutputDirectory $PackagesDir -Verbosity quiet -ExcludeVersion
}

# change working directory to the repository root
Push-Location $RepoRoot

DownloadNuGetToCache
CopyNuGetLocally
ClearPackagesIfChanged
RestorePackages

# run build script
& $CsiExe $args

# return to original working directory
Pop-Location
