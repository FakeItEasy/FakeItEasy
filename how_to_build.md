# How to build

These instructions are *only* for building from the command line, which includes compilation, test execution and packaging. This is the simplest way to build.
It also replicates the build on the Continuous Integration build server and is the best indicator of whether a pull request will build.

You can also build the solution using Visual Studio 2017 or later, but this doesn't provide the same assurances as the command line build.

At the time of writing the build is only confirmed to work on Windows using the Microsoft .NET framework.

## Prerequisites

The build requires that a few pieces of software be installed on the host computer. We're somewhat aggressive about adoptiong new language features and the like, so rather than specifying exactly which versions are required, we'll tend toward
"latest" or "at least" forms of guidance. If it seems you have an incompatible version of the software, prefer to upgrade rather than downgrade.

Ensure that the following are installed:

1. .NET 3.5, required by ILMerge

1. a very recent version of Visual Studio 2017 (currently this means 15.7 or later) or the Build Tools for Visual Studio 2017

1. a very recent version of the .NET Core SDK (currently this means 2.1.3 or later)

## Building

Using a command prompt, navigate to your clone root folder and execute `build.cmd`.

This executes the default build targets to produce both .NET Standard and .NET Framework artifacts.

After the build has completed, the build artifacts will be located in `artifacts`.

## Extras

`build.cmd` wraps a [Bullseye](https://github.com/adamralph/bullseye) targets project, so you can use all the usual command line arguments that you would use with Bullseye, e.g.:

* View the full list of build targets:

    `build.cmd -T`

* Run a specific target:

    `build.cmd spec`

* Run multiple targets:

    `build.cmd spec pack`

* View the full list of options:

    `build.cmd -?`
