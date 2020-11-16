# How to build

These instructions are *only* for building from the command line, which includes compilation, test execution and packaging. This is the simplest way to build.
It also replicates the build on the Continuous Integration build server and is the best indicator of whether a pull request will build.

You can also build the solution using Visual Studio 2019 or later, but this doesn't provide the same assurances as the command line build.

At the time of writing the full build (including all target frameworks) can only run on Windows.

[Partial builds](#building-only-a-subset-of-the-supported-target-frameworks) are supported on Linux (Ubuntu 18.04). This might also run on macOS, but hasn't been tested.

## Prerequisites

The build requires that a few pieces of software be installed on the host computer. We're somewhat aggressive about adoptiong new language features and the like, so rather than specifying exactly which versions are required, we'll tend toward "latest" or "at least" forms of guidance. If it seems you have an incompatible version of the software, prefer to upgrade rather than downgrade.

### On Windows

Ensure that the following are installed:

1. a recent version of Visual Studio 2019 (currently this means 16.3 or later) or the Build Tools for Visual Studio 2019

2. The .NET Core 1.0 runtime

3. The .NET Core 2.1 runtime

4. The .NET Framework 4.6.1 or higher

5. The targeting packs for .NET Framework 4.0 and 4.5

6. A recent version of the .NET Core 5.0 SDK (currently this means 5.0.100 or later)

You might not need everything to run a [partial build](#building-only-a-subset-of-the-supported-target-frameworks).

### On Linux

The default [build profile](#building-only-a-subset-of-the-supported-target-frameworks) on Linux is `netcore2+`, which builds FakeItEasy for .NET Standard 2.0 and 2.1, and runs the tests on .NET Core 2.1 and 3.1. The .NET Framework isn't supported on Linux, and .NET Core 1.0 is no longer supported, which makes it difficult (if not impossible) to install on Linux.

Ensure the following are installed:

1. The .NET Core 2.1 runtime

2. A recent version of the .NET Core 3.1 SDK (currently this means 3.1.404 or later)

## Building

Using a command prompt, navigate to your clone root folder and execute:

- `build.cmd` on Windows
- `./build.sh` on Linux

This executes the default build targets to produce .NET Standard and/or .NET Framework artifacts, depending on the selected [build profile](#building-only-a-subset-of-the-supported-target-frameworks).

After the build has completed, the build artifacts will be located in `artifacts`.

## Extras

### Running specific build tasks

`build.cmd` wraps a [Bullseye](https://github.com/adamralph/bullseye) targets project, so you can use all the usual command line arguments that you would use with Bullseye, e.g.:

* View the full list of build targets:

    `build.cmd -T`

* Run a specific target:

    `build.cmd spec`

* Run multiple targets:

    `build.cmd spec pack`

* Run a target without running its dependencies (might fail if the dependencies
  haven't been previously built):

    `build.cmd -s spec`

* View the full list of options:

    `build.cmd -?`

(On Linux, just replace `build.cmd` with `./build.sh`)

### Building only a subset of the supported target frameworks

FakeItEasy targets multiple versions of .NET (.NET Framework 4.0 and 4.5, .NET
Standard 2.0 and 2.1), and the tests also run on multiple frameworks (.NET
Framework 4.6.1 and several versions of .NET Core). A consequence is that a full
build can take a significant amount of time. When working on the code, you might
want a faster feedback loop. To this end, FakeItEasy's build infrastructure has
the concept of "build profiles", which makes it possible to build only a subset
of all the target frameworks supported by FakeItEasy. The following profiles are
available:

* `full`: the default profile on Windows, builds all supported target frameworks
* `net45`: builds only the .NET Framework 4.5 target framework
* `netcore2+`: the default profile on non-Windows OSes, builds only .NET Core 2.1 and 3.1 / .NET Standard 2.0 and 2.1 target frameworks.
* `netcore2.1`: builds only .NET Core 2.1 / .NET Standard 2.0 target frameworks
* `netcore3.1`: builds only .NET Core 3.1 / .NET Standard 2.1 target frameworks
* `netcoreall`: builds all supported .NET Core / .NET Standard target frameworks

In order to select a profile, create a `FakeItEasy.user.props` file at the root
of the repository by running

```
build.cmd initialize-user-properties
```

Thereafter
you can switch profiles by replacing the contents of the `BuildProfile` element
either by editing the file by hand or via a build target (which will actually 
create the file if it doesn't already exist). For example:

```
build.cmd use-profile-netcore3.1
```

Note that Visual Studio will not reflect a change of build profile until you
reload the solution. The command line build will reflect the change immediately.
