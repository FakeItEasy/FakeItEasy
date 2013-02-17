#How to build#

These instructions are *only* for building with Rake, which includes compilation, test execution and packaging. Once you have the prerequisites set up this is the simplest way of building the assemblies.

You can also build the solution using Visual Studio 2010 or later.

## Prerequisites ##

1. Install Ruby 1.8.7 or later. For Windows we recommend using [RubyInstaller](http://rubyinstaller.org/). Other methods of installation are listed on the [Ruby download page](http://www.ruby-lang.org/en/downloads/).
1. Using a command prompt, update RubyGems to the latest version:

    `gem update --system`

1. Install/update the Albacore gem (0.3.5 or later is required):

    `gem install albacore`

## Building ##

Using a command prompt, navigate to your clone root folder and execute:

`rake`

This executes the default build tasks. After the build has completed, the build artifacts will be located in `Build`.

##Extras##

* View the full list of build tasks:

    `rake -T`

* Run a specific task:

    `rake build`

* Run multiple tasks:

    `rake build nugetpack`