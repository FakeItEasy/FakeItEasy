# Scanning for Extension Points

On initialization, essentially as soon as a FakeItEasy type is
accessed, FakeItEasy uses reflection to look for internal and
user-supplied extension points. In most cases, there is no _need_ for
users to define any extensions, but they may be used to enhance the
power and usability of FakeItEasy.

There are currently four kinds of extension points defined:

* [Custom Dummy Creation](custom-dummy-creation.md) rules,
* [Implicit Creation Options](implicit-creation-options.md),
* [Argument Value Formatters](formatting-argument-values.md), and
* [Argument Equality Comparers](custom-argument-equality.md)

Please see their individual documentation to learn how each of these is used.

## The scanning process

On startup, FakeItEasy searches the following assemblies for classes that
implement the various extensions points:

* its own assembly,
* assemblies already loaded in the current AppDomain, if they reference
  FakeItEasy,
* assemblies referenced by assemblies from the previous bullet point, if they
  reference FakeItEasy,
* additional assemblies identified by the [Bootstrapper](bootstrapper.md)'s
  `GetAssemblyFileNamesToScanForExtensions` method, if they reference
  FakeItEasy.

Any such classes found are added to a catalogue and used at need.
