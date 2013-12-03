require 'albacore'
require 'fileutils'

version = IO.read("Source/SharedAssemblyInfo.cs")[/AssemblyInformationalVersion\("([^"]+)"\)/, 1]
nunit_command = "Source/packages/NUnit.Runners.2.6.2/tools/nunit-console.exe"
mspec_command = "Source/packages/Machine.Specifications.0.5.11/tools/mspec-clr4.exe"
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
assemblyinfo :set_version, :new_version do |asm, args|
  puts "args were #{args}"
  net_version = args.new_version.split(/[^\d.]/, 2).first
  
  # not using asm.version and asm.file_version due to StyleCop violations
  asm.custom_attributes = {
    :AssemblyVersion => net_version,
    :AssemblyFileVersion => net_version,
    :AssemblyInformationalVersion => args.new_version
  }
  asm.input_file = "Source/SharedAssemblyInfo.cs"
  asm.output_file = "Source/SharedAssemblyInfo.cs"
end

desc "Build solution"
msbuild :build => [:clean] do |msb|
  msb.properties = { :configuration => :Release }
  msb.targets = [ :Build ]
  msb.solution = "Source/FakeItEasy.sln"
end

desc "Execute unit tests"
nunit :unit => [:build] do |nunit|
  nunit.command = nunit_command
  nunit.assemblies "Source/FakeItEasy.Net35.Tests/bin/Release/FakeItEasy.Net35.Tests.dll", "Source/FakeItEasy.Tests/bin/Release/FakeItEasy.Tests.dll", "Source/FakeItEasy-SL.Tests/Bin/Release/FakeItEasy-SL.Tests.dll"
  nunit.options "/result=TestResult.Unit.xml"
end

desc "Execute integration tests"
nunit :integ => [:build] do |nunit|
  nunit.command = nunit_command
  nunit.assemblies "Source/FakeItEasy.IntegrationTests/bin/Release/FakeItEasy.IntegrationTests.dll", "Source/FakeItEasy.IntegrationTests.VB/bin/Release/FakeItEasy.IntegrationTests.VB.dll"
  nunit.options "/result=TestResult.Integration.xml"
end

desc "Execute specifications"
mspec :spec => [:build] do |mspec|
  mspec.command = mspec_command
  mspec.assemblies "Source/FakeItEasy.Specs/bin/Release/FakeItEasy.Specs.dll"
end

desc "create the nuget package"
exec :pack => [:build] do |cmd|
  FileUtils.mkpath "Build"
  cmd.command = nuget_command
  cmd.parameters "pack Source/FakeItEasy.nuspec -Version " + version + " -OutputDirectory Build"
end
