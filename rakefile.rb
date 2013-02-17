require 'albacore'
require 'fileutils'

Albacore.configure do |config|
    config.log_level = :verbose
end

desc "Execute default tasks"
task :default => [ :nugetpack ]

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

desc "create the nuget package"
exec :nugetpack => [:build, :get_version] do |cmd|
    FileUtils.mkpath "Build"
    cmd.command = "Source/.nuget/NuGet.exe"
    cmd.parameters "pack Source/FakeItEasy.nuspec -Version " + ENV["version"] + " -OutputDirectory Build"
end