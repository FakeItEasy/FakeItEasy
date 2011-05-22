@echo off
@%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild FakeItEasy.build /property:SignAssembly=true %*
pause