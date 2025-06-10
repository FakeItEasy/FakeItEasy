# How to build

These instructions focus on building from the command line, which includes compilation, test execution and packaging.
This is the simplest way to build.
It also replicates the build on the Continuous Integration (CI) build server and is the best indicator of whether a pull request will build.
It's possible to build and test in various IDEs as well, but these may not perfectly replicate the official builds.

## Supported frameworks and prerequisites

FakeItEasy can be built and tested on Windows, Linux, and macOS operating systems with a few differences between the OSes.

The build requires that a few pieces of software be installed on the host computer. We're somewhat aggressive about adopting new language features and the like, so rather than specifying exactly which versions are required, we'll tend toward "latest" or "at least" forms of guidance. If it seems you have an incompatible version of the software, prefer to upgrade rather than downgrade.

To build FakeItEasy at all, you must have

* an up-to-date version of the .NET 9.0 SDK (currently this means 9.0.300 or later)

FakeItEasy supports the following targets

| Target                | Tested On            | Additional prerequisites                                                        | Build Profile  |
|-----------------------|----------------------|---------------------------------------------------------------------------------|----------------|
| .NET 8.0              | .NET 8.0             |                                                                                 | net8.0         |
| .NET Framework 4.6.2  | .NET Framework 4.7.2 | Windows OS, .NET Framework 4.7.2 or higher, .NET Framework 4.6.2 targeting pack | net462         |

The default [build profile](#building-only-a-subset-of-the-supported-target-frameworks) (called `full`)
will build and test all targets that are supported on the active operating system, so will require all
the additional prerequisites listed above.

[Partial builds](#building-only-a-subset-of-the-supported-target-frameworks) are also supported.

## Building

Using a command prompt, navigate to your clone root folder and execute:

- `build.cmd` on Windows
- `./build.sh` on Linux or macOS
- `./build.ps1` on any OS, if PowerShell is installed

This executes the default build targets to produce artifacts for various target frameworks,
depending on the selected [build profile](#building-only-a-subset-of-the-supported-target-frameworks).

After the build has completed, the build artifacts will be located in `artifacts`.

### Running specific build tasks

`build.cmd`, `build.sh`, and `build.ps1` wrap a [Bullseye](https://github.com/adamralph/bullseye) targets project, so you can use all the usual command line arguments that you would use with Bullseye, e.g.:

* View the full list of build targets with their dependencies:

    `build.cmd -t`

* Run a specific target:

    `build.cmd spec`

* Run multiple targets:

    `build.cmd spec pack`

* Run a target without running its dependencies (might fail if the dependencies
  haven't been previously built):

    `build.cmd -s spec`

* View the full list of options:

    `build.cmd -?`

(Depending on your operating system or preferred shell, replace `build.cmd` with `./build.sh` or `./build.ps1`.)

### Alternative flow: building with Visual Studio

We've only tested building FakeItEasy from Visual Studio on Windows. It can be convenient,
but is not official and does not provide the same assurance as a command-line build.
The additional requirements are:

1. Visual Studio 2022 (17.0 or later)
2. Install the following components via the Visual Studio installer (in the "Individual components" tab):
   * .NET Framework 4.7.2 targeting pack
   * .NET Framework 4.6.2 targeting pack

### Building only a subset of the supported target frameworks

FakeItEasy is built to target and be tested against multiple versions of .NET as listed under
[Supported frameworks and prerequisites](#supported-frameworks-and-prerequisites) above.
In some cases, you may want to build only a subset of the frameworks. Perhaps you're
unable to install a particular prerequisite, or you want to iterate quickly and so you
want to try building a single framework for a while before finally testing with all
frameworks.

To this end, FakeItEasy's build infrastructure has "build profiles", which makes it possible to
build only a subset of the target frameworks. All the profiles listed above are available,
including `full`, the default profile which builds all target frameworks supported on the current platform.

To activate a profile, create or update a `FakeItEasy.user.props` file at the root
of the repository by running

```
build.cmd use-profile-<profile name>
```

For example:

```
build.cmd use-profile-net8.0
```

or

```
build.cmd use-profile-full
```

Note that Visual Studio will not reflect a change of build profile until you
reload the solution. The command line build will reflect the change immediately.

### Building the documentation

The CI workflow uses [mkdocs](https://www.mkdocs.org/) to build the documentation.
We use [uv](https://docs.astral.sh/uv/) to manage mkdocs and other Python tools used for the documentation.
To build the documentation, **[install uv](https://docs.astral.sh/uv/getting-started/installation/)**.

After this, you can generate the docs by running

```
build.cmd docs
```

The documentation will be built in `artifacts/docs` and can be viewed directly in a web
browser or served using a number of tools such as [dotnet-serve](https://github.com/natemcmaster/dotnet-serve) or [http.server](https://docs.python.org/3/library/http.server.html).

### Updating the documentation-building packages

The versions of the packages used to build the documentation are frozen using uv as well.

If you wish to update the packages used, edit `pyproject.toml`, specifying the dev-dependencies, and run

```
uv lock
```

from the root of the repository. After verifying the generated documentation, you can commit both
`pyproject.toml` and `uv.lock`.
