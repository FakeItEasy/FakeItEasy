require 'albacore'
require 'fileutils'

ENV["NUnitConsole"] = "Source/packages/NUnit.Runners.2.6.2/tools/nunit-console.exe"
ENV["MSpec"] = "Source/packages/Machine.Specifications.0.5.8/tools/mspec-clr4.exe"

Albacore.configure do |config|
    config.log_level = :verbose
end

desc "Execute default tasks"
task :default => [ :unit, :integ, :spec, :nugetpack ]

desc "Clean solution"
msbuild :clean do |msb|
    FileUtils.rmtree "Build"

    msb.properties = { :configuration => :Release }
    msb.targets = [ :Clean ]
    msb.solution = "Source/FakeItEasy.sln"
end

task :get_version do
    ENV["version"] = IO.read("Source/Version.txt")
end

desc "Update version number"
assemblyinfo :update_version => [:get_version] do |asm|
    version = ENV["version"].split("-").first
    asm.version = version
    asm.file_version = version
    asm.informational_version = ENV["version"]
    asm.custom_attributes = { :AssemblyConfiguration => :Release.to_s + " built on " + Time.now.strftime("%Y-%m-%d %H:%M:%S%z") }
    asm.output_file = "Source/VersionAssemblyInfo.cs"
end

desc "Build solution"
msbuild :build => [:clean, :update_version] do |msb|
    msb.properties = { :configuration => :Release }
    msb.targets = [ :Build ]
    msb.solution = "Source/FakeItEasy.sln"
end

desc "Execute unit tests"
nunit :unit => [:build] do |nunit|
	nunit.command = ENV["NUnitConsole"]
	nunit.assemblies "Source/FakeItEasy.Net35.Tests/bin/Release/FakeItEasy.Net35.Tests.dll", "Source/FakeItEasy.Tests/bin/Debug/FakeItEasy.Tests.dll", "Source/FakeItEasy-SL.Tests/Bin/Release/FakeItEasy-SL.Tests.dll"
    nunit.options "/result=TestResult.Unit.xml"
end

desc "Execute integration tests"
nunit :integ => [:build] do |nunit|
	nunit.command = ENV["NUnitConsole"]
	nunit.assemblies "Source/FakeItEasy.IntegrationTests/bin/Debug/FakeItEasy.IntegrationTests.dll", "Source/FakeItEasy.IntegrationTests.VB/bin/Debug/FakeItEasy.IntegrationTests.VB.dll"
    nunit.options "/result=TestResult.Integration.xml"
end

desc "Execute specifications"
mspec :spec => [:build] do |mspec|
    FileUtils.mkpath "Build"
	mspec.command = ENV["MSpec"]
	mspec.assemblies "Source/FakeItEasy.Specs/bin/Release/FakeItEasy.Specs.dll"
end

desc "create the nuget package"
exec :nugetpack => [:build, :get_version] do |cmd|
    FileUtils.mkpath "Build"
    cmd.command = "Source/.nuget/NuGet.exe"
    cmd.parameters "pack Source/FakeItEasy.nuspec -Version " + ENV["version"] + " -OutputDirectory Build"
end