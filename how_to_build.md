# How to build

These instructions are *only* for building from the command line, which includes compilation, test execution and packaging. This is the simplest way to build.
It also replicates the build on the Continuous Integration build server and is the best indicator of whether a pull request will build.

You can also build the solution using Visual Studio 2017 or later, but this doesn't provide the same assurances as the command line build.

At the time of writing the build is only confirmed to work on Windows using the Microsoft .NET framework.

## Prerequisites

1. Ensure you have .NET framework 4.6.1 or later installed.

1. Ensure you have version 15.3 or later of either Visual Studio 2017 or MSBuild installed.

1. Ensure you have .NET Core SDK 2.0.2 or later installed

## Building

Using a command prompt, navigate to your clone root folder and execute `build.cmd`.

This executes the default build targets to produce both the .NET Standard and the .NET 4.0 artifacts.

After the build has completed, the build artifacts will be located in `artifacts`.

## Extras

`build.cmd` wraps a [simple-targets-csx](https://github.com/adamralph/simple-targets-csx) script, so you can use all the usual command line arguments that you would use with simple-targets-csx , e.g.:

* View the full list of build targets:

    `build.cmd -T`

* Run a specific target:

    `build.cmd spec`

* Run multiple targets:

    `build.cmd spec pack`

* View the full list of options:

    `build.cmd -?`
