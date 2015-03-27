# How to build

These instructions are *only* for building with Rake, which includes compilation, test execution and packaging. This is the simplest way to build.
It also replicates the build on the Continuous Integration build server and is the best indicator of whether a pull request will build.

*Don't be put off by the prerequisites!* It only takes a few minutes to set them up and only needs to be done once. If you haven't used [Rake](http://rake.rubyforge.org/ "RAKE -- Ruby Make") before then you're in for a real treat!

You can also build the solution using Visual Studio 2012 or later, but this doesn't provide the same assurances as the Rake build.

At the time of writing the build is only confirmed to work on Windows using the Microsoft .NET framework.

## Prerequisites

1. Ensure you have .NET framework 3.5 and 4.0/4.5 installed.

1. Install the [Silverlight 4 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=7335) or later.

1. Install Ruby 1.8.7 or later.

 For Windows we recommend using [Chocolatey](https://chocolatey.org/). Once you have Chocolatey installed (it takes seconds), just run  

 `cinst ruby`
 
 from a command prompt. 
  
 If you can't use Chocolatey, or prefer not to use it, the next best approach is to use the [RubyInstaller](http://rubyinstaller.org/) and select 'Add Ruby executables to your PATH' when prompted. For other alternatives see the [Ruby download page](http://www.ruby-lang.org/en/downloads/).

1. Using a command prompt, update RubyGems to the latest version:

    `gem update --system`

1. Install bundler:

    `gem install bundler`

1. Install gems:

    `bundler install`

## Building

Using a command prompt, navigate to your clone root folder and execute:

`bundle exec rake`

This executes the default build tasks. After the build has completed, the build artifacts will be located in `Build`.

## Extras

* View the full list of build tasks:

    `bundle exec rake -T`

* Run a specific task:

    `bundle exec rake spec`

* Run multiple tasks:

    `bundle exec rake spec pack`

* View the full list of rake options:

    `bundle exec rake -h`
