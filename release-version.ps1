[CmdletBinding()]
Param ( 
    [string]$NewVersion = $(throw "NewVersion is required"),
    [string]$remoteName = 'upstream'
)

# ------------------------------------------------------------------------------

$branchName = "release/$NewVersion"

Write-Host "Releasing version $NewVersion"
$response = Read-Host "  Proceed (y/N)?"
Switch ($response) {
    y { }
    n { Write-Host "Release cancelled."; return }
    Default { Write-Host "Unknown response '$response'. Aborting."; return }
}

git checkout --quiet -b  $branchName develop
git checkout --quiet master
git merge --quiet --no-ff $branchName
git branch --delete $branchName
git tag $NewVersion
git push --tags $RemoteName HEAD