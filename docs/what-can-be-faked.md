# What can be faked

## What types can be faked?

FakeItEasy uses
[Castle DynamicProxy](http://www.castleproject.org/projects/dynamicproxy/)
to create fakes. Thus, it can fake just about anything that could
normally be overridden, extended, or implemented.  This means that the
following entities can be faked:

* interfaces
* classes that
    * are not sealed,
    * are not static, and
    * have at least one public or protected constructor whose arguments FakeItEasy can construct or obtain
* delegates

Note that special steps will need to be taken to
[fake internal interfaces and classes](how-to-fake-internal-types.md).

### Types whose methods have `in` parameters

Currently, types that contain methods having a parameter modified by the `in` keyword cannot be faked by the .NET Standard version of the FakeItEasy library.

If the type is a generic type, no FakeItEasy library can fake it.

These limitations are tracked as [issue 1338](https://github.com/FakeItEasy/FakeItEasy/issues/1338).

### Where do the constructor arguments come from?
  
* they can be supplied via `WithArgumentsForConstructor` as shown in
  [creating fakes](creating-fakes.md), or
* FakeItEasy will use [dummies](dummies.md) as arguments

## What members can be overridden?

Once a fake has been constructed, its methods and properties can be
overridden if they are:

* virtual,
* abstract, or
* an interface method when an interface is being faked

Note that this means that static members, including extension methods,
**cannot** be overridden.

### Methods with reference return values

Methods with [reference return values](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/ref-returns#what-is-a-reference-return-value) can not be invoked on a Fake. Any attempt to do so will result in a `NullReferenceException` being thrown. 