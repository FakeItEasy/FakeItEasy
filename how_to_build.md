# How to build

These instructions are *only* for building with Rake, which includes compilation, test execution and packaging. This is the simplest way to build.

You can also build the solution using Visual Studio 2012 or later.

*Don't be put off by the prerequisites!* It only takes a few minutes to set them up and only needs to be done once. If you haven't used [Rake](http://rake.rubyforge.org/ "RAKE -- Ruby Make") before then you're in for a real treat!

At the time of writing the build is only confirmed to work on Windows using the Microsoft .NET framework.

## Prerequisites

1. Ensure you have .NET framework 3.5 and 4.0/4.5 installed.

1. Install the [Silverlight 4 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=7335) or later.

1. Install Ruby 1.8.7 or later.

 For Windows we recommend using [RubyInstaller](http://rubyinstaller.org/) and selecting 'Add Ruby executables to your PATH' when prompted. For alternatives see the [Ruby download page](http://www.ruby-lang.org/en/downloads/).
1. Using a command prompt, update RubyGems to the latest version:

    `gem update --system`

1. Install/update Rake (already included in Ruby 1.9 or later):

    `gem install rake`

1. Install/update Albacore (0.3.4 or later is required):

    `gem install albacore`

## Building

Using a command prompt, navigate to your clone root folder and execute:

`rake`

This executes the default build tasks. After the build has completed, the build artifacts will be located in `Build`.

## Extras

* View the full list of build tasks:

    `rake -T`

* Run a specific task:

    `rake spec`

* Run multiple tasks:

    `rake spec pack`

* View the full list of rake options:

    `rake -h`
