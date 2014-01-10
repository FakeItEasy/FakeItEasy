require 'albacore'
require 'fileutils'

assembly_info = "Source/CommonAssemblyInfo.cs"
version = IO.read(assembly_info)[/AssemblyInformationalVersion\("([^"]+)"\)/, 1]
nunit_command = "Source/packages/NUnit.Runners.2.6.2/tools/nunit-console.exe"
mspec_command = "Source/packages/Machine.Specifications.0.5.11/tools/mspec-clr4.exe"
nuget_command = "Source/.nuget/NuGet.exe"
solution = "Source/FakeItEasy.sln"
unit_tests = ["Source/FakeItEasy.Net35.Tests/bin/Release/FakeItEasy.Net35.Tests.dll", "Source/FakeItEasy.Tests/bin/Release/FakeItEasy.Tests.dll", "Source/FakeItEasy-SL.Tests/Bin/Release/FakeItEasy-SL.Tests.dll"]
integration_tests = ["Source/FakeItEasy.IntegrationTests/bin/Release/FakeItEasy.IntegrationTests.dll", "Source/FakeItEasy.IntegrationTests.VB/bin/Release/FakeItEasy.IntegrationTests.VB.dll"]
specs = ["Source/FakeItEasy.Specs/bin/Release/FakeItEasy.Specs.dll"]
nuspec = "Source/FakeItEasy.nuspec"
output_folder = "Build"

Albacore.configure do |config|
  config.log_level = :verbose
end

desc "Execute default tasks"
task :default => [ :unit, :integ, :spec, :pack ]

desc "Clean solution"
msbuild :clean do |msb|
  FileUtils.rmtree output_folder
  msb.properties = { :configuration => :Release }
  msb.targets = [ :Clean ]
  msb.solution = solution
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
  asm.input_file = assembly_info
  asm.output_file = assembly_info
end

desc "Build solution"
msbuild :build => [:clean] do |msb|
  msb.properties = { :configuration => :Release }
  msb.targets = [ :Build ]
  msb.solution = solution
end

task :create_output_folder do
  FileUtils.mkpath output_folder
end

desc "Execute unit tests"
nunit :unit => [:build, :create_output_folder] do |nunit|
  nunit.command = nunit_command
  nunit.assemblies unit_tests
  nunit.options "/result=#{output_folder}/TestResult.Unit.xml"
end

desc "Execute integration tests"
nunit :integ => [:build, :create_output_folder] do |nunit|
  nunit.command = nunit_command
  nunit.assemblies integration_tests
  nunit.options "/result=#{output_folder}/TestResult.Integration.xml"
end

desc "Execute specifications"
mspec :spec => [:build] do |mspec|
  mspec.command = mspec_command
  mspec.assemblies specs
end

desc "create the nuget package"
exec :pack => [:build, :create_output_folder] do |cmd|
  cmd.command = nuget_command
  cmd.parameters "pack #{nuspec} -Version #{version} -OutputDirectory #{output_folder}"
end
