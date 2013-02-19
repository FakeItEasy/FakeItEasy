require 'albacore'
require 'fileutils'

version = IO.read("Source/Version.txt")
nunit_command = "Source/packages/NUnit.Runners.2.6.2/tools/nunit-console.exe"
mspec_command = "Source/packages/Machine.Specifications.0.5.8/tools/mspec-clr4.exe"
ilrepack_command = "Source/packages/ILRepack.1.21.3/lib/ilrepack.exe"
nuget_command = "Source/.nuget/NuGet.exe"

Albacore.configure do |config|
  config.log_level = :verbose
end

desc "Execute default tasks"
task :default => [ :unit, :integ, :spec, :pack ]

desc "Clean solution"
msbuild :clean do |msb|
  FileUtils.rmtree "Build"
  msb.properties = { :configuration => :Release }
  msb.targets = [ :Clean ]
  msb.solution = "Source/FakeItEasy.sln"
end

desc "Update version number"
assemblyinfo :version do |asm|
  net_version = version.split("-").first
  asm.version = net_version
  asm.file_version = net_version
  asm.custom_attributes = { :AssemblyInformationalVersion => version, :AssemblyConfiguration => :Release.to_s + " built on " + Time.now.strftime("%Y-%m-%d %H:%M:%S%z") }
  asm.output_file = "Source/VersionAssemblyInfo.cs"
end

desc "Build solution"
msbuild :build => [:clean, :version] do |msb|
  msb.properties = { :configuration => :Release }
  msb.targets = [ :Build ]
  msb.solution = "Source/FakeItEasy.sln"
end

desc "Execute unit tests"
nunit :unit => [:build] do |nunit|
  nunit.command = nunit_command
  nunit.assemblies "Source/FakeItEasy.Net35.Tests/bin/Debug/FakeItEasy.Net35.Tests.dll", "Source/FakeItEasy.Tests/bin/Debug/FakeItEasy.Tests.dll", "Source/FakeItEasy-SL.Tests/Bin/Debug/FakeItEasy-SL.Tests.dll"
  nunit.options "/result=TestResult.Unit.xml"
end

desc "Execute integration tests"
nunit :integ => [:build] do |nunit|
  nunit.command = nunit_command
  nunit.assemblies "Source/FakeItEasy.IntegrationTests/bin/Debug/FakeItEasy.IntegrationTests.dll", "Source/FakeItEasy.IntegrationTests.VB/bin/Debug/FakeItEasy.IntegrationTests.VB.dll"
  nunit.options "/result=TestResult.Integration.xml"
end

desc "Execute specifications"
mspec :spec => [:build] do |mspec|
  mspec.command = mspec_command
  mspec.assemblies "Source/FakeItEasy.Specs/bin/Debug/FakeItEasy.Specs.dll"
end

desc "create the nuget package"
exec :pack => [:build, :ilrepack, :ilrepack_35, :ilrepack_sl] do |cmd|
  FileUtils.mkpath "Build"
  cmd.command = nuget_command
  cmd.parameters "pack Source/FakeItEasy.nuspec -Version " + version + " -OutputDirectory Build"
end

desc "merge dependencies"
exec :ilrepack do |cmd|
  assemblies = "Source/FakeItEasy/Bin/Release/FakeItEasy.dll Source/FakeItEasy/Bin/Release/Castle.Core.dll"

  cmd.command = ilrepack_command
  cmd.parameters = "/internalize /out:Source/FakeItEasy/Bin/Release/Merged/FakeItEasy.dll /xmldocs /targetplatform:v4 /log:Source/FakeItEasy/Bin/Release/Merged/merge.log.txt #{ assemblies } "
end

desc "merge 3.5 dependencies"
exec :ilrepack_35 do |cmd|
  assemblies = "Source/FakeItEasy.Net35/Bin/Release/FakeItEasy.dll Source/FakeItEasy.Net35/Bin/Release/Castle.Core.dll"

  cmd.command = ilrepack_command
  cmd.parameters = "/internalize /out:Source/FakeItEasy.Net35/Bin/Release/Merged/FakeItEasy.dll /xmldocs /targetplatform:v4 /log:Source/FakeItEasy.Net35/Bin/Release/Merged/merge.log.txt #{ assemblies } "
end

desc "merge SL dependencies"
exec :ilrepack_sl do |cmd|
  assemblies = "Source/FakeItEasy-SL/Bin/Release/FakeItEasy.dll Source/FakeItEasy-SL/Bin/Release/Castle.Core.dll"

  cmd.command = ilrepack_command
  cmd.parameters = "/internalize /out:Source/FakeItEasy-SL/Bin/Release/Merged/FakeItEasy.dll /xmldocs /targetplatform:v4 /log:Source/FakeItEasy-SL/Bin/Release/Merged/merge.log.txt #{ assemblies } "
end