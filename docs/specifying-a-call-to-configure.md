# Specifying a Call to Configure

One of the first steps in configuring a fake object's behavior is to
specify which call to configure. Like most FakeItEasy actions, this is
done using a method on the `A` class: `A.CallTo`.

## Specifying a method call or property `get` using an Expression

```csharp
A.CallTo(() => fakeShop.GetTopSellingCandy()).Returns(lollipop);
A.CallTo(() => fakeShop.Address).Returns("123 Fake Street");
```

The expressions in the above example are not evaluated by FakeItEasy:
no call to `GetTopSellingCandy` or `Address` is made. The expressions
are just used to identify which call to configure, after which
`A.CallTo` returns an object that can be used to specify how the fake
should behave when the call is made.

???+ note "Use an action to complete call configuration"
    Specifying the call via `A.CallTo` and related methods does not
    have any effect on the fake object. You must include an action
    for the call to perform to alter the behavior of the fake object.

    There are many types of actions that can be specified, including
    [returning various values](specifying-return-values.md),
    [throwing exceptions](throwing-exceptions.md), and more. Even
    [doing nothing](doing-nothing.md), which may be required for a
    void method.

## Specifying a call to a property setter

Assignment operators can't be used in lambda expressions, so the
`A.CallTo` overloads described above cannot be used to configure calls
to property setters.
Use `A.CallToSet` to configure the `set` behavior of read/write properties:

```csharp
A.CallToSet(() => fakeShop.Address).To("123 Fake Street").CallsBaseMethod();
A.CallToSet(() => fakeShop.Address).To(() => A<string>.That.StartsWith("123")).DoesNothing();
A.CallToSet(() => fakeShop.Address).DoesNothing(); // ignores the value that's set
```

[Argument constraints](argument-constraints.md) can be used to
constrain the value that's set into the property, or the indexes that
must be supplied when invoking an indexer.

Note that any customization of a read/write property's behavior will break the
default behavior of having
[the getter return the last set value](default-fake-behavior.md#readwrite-properties).
To avoid this, a
[custom action](invoking-custom-code.md#case-study-customizing-a-readwrite-property)
may be used to preserve the behavior.

## Specifying the invocation of a delegate

To specify the invocation of a delegate, just use `A.CallTo`, invoking the fake delegate as you normally would:

```csharp
var deepThought = A.Fake<Func<string, int>>();
A.CallTo(() => deepThought.Invoke("What is the answer to life, the universe, and everything?")).Returns(42);

// Note that the .Invoke part is optional:
A.CallTo(() => deepThought("What is the answer to life, the universe, and everything?")).Returns(42);
```

## Specifying a call to an explicitly implemented interface member

An explicitly implemented member is not directly visible on the concrete class. Instead, it has to be called
(or overridden) via the interface. In addition, since fakes don't automatically intercept explicitly implemented
interfaces, you need to explicitly specify that the fake implements the interface:

```csharp
var fakeShop = A.Fake<CandyShop>(options => options.Implements<ICandyShop>());
A.CallTo(() => ((ICandyShop)fakeShop).GetTopSellingCandy()).Returns(lollipop);
```

## Specifying a call to any method or property

Instead of supplying an expression to identify a specific method, pass
the fake to `A.CallTo` to refer to any method on the fake:

```csharp
A.CallTo(fakeShop).Throws(new Exception());

// Or limit the calls to void methods
A.CallTo(fakeShop).WithVoidReturnType().Throws("sugar overflow");

// Or limit the calls by return type
A.CallTo(fakeShop).WithReturnType<string>().Returns("sugar tastes good");

// Or limit the calls to methods that return a value. Note that it will throw at runtime
// if the configured return value doesn't match the called method's return type.
A.CallTo(fakeShop).WithNonVoidReturnType().Returns("sugar tastes good");

// Or create a sophisticated test with a predicate that acts on an IFakeObjectCall.
// One use case is when the call's arguments include an object of an anonymous type,
// since it's impossible to create argument constraints for anonymous types.
A.CallTo(fakeShop).Where(call => call.Method.Name == "AnonymousArgEater")
                  .WithReturnType<string>()
                  .Returns("no-name candy");
```

`A.CallTo(object)` can also be used to specify write-only properties and
`protected` members:

```csharp
A.CallTo(fakeShop).Where(call => call.Method.Name == "ProtectedCalculateSalesForToday")
                  .WithReturnType<double>()
                  .Returns(4741.71);

// Use the conventional .NET prefix "get_" to refer to a property's getter:
A.CallTo(fakeShop).Where(call => call.Method.Name == "get_Address")
                  .WithReturnType<string>()
                  .Returns("123 Fake Street");

// Use the conventional .NET prefix "set_" to refer to a property's setter:
A.CallTo(fakeShop).Where(call => call.Method.Name == "set_Address")
                  .Throws(new Exception("we can't move"));
```

## Specifying a call to an event accessor

Although calls to event accessors can be specified using the approach described
in the previous section, FakeItEasy also provides helper methods to make this
easier:

```csharp
// Specifies a call to the add accessor of the MyEvent event of the fake
A.CallTo(fake, EventAction.Add("MyEvent")).Invokes((EventHandler h) => ...);
// Specifies a call to the remove accessor of the MyEvent event of the fake
A.CallTo(fake, EventAction.Remove("MyEvent")).Invokes((EventHandler h) => ...);
// Specifies a call to the add accessor of any event of the fake
A.CallTo(fake, EventAction.Add()).Invokes(...);
// Specifies a call to the remove accessor of any event of the fake
A.CallTo(fake, EventAction.Remove()).Invokes(...);
```

## VB.Net
Special syntax is provided to specify `Func`s and `Sub`s in VB, using their respective keywords:

```
A.CallTo(Sub() fakeShop.SellSomething())
                       .DoesNothing()

A.CallTo(Func() fakeShop.GetTopSellingCandy())
                        .Returns(lollipop)
```
