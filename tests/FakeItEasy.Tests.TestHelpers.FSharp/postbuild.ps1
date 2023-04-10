# Due to dotnet/fsharp#14863, FakeItEasy.Tests.TestHelper.FSharp is delay signed.
# This prevents FakeItEasy.Tests from loading the assembly.
# Workaround until the tooling is fixed is to resign the assembly.
# The path to sn.exe seems impossible to find with any certainty, so
# for now we'll probe a few likely location and hope we can remove this
# postbuild step before it becomes an issue.

$assembly = $Args[0]


$windowsSDKExecutablePath = $env:WindowsSDK_ExecutablePath_x86

$netfxsdkKey = "HKLM:\SOFTWARE\Microsoft\Microsoft SDKs\NETFXSDK\4.8\"
if (Test-Path $netfxsdkKey) {
    $toolsItem = Get-ItemProperty "${netfxsdkKey}\WinSDK-NetFx40Tools"
    if ($toolsItem) {
        $netFx40ToolsPath = $toolsItem.InstallationFolderd
    }
    else {
        $netFx40ToolsPath = $null
    }
}

$assumedProgramFilesNextFx48ToolsPath = Join-Path ${env:ProgramFiles(x86)} "Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools"

$candidatePaths = [array]($windowsSDKExecutablePath, $netFx40ToolsPath, $assumedProgramFilesNextFx48ToolsPath |
    Where-Object { $_ } |
    ForEach-Object { Join-Path $_ "sn.exe" } |
    Where-Object { Test-Path $_ }
)

if (!$candidatePaths) {
    throw "Can't find sn.exe."
}

$sn = $candidatePaths[0]

&$sn -q -R $assembly ../../src/FakeItEasy.snk
