@echo off
@%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild FakeItEasy.build %* 
@%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild FakeItEasy.build /property:BuildPath=Build-Signed /property:SignAssembly=true %* 
pause