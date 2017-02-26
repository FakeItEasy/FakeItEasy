require 'albacore'

msbuild_command = "C:/Program Files (x86)/MSBuild/14.0/Bin/MSBuild.exe"
if !File.file?(msbuild_command)
  raise "MSBuild not found"
end

nuget_command   = ".nuget/nuget.exe"
gitlink_command = "packages/gitlink.2.3.0/lib/net45/GitLink.exe"
xunit_command   = "packages/xunit.runner.console.2.0.0/tools/xunit.console.exe"

solution        = "FakeItEasy.sln"
assembly_info   = "src/CommonAssemblyInfo.cs"
version         = IO.read(assembly_info)[/AssemblyInformationalVersion\("([^"]+)"\)/, 1]
version_suffix  = ENV["VERSION_SUFFIX"]
build           = (ENV["BUILD"] || "").rjust(6, "0");
build_suffix    = version_suffix.to_s.empty? ? "" : "-build" + build;
repo_url        = "https://github.com/FakeItEasy/FakeItEasy"
nuspec          = "src/FakeItEasy.nuspec"
analyzer_nuspec = "src/FakeItEasy.Analyzer.nuspec"
logs            = "artifacts/logs"
output          = File.absolute_path("artifacts/output")
tests           = "artifacts/tests"
packages        = File.absolute_path("packages")

gitlinks        = ["FakeItEasy", "FakeItEasy.Analyzer.CSharp", "FakeItEasy.Analyzer.VisualBasic", "FakeItEasy.netstd"]

unit_tests = [
  "tests/FakeItEasy.Tests/bin/Release/FakeItEasy.Tests.dll",
  "tests/FakeItEasy.Analyzer.CSharp.Tests/bin/Release/FakeItEasy.Analyzer.CSharp.Tests.dll",
  "tests/FakeItEasy.Analyzer.VisualBasic.Tests/bin/Release/FakeItEasy.Analyzer.VisualBasic.Tests.dll"
]

netstd_unit_test_directories = [
  "tests/FakeItEasy.Tests.netstd"
]

integration_tests = [
  "tests/FakeItEasy.IntegrationTests/bin/Release/FakeItEasy.IntegrationTests.dll",
  "tests/FakeItEasy.IntegrationTests.VB/bin/Release/FakeItEasy.IntegrationTests.VB.dll"
]

netstd_integration_test_directories = [
  "tests/FakeItEasy.IntegrationTests.netstd"
]

specs = [
  "tests/FakeItEasy.Specs/bin/Release/FakeItEasy.Specs.dll"
]

netstd_spec_directories = [
  "tests/FakeItEasy.Specs.netstd"
]

approval_tests = [
  "tests/FakeItEasy.Tests.Approval/bin/Release/FakeItEasy.Tests.Approval.dll",
  "tests/FakeItEasy.Tests.Approval.netstd/bin/Release/FakeItEasy.Tests.Approval.netstd.dll"
]

Albacore.configure do |config|
  config.log_level = :verbose
end

desc "Execute default tasks"
task :default => [ :vars, :gitlink, :unit, :integ, :spec, :approve, :pack ]

desc "Print all variables"
task :vars do
  print_vars(local_variables.sort.map { |name| [name.to_s, (eval name.to_s)] })
end

desc "Restore NuGet packages"
task :restore do
  restore_cmd = Exec.new
  restore_cmd.command = nuget_command
  restore_cmd.parameters "restore #{solution} -PackagesDirectory #{packages}"
  restore_cmd.execute

  # performing restore on the solution doesn't restore
  # all the xprojs' dependencies, so do them separately
  (netstd_unit_test_directories + netstd_integration_test_directories + netstd_spec_directories).each do | test_directory |
    restore_dependencies_for_project test_directory
  end
end

directory logs

desc "Clean solution"
task :clean => [logs] do
  run_msbuild solution, "Clean", msbuild_command
end

desc "Build solution"
task :build => [:clean, :restore, logs] do
  run_msbuild solution, "Build", msbuild_command, packages
end

desc "GitLink PDB's"
exec :gitlink => [:build] do |cmd|
  cmd.command = gitlink_command
  cmd.parameters ". -f #{solution} -u #{repo_url} -include " + gitlinks.join(",")
