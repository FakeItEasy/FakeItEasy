# Specifying a Call to Configure

One of the first steps in configuring a fake object's behavior is to
specify which call to configure. Like most FakeItEasy actions, this is
done using a method on the `A` class: `A.CallTo`.

## Specifying a method call or property `get` using an Expression

```csharp
A.CallTo(() => fakeShop.GetTopSellingCandy())
A.CallTo(() => fakeShop.Address)
```

The expressions in the above example are not evaluated by FakeItEasy:
no call to `GetTopSellingCandy` or `Address` is made. The expressions
are just used to identify which call to configure.

`A.CallTo` returns an object that can be used to specify how the fake
should behave when the call is made. For example:

```csharp
A.CallTo(() => fakeShop.GetTopSellingCandy())
                       .Returns(lollipop);
```

Many types of actions can be specified, including
[returning various values](specifying-return-values.md),
[throwing exceptions](throwing-exceptions.md), and more.

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

## Specifying the invocation of a delegate

To specify the invocation of a delegate, just use `A.CallTo`, invoking the fake delegate as you normally would:

```csharp
var deepThought = A.Fake<Func<string, int>>();
A.CallTo(() => deepThought.Invoke("What is the answer to life, the universe, and everything?")).Returns(42);

// Note that the .Invoke part is optional:
A.CallTo(() => deepThought("What is the answer to life, the universe, and everything?")).Returns(42);
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

// Or create a sophisticated test with a predicate that acts on an IFakeObjectCall
A.CallTo(fakeShop).Where(call => call.Arguments.Count > 4)
                  .Throws(new Exception("too many arguments is bad");
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

## VB.Net
Special syntax is provided to specify `Func`s and `Sub`s in VB, using their respective keywords:

```
A.CallTo(Sub() fakeShop.SellSomething())
                       .DoesNothing()

A.CallTo(Func() fakeShop.GetTopSellingCandy())
                        .Returns(lollipop)
```
