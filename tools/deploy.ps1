$ErrorActionPreference = "Stop"

Push-Location (Get-Item $PSScriptRoot).Parent.FullName

try {
    if (! $env:APPVEYOR_REPO_TAG_NAME) {
        Write-Output "No Appveyor tag name supplied. Not deploying."
        return
    }

    $releaseName = $env:APPVEYOR_REPO_TAG_NAME
    $gitHubAuthToken = $env:GITHUB_TOKEN
    $nugetApiKey = $env:NUGET_API_KEY
    $repo = $env:APPVEYOR_REPO_NAME

    $nugetServer = "https://www.nuget.org/api/v2/package"
    $artifactsPattern = "artifacts/output/*.nupkg"
    $releasesUrl = "https://api.github.com/repos/$repo/releases"
    $headers = @{
        "Authorization" = "Bearer $gitHubAuthToken"
        "Content-type"  = "application/json"
    }

    Write-Output "Deploying $releaseName"
    Write-Output "Looking for GitHub release $releaseName"

    $releases = Invoke-RestMethod -Uri $releasesUrl -Headers $headers -Method GET
    $release = $releases | Where-Object { $_.name -eq $releaseName }
    if (! $release) {
        throw "Can't find release $releaseName"
    }

    $headers["Content-type"] = "application/octet-stream"
    $uploadsUrl = "https://uploads.github.com/repos/$repo/releases/$($release.id)/assets?name="

    Write-Output "Uploading artifacts to GitHub release"

    $artifacts = Get-ChildItem -Path $artifactsPattern
    if (! $artifacts) {
        throw "Can't find any artifacts to publish"
    }

    $artifacts | ForEach-Object {
        Write-Output "Uploading $($_.Name)"
        $asset = Invoke-RestMethod -Uri ($uploadsUrl + $_.Name) -Headers $headers -Method POST -InFile $_
        Write-Output "Uploaded  $($asset.name)"
    }

    Write-Output "Pushing nupkgs to nuget.org"
    $artifacts | ForEach-Object {
        Write-Output "Pushing $($_.Name)"
        $nugetPath = Join-Path $PSScriptRoot .nuget/nuget.exe
        & $nugetPath push $_ -ApiKey $nugetApiKey -Source $nugetServer -NonInteractive -ForceEnglishOutput
        if ($LASTEXITCODE -ne 0) {
            throw "Push failed with error $LASTEXITCODE"
        }
        Write-Output "Pushed  $($_.Name)"
    }

    Write-Output "Finished deploying $releaseName"
} finally {
    Pop-Location
}