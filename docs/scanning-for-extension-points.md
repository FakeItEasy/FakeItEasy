# Scanning for Extension Points

On initialization, essentially as soon as a FakeItEasy type is
accessed, FakeItEasy uses reflection to look for internal and
user-supplied extension points. In most cases, there is no _need_ for
users to define any extensions, but they may be used to enhance the
power and usability of FakeItEasy.

There are currently three kinds of extension points defined:

* [Custom Dummy Creation](custom-dummy-creation.md) rules,
* Fake Configurators
* [Argument Value Formatters](formatting-argument-values.md)

Please see their individual documentation pages to learn how each of these is used.

## The scanning process

On startup, FakeItEasy searches:
* its own assembly,
* assemblies already loaded in the current AppDomain and
* additional assemblies identified by the [Bootstrapper](bootstrapper.md)'s
  `GetAssemblyFileNamesToScanForExtensions` method<sup>1</sup>
  
for classes that implement the various extensions points.
Any such classes found are added to a catalogue and used at need.

**Note: this does not apply to the Silverlight version of the DLL,
  which does not load externally-supplied extension points. Under
  Silverlight, only extensions defined in the FakeItEasy assembly are
  used.**

----
1. In FakeItEasy 1.17.0 or earlier, there was no
  [Bootstrapper](bootstrapper.md), and all DLLs in the current working
  directory were considered as sources for extension points. This lead
  to some problems, notably slow startup. If you use an old version of
  FakeItEasy and have these problems,
  [upgrade now](https://nuget.org/packages/FakeItEasy/).
  