end

directory tests

desc "Execute unit tests"
task :unit => [:build, tests] do
    run_tests(unit_tests, xunit_command, tests)
    run_netstd_tests(netstd_unit_test_directories, tests)
end

desc "Execute integration tests"
task :integ => [:build, tests] do
    run_tests(integration_tests, xunit_command, tests)
    run_netstd_tests(netstd_integration_test_directories, tests)
end

desc "Execute specifications"
task :spec => [:build, tests] do
    run_tests(specs, xunit_command, tests)
    run_netstd_tests(netstd_spec_directories, tests)
end

desc "Execute approval tests"
task :approve => [:build, tests] do
    run_tests(approval_tests, xunit_command, tests)
end

directory output

desc "create the nuget package"
exec :pack => [:build, output] do |cmd|
  cmd.command = nuget_command
  cmd.parameters "pack #{nuspec} -Version #{version}#{version_suffix}#{build_suffix} -OutputDirectory #{output}"
end

desc "create the analyzer nuget package"
exec :pack => [:build, output] do |cmd|
  cmd.command = nuget_command
  cmd.parameters "pack #{analyzer_nuspec} -Version #{version}#{version_suffix}#{build_suffix} -OutputDirectory #{output}"
end

def print_vars(variables)

  scalars = []
  vectors = []

  variables.each { |name, value|
    if value.respond_to?('each')
      vectors << [name, value.map { |v| v.to_s }]
    else
      string_value = value.to_s
      lines = string_value.lines
      if lines.length > 1
        vectors << [name, lines]
      else
        scalars << [name, string_value]
      end
    end
  }

  scalar_name_column_width = scalars.map { |s| s[0].length }.max
  scalars.each { |name, value|
    puts "#{name}:#{' ' * (scalar_name_column_width - name.length)} #{value}"
  }

  puts
  vectors.select { |name, value| ![
                                   'release_body',
                                   'release_issue_rtm_steps',
                                   'release_issue_prerelease_steps',
                                   'release_issue_labels'
                                  ].include? name }.each { |name, value|
    puts "#{name}:"
    puts value.map {|v| "  " + v }
    puts ""
  }
end

def run_msbuild(solution, target, command, packages_dir = nil)
  packages_dir_option = "/p:NuGetPackagesDirectory=#{packages_dir}" if packages_dir
  cmd = Exec.new
  cmd.command = command
  cmd.parameters "#{solution} /target:#{target} /p:configuration=Release /nr:false /verbosity:minimal /nologo /fl /flp:LogFile=artifacts/logs/#{target}.log;Verbosity=Detailed;PerformanceSummary #{packages_dir_option}"
  cmd.execute
end

def run_tests(test_assemblies, command, result_dir)
  test_assemblies.each do |test_assembly|
    xml   = File.expand_path(File.join(result_dir, File.basename(test_assembly, '.dll') + '.TestResults.xml'))
    html  = File.expand_path(File.join(result_dir, File.basename(test_assembly, '.dll') + '.TestResults.html'))

    xunit = XUnitTestRunner.new
    xunit.command = command
    xunit.assembly = test_assembly
    xunit.options '-noshadow', '-nologo', '-notrait', '"explicit=yes"', '-xml', xml, '-html', html
    xunit.execute
  end
end

def run_netstd_tests(test_directories, result_dir)
  test_directories.each do |test_directory|
    xml = File.expand_path(File.join(result_dir, File.basename(test_directory) + '.TestResults.xml'))
    # the xunit .NET Core runner can't produce HTML yet since XSLT isn't scheduled until .NET Core 1.2

    test_cmd = Exec.new
    test_cmd.command = "dotnet"
    test_cmd.parameters "test -c Release #{test_directory} -nologo -notrait \"explicit=yes\" -xml #{xml}"
    test_cmd.execute
  end
end

def restore_dependencies_for_project(project_dir)
  restore_cmd = Exec.new
  restore_cmd.command = "dotnet"
  restore_cmd.parameters "restore #{project_dir}"
  restore_cmd.execute
end
