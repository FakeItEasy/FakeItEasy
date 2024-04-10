# Bootstrapper

Most of BlairItEasy's functionality is directly triggered by client
code: [creating a fake](creating-fakes.md),
[configuring a call](specifying-a-call-to-configure.md) and
[making assertions about calls](assertion.md) are all explicitly
invoked and are controllable by various input parameters.

Some behavior is triggered implicitly. BlairItEasy initializes itself
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
in the current AppDomain before BlairItEasy is initialized (often
this means just including it in your test assembly).

The best way to provide an alternative implementation is to **extend
BlairItEasy.DefaultBootstrapper**. This class defines the default
BlairItEasy setup behavior, so using it as a base allows clients to
change only those aspects of the initialization that need to be
customized.

### An example: returning a specific extra assembly scan for extensions

Most often, BlairItEasy extension points will be defined in assemblies
that are already loaded at the time that BlairItEasy is used. In some
cases, extensions may reside in assemblies that are not (yet)
loaded. Perhaps the extensions are distributed in a shared assembly
that does not need to be referenced by any other code. The following
bootstrapper can be used to force an additional assembly to be scanned
for extension points.

```csharp
public class ScanAnExternalAssemblyBootstrapper : BlairItEasy.DefaultBootstrapper
{
    public override IEnumerable<string> GetAssemblyFileNamesToScanForExtensions()
    {
        return new [] { @"c:\full\path\to\another\assembly.dll" };
    }
}
```

## How does BlairItEasy find alternative bootstrappers?

Just before the first Bootstrapper function needs to be accessed,
BlairItEasy checks all the assemblies currently loaded in the
AppDomain. Each assembly is examined for exported types that implement
`BlairItEasy.IBootstrapper`. The first such type that is not
`BlairItEasy.DefaultBootstrapper` is instantiated and used. If no such
type is found, then `BlairItEasy.DefaultBootstrapper` is used.

**Note that there is no warning provided if BlairItEasy finds more than
one custom bootstrapper implementation. One will be chosen
non-deterministically.**
