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


Note that special steps will need to be taken to
[fake internal interfaces and classes](how-to-fake-internal-types.md).

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
