require 'albacore'
require 'fileutils'

nuget_command = "Source/packages/NuGet.CommandLine.2.8.0/tools/NuGet.exe"
nunit_command = "Source/packages/NUnit.Runners.2.6.3/tools/nunit-console.exe"
mspec_command = "Source/packages/Machine.Specifications.0.8.0/tools/mspec-clr4.exe"

solution      = "Source/FakeItEasy.sln"
assembly_info = "Source/CommonAssemblyInfo.cs"
version       = IO.read(assembly_info)[/AssemblyInformationalVersion\("([^"]+)"\)/, 1]
nuspec        = "Source/FakeItEasy.nuspec"
output_folder = "Build"

unit_tests = [
  "Source/FakeItEasy.Net35.Tests/bin/Release/FakeItEasy.Net35.Tests.dll",
  "Source/FakeItEasy.Tests/bin/Release/FakeItEasy.Tests.dll",
  "Source/FakeItEasy-SL.Tests/Bin/Release/FakeItEasy-SL.Tests.dll"
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
- [ ] check build, update draft release in [GitHub UI](https://github.com/FakeItEasy/FakeItEasy/releases)
       including release notes, mentioning non-owner contributors, if any
- [ ] push NuGet package
- [ ] copy release notes from GitHub to NuGet
- [ ] de-list pre-release NuGet packages if present
- [ ] update website with contributors list (if in place)
- [ ] tweet, mentioning contributors and post link as comment here for easy retweeting ;-)
- [ ] post tweet in JabbR ([fakeiteasy][1] and [general-chat][2]) and Gitter ([FakeItEasy/FakeItEasy][3])
- [ ] post links to the NuGet and GitHub release in each issue in this milestone, with thanks to contributors
- [ ] use `rake set_version[new_version]` to change CommonAssemblyInfo.cs to expected minor version (of form _xx.yy.zz_)
- [ ] push to origin branch, create PR to upstream master
- [ ] use `rake create_milestone[new_version]` to
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

desc "Clean solution"
msbuild :clean do |msb|
  FileUtils.rmtree output_folder
  msb.properties = { :configuration => :Release }
  msb.targets = [ :Clean ]
  msb.solution = solution
end

desc "Update version number"
assemblyinfo :set_version, :new_version do |asm, args|
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
msbuild :build => [:clean, :restore] do |msb|
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

desc "create new milestone, release issue and release"
task :create_milestone, :milestone_version do |t, args|
  require 'octokit'

  ssl_cert_file = get_temp_ssl_cert_file(ssl_cert_file_url)

  client = Octokit::Client.new(:netrc => true)

  release_description = args.milestone_version + ' release'

  puts "Creating milestone '#{args.milestone_version}'..."
  milestone = client.create_milestone(
    repo,
    args.milestone_version,
    :description => release_description
    )
  puts "Created milestone '#{args.milestone_version}'."

  puts "Creating issue '#{release_description}'..."
  issue = client.create_issue(
    repo,
    release_description,
    release_issue_body,
    :labels => release_issue_labels,
    :milestone => milestone.number
    )
  puts "Created issue \##{issue.number} '#{release_description}'."

  puts "Creating release '#{args.milestone_version}'..."
  client.create_release(
    repo,
    args.milestone_version,
    :name => args.milestone_version,
    :draft => true,
    :body => release_body
    )
  puts "Created release '#{args.milestone_version}'."
end

def highlight(text)
  return "\e[33m#{text}\e[0m"
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
    puts "#{highlight(name)}:#{' ' * (scalar_name_column_width - name.length)} #{value}"
  }

  puts
  vectors.each { |name, value| 
    puts "#{highlight(name)}:"
    puts value.map {|v| "  " + v }
    puts ""
  }
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
