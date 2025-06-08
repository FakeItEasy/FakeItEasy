# Platform support

## Supported platforms for released versions

See the "Dependencies" section of the [FakeItEasy NuGet Gallery page](https://www.nuget.org/packages/FakeItEasy) to see each released package's supported platforms and package dependencies.

## Platform support policy

Beginning with FakeItEasy 8.0.0, we intend to publish new packages that target

* .NET versions designated as Long Term Support (LTS) and currently supported by Microsoft
* .NET Framework 4.6.2[^1]

See the [.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy) to learn which target frameworks
are categorized as "Long Term Support", and when support begins and ends.

We will add builds for new LTS .NET versions as they are released, and remove them from
new FakeItEasy builds once support is ended.

It's not clear that there's a global consensus on whether the removal of a supported target framework constitutes
a breaking change and therefore requires a new major version. We choose to limit surprises to FakeItEasy's clients
and issue a new major release when dropping a supported target framework.

[^1]:
    There are no explicit plans to remove support for .NET Framework, but we may increase the supported
    version or remove support if Microsoft ends support for .NET Framework, or if supporting it within
    FakeItEasy becomes too onerous.
