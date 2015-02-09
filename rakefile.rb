require 'albacore'

msbuild_command = "C:/Program Files (x86)/MSBuild/12.0/Bin/MSBuild.exe"
nuget_command  = "Source/packages/NuGet.CommandLine.2.8.0/tools/NuGet.exe"
nunit_command  = "Source/packages/NUnit.Runners.2.6.3/tools/nunit-console.exe"
mspec_command  = "Source/packages/Machine.Specifications.Runner.Console.0.9.0/tools/mspec-clr4.exe"

solution       = "Source/FakeItEasy.sln"
assembly_info  = "Source/CommonAssemblyInfo.cs"
version        = IO.read(assembly_info)[/AssemblyInformationalVersion\("([^"]+)"\)/, 1]
version_suffix = ENV["VERSION_SUFFIX"]
nuspec         = "Source/FakeItEasy.nuspec"
logs           = "artifacts/logs"
output         = "artifacts/output"
tests          = "artifacts/tests"

unit_tests = [
  "Source/FakeItEasy.Net35.Tests/bin/Release/FakeItEasy.Net35.Tests.dll",
  "Source/FakeItEasy.Tests/bin/Release/FakeItEasy.Tests.dll",
  "Source/FakeItEasy-SL.Tests/Bin/Release/FakeItEasy-SL.Tests.dll",
  "Source/FakeItEasy.Win8.Tests/Bin/Release/FakeItEasy.Win8.Tests.dll",
  "Source/FakeItEasy.Win81.Tests/Bin/Release/FakeItEasy.Win81.Tests.dll",
]

integration_tests = [
  "Source/FakeItEasy.IntegrationTests/bin/Release/FakeItEasy.IntegrationTests.dll",
  "Source/FakeItEasy.IntegrationTests.VB/bin/Release/FakeItEasy.IntegrationTests.VB.dll"
]

specs = [
  "Source/FakeItEasy.Specs/bin/Release/FakeItEasy.Specs.dll"
]

repo = 'FakeItEasy/FakeItEasy'
release_issue_labels = ['0 - Backlog', 'P2', 'build', 'documentation']
release_issue_body = <<-eos
**Ready** when all other issues forming part of the release are **Done**.

- [ ] run code analysis in VS in *Release* mode and address violations (send a regular PR which must be merged before continuing)
- [ ] if necessary, change `VERSION_SUFFIX` on [CI Server](http://teamcity.codebetter.com/admin/editBuildParams.html?id=buildType:bt929)
      to appropriate "-beta123" or "" (for non-betas) value and initiate a build
- [ ] check build
-  edit draft release in [GitHub UI](https://github.com/FakeItEasy/FakeItEasy/releases):
    - [ ] complete release notes, mentioning non-owner contributors, if any
    - [ ] attach nupkg
    - [ ] publish the release
- [ ] push NuGet package
- [ ] copy release notes from GitHub to NuGet
- [ ] de-list pre-release or superseded buggy NuGet packages if present (copy any release notes forward to the new version)
- [ ] update website with contributors list (if in place)
- [ ] tweet, mentioning contributors and post link as comment here for easy retweeting ;-)
- [ ] post tweet in JabbR ([fakeiteasy][1] and [general-chat][2]) and Gitter ([FakeItEasy/FakeItEasy][3])
- [ ] post links to the NuGet and GitHub release in each issue in this milestone, with thanks to contributors
- [ ] use `rake set_version[new_version]` to 
    - create a new branch
    - change CommonAssemblyInfo.cs to expected minor version (of form _xx.yy.zz_)
    - push to origin
    - create PR to upstream master
- [ ] use `rake create_milestone` to
    - create a new milestone for the next release
    - create new issue (like this one) for the next release, adding it to the new milestone
    - create a new draft GitHub Release 
- [ ] close all issues on this milestone
- [ ] close this milestone

[1]: https://jabbr.net/#/rooms/fakeiteasy
[2]: https://jabbr.net/#/rooms/general-chat
[3]: https://gitter.im/FakeItEasy/FakeItEasy
eos

release_body = <<-eos
* **Changed**: _&lt;description&gt;_ - _#&lt;issue number&gt;_
* **New**: _&lt;description&gt;_ - _#&lt;issue number&gt;_
* **Fixed**: _&lt;description&gt;_ - _#&lt;issue number&gt;_

With special thanks for contributions to this release from:

* _&lt;user's actual name&gt;_ - _@&lt;github_userid&gt;_
eos

ssl_cert_file_url = "http://curl.haxx.se/ca/cacert.pem"

Albacore.configure do |config|
  config.log_level = :verbose
end

desc "Execute default tasks"
task :default => [ :vars, :unit, :integ, :spec, :pack ]

desc "Print all variables"
task :vars do
  print_vars(local_variables.sort.map { |name| [name.to_s, (eval name.to_s)] })  
end

desc "Restore NuGet packages"
exec :restore do |cmd|
  cmd.command = nuget_command
  cmd.parameters "restore #{solution}"
end

directory logs

desc "Clean solution"
task :clean => [logs] do
  run_msbuild solution, "Clean", msbuild_command
end

desc "Update version number"
task :set_version, :new_version do |asm, args|
  current_branch = `git rev-parse --abbrev-ref HEAD`.strip()
  
  if current_branch != 'master'
    fail("ERROR: Current branch is '#{current_branch}'. Must be on branch 'master' to set new version.")
  end if

  new_version = args.new_version
  new_branch = "set-version-to-" + new_version

  require 'octokit'

  ssl_cert_file = get_temp_ssl_cert_file(ssl_cert_file_url)

  client = Octokit::Client.new(:netrc => true)

  puts "Creating branch '#{new_branch}'..."
  `git checkout -b #{new_branch}`
  puts "Created branch '#{new_branch}'."

  puts "Setting version to '#{new_version}' in '#{assembly_info}'..."
  Rake::Task["set_version_in_assemblyinfo"].invoke(new_version)
  puts "Set version to '#{new_version}' in '#{assembly_info}'."

  puts "Committing '#{assembly_info}'..."
  `git commit -m "setting version to #{new_version}" #{assembly_info}`
  puts "Committed '#{assembly_info}'."

  puts "Pushing '#{new_branch}' to origin..."
  `git push origin #{new_branch}"`
  puts "Pushed '#{new_branch}' to origin."

  puts "Creating pull request..."
  pull_request = client.create_pull_request(
    repo,
    "master",
    "#{client::user.login}:#{new_branch}",
    "set version to #{new_version} for next release",
    "preparing for #{new_version}"
  )
  puts "Created pull request \##{pull_request.number} '#{pull_request.title}'."
end

desc "Update assembly info"
assemblyinfo :set_version_in_assemblyinfo, :new_version do |asm, args|
  new_version = args.new_version
  
  # not using asm.version and asm.file_version due to StyleCop violations
  asm.custom_attributes = {
    :AssemblyVersion => new_version,
    :AssemblyFileVersion => new_version,
    :AssemblyInformationalVersion => new_version
  }
  asm.input_file = assembly_info
  asm.output_file = assembly_info
end

desc "Build solution"
task :build => [:clean, :restore, logs] do
  run_msbuild solution, "Build", msbuild_command
end

directory tests

desc "Execute unit tests"
nunit :unit => [:build, tests] do |nunit|
  nunit.command = nunit_command
  nunit.assemblies unit_tests
  nunit.options "/result=#{tests}/TestResult.Unit.xml", "/nologo"
end

desc "Execute integration tests"
nunit :integ => [:build, tests] do |nunit|
  nunit.command = nunit_command
  nunit.assemblies integration_tests
  nunit.options "/result=#{tests}/TestResult.Integration.xml", "/nologo"
end

desc "Execute specifications"
mspec :spec => [:build, tests] do |mspec|
  mspec.command = mspec_command
  mspec.assemblies specs
  mspec.html_output = "#{tests}/TestResult.Specifications.html"
  mspec.options "--timeinfo", "--progress", "--silent"
end

directory output

desc "create the nuget package"
exec :pack => [:build, output] do |cmd|
  cmd.command = nuget_command
  cmd.parameters "pack #{nuspec} -Version #{version}#{version_suffix} -OutputDirectory #{output}"
end

desc "create new milestone, release issue and release"
task :create_milestone do |t|
  require 'octokit'

  ssl_cert_file = get_temp_ssl_cert_file(ssl_cert_file_url)

  client = Octokit::Client.new(:netrc => true)

  release_description = version + ' release'

  puts "Creating milestone '#{version}'..."
  milestone = client.create_milestone(
    repo,
    version,
    :description => release_description
    )
  puts "Created milestone '#{version}'."

  puts "Creating issue '#{release_description}'..."
  issue = client.create_issue(
    repo,
    release_description,
    release_issue_body,
    :labels => release_issue_labels,
    :milestone => milestone.number
    )
  puts "Created issue \##{issue.number} '#{release_description}'."

  puts "Creating release '#{version}'..."
  client.create_release(
    repo,
    version,
    :name => version,
    :draft => true,
    :body => release_body
    )
  puts "Created release '#{version}'."
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
  vectors.select { |name, value| !['release_body', 'release_issue_body', 'release_issue_labels'].include? name }.each { |name, value| 
    puts "#{name}:"
    puts value.map {|v| "  " + v }
    puts ""
  }
end

def run_msbuild(solution, target, command)
  cmd = Exec.new
  cmd.command = command
  cmd.parameters "#{solution} /target:#{target} /p:configuration=Release /nr:false /verbosity:minimal /nologo /fl /flp:LogFile=artifacts/logs/#{target}.log;Verbosity=Detailed;PerformanceSummary"
  cmd.execute
end

# Get a temporary SSL cert file if necessary.
# If ENV["SSL_CERT_FILE"] is set, will return nil.
# Otherwise, attempts to download a known
# SSL cert file, sets ENV["SSL_CERT_FILE"]
# to point at it, and returns the file (mostly so it will
# stay in scope while it's needed).
def get_temp_ssl_cert_file(ssl_cert_file_url)
  ssl_cert_file_path = ENV["SSL_CERT_FILE"]
  if ssl_cert_file_path
    return nil
  end
  
  puts "Environment variable SSL_CERT_FILE is not set. Downloading a cert file from '#{ssl_cert_file_url}'..."

  require 'open-uri'
  require 'tempfile'

  file = Tempfile.new('ssl_cert_file')
  file.binmode
  file << open(ssl_cert_file_url).read
  file.close

  ENV["SSL_CERT_FILE"] = file.path

  puts "Downloaded cert file to '#{ENV['SSL_CERT_FILE']}'."
  return file
end
