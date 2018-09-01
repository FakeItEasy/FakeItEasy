@pushd %~dp0
@dotnet run --project ".\tools\FakeItEasy.Build\FakeItEasy.Build.csproj" -- %*
@popd
