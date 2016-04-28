# Creating Fakes

##Natural fakes
The common way to create a fake object is by using the `A.Fake` syntax, for example:

```csharp
var foo = A.Fake<IFoo>();
```
This will return a faked object that is an actual instance of the type specified (`IFoo` in this case).

##Explicit Creation Options
When creating fakes you can, through a fluent interface, specify options for how the fake should be created:

* Specify arguments for the constructor of the faked type.
* Specify additional interfaces that the fake should implement.
* Assign additional custom attributes to the faked class.
* Cause a fake to have [strict mocking semantics](strict-fakes.md).
* Configure all of a fake's methods to [use their original implementation](calling-base-methods.md).
* Create a fake that wraps another object.
  * Specify a recorder for wrapping fakes.

Examples:

```csharp
// Specifying arguments for constructor using expression. This is refactoring friendly!
// The constructor seen here is never actually invoked. It is an expression and it's purpose
// is purely to communicate the constructor arguments which will be extracted from it
var foo = A.Fake<FooClass>(x => x.WithArgumentsForConstructor(() => new FooClass("foo", "bar")));

// Specifying arguments for constructor using IEnumerable<object>.
var foo = A.Fake<FooClass>(x => x.WithArgumentsForConstructor(new object[] { "foo", "bar" }));

// Specifying additional interfaces to be implemented. Among other uses,
// this can help when a fake skips members because they have been 
// explicitly implemented on the class being faked.
var foo = A.Fake<FooClass>(x => x.Implements(typeof(IFoo)));
// or 
var foo = A.Fake<FooClass>(x => x.Implements<IFoo>());

// Assigning custom attributes to the faked class.
// Get a parameterless constructor for our attribute and create a builder 
var constructor = typeof(FooAttribute).GetConstructor(new Type[0]);
var builder = new CustomAttributeBuilder(constructor, new object[0]);
var builders = new List<CustomAttributeBuilder>() { builder };
// foo and foo's type should both have "FooAttribute"
var foo = A.Fake<IFoo>(x => x.WithAdditionalAttributes(builders));

// Create wrapper - unconfigured calls will be forwarded to wrapped
var wrapped = new FooClass("foo", "bar");
var foo = A.Fake<IFoo>(x => x.Wrapping(wrapped));
```

##Implicit Creation Options

[Implicit creation options](implicit-creation-options.md) are
available, equivalent in power to the explicit creation options
mentioned above.

##Unnatural fakes

For those accustomed to [Moq](http://www.moqthis.com/) there is an
alternative way of creating fakes through the `new Fake<T>`
syntax. The fake provides a fluent interface for configuring the faked
object:

```csharp
var fake = new Fake<IFoo>();
fake.CallsTo(x => x.Bar("some argument")).Returns("some return value");

var foo = fake.FakeObject;
```

For an alternative look at migrating from Moq to FakeItEasy, see Daniel Marbach's blog post that talks about [Migration from Moq to FakeItEasy with Resharper Search Patterns](http://www.planetgeek.ch/2013/07/18/migration-from-moq-to-fakeiteasy-with-resharper-search-patterns/).
