@echo off
git submodule -q update --init
if %errorlevel% neq 0 exit /b %errorlevel%
dotnet run --project "%~dp0..\tools-shared\FakeItEasy.Deploy\FakeItEasy.Deploy.csproj" -- "%~dp0..\artifacts\output" %*
