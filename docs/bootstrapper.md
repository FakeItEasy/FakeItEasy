# Bootstrapper

Most of FakeItEasy's functionality is directly triggered by client
code: [creating a fake](creating-fakes.md),
[configuring a call](specifying-a-call-to-configure.md) and
[making assertions about calls](assertion.md) are all explicitly
invoked and are controllable by various input parameters.

Some behavior is triggered implicitly. FakeItEasy initializes itself
when its classes are first accessed. The Bootstrapper
allows users to customize the initialization process.

## What does the Bootstrapper do?

At present, the Bootstrapper provides only one service:

* `GetAssemblyFileNamesToScanForExtensions` provides a list of
  absolute paths to assemblies that should be
  [scanned for extension points](scanning-for-extension-points.md).  
  The default behavior is to return an empty list.

## How can the behavior be changed?

Provide an alternative bootstrapper class and ensure that it is loaded
in the current AppDomain before FakeItEasy is initialized (often
this means just including it in your test assembly).

The best way to provide an alternative implementation is to **extend
FakeItEasy.DefaultBootstrapper**. This class defines the default
FakeItEasy setup behavior, so using it as a base allows clients to
change only those aspects of the initialization that need to be
customized.

### An example: returning a specific extra assembly scan for extensions

Most often, FakeItEasy extension points will be defined in assemblies
that are already loaded at the time that FakeItEasy is used. In some
cases, extensions may reside in assemblies that are not (yet)
loaded. Perhaps the extensions are distributed in a shared assembly
that does not need to be referenced by any other code. The following
bootstrapper can be used to force an additional assembly to be scanned
for extension points.

```csharp
public class ScanAnExternalAssemblyBootstrapper : FakeItEasy.DefaultBootstrapper
{
    public override IEnumerable<string> GetAssemblyFilenamesToScanForExtensions()
    {
        return new [] { @"c:\full\path\to\another\assembly.dll" };
    }
}
```

## How does FakeItEasy find alternative bootstrappers?

Just before the first Bootstrapper function needs to be accessed,
FakeItEasy checks all the assemblies currently loaded in the
AppDomain. Each assembly is examined for exported types that implement
`FakeItEasy.IBootstrapper`. The first such type that is not
`FakeItEasy.DefaultBootstrapper` is instantiated and used. If no such
type is found, then `FakeItEasy.DefaultBootstrapper` is used.

**Note that there is no warning provided if FakeItEasy finds more than
one custom bootstrapper implementation. One will be chosen
non-deterministically.**
