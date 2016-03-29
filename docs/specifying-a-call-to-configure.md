# Specifying a Call to Configure

One of the first steps in configuring a fake object's behaviour is to
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

## Specifying a call to any method or property

Instead of supplying an expression to identify a specific method, pass
the fake to `A.CallTo` to refer to any method on the fake:

```csharp
A.CallTo(fakeShop).Throws(new Exception());

// Or limit the calls by return type
A.CallTo(fakeShop).WithReturnType<string>().Returns("sugar tastes good");

// Or create a sophisticated test with a predicate that acts on an IFakeObjectCall
A.CallTo(fakeShop).Where(call => call.Arguments.Count > 4)
                  .Throws(new Exception("too many arguments is bad");
```

`A.CallTo(object)` can also be used to specify property `set`s and
`protected` members:

```csharp
A.CallTo(fakeShop).Where(call => call.Method.Name == "ProtectedCalculateSalesForToday")
                  .WithReturnType<double>()
                  .Returns(4741.71);

// refers to the Address property's setter
A.CallTo(fakeShop).Where(call => call.Method.Name == "set_Address")
                  .Throws(new Exception("we can't move");
```

[Issue 175](https://github.com/FakeItEasy/FakeItEasy/issues/175) has
been raised to develop a better mechanism for specifying property
`set`s.

## Specifying a call by example
```csharp
NextCall.To(fakeShop).WithAnyArguments()
                     .Throws(new Exception("we're closed");
fakeShop.SellThisCandy(null); // recorded, and configuration above is applied

...

fakeShop.SellThisCandy(lollipop); // will throw now
```

## VB.Net
Special syntax is provided to specify `Func`s and `Sub`s in VB, using their respective keywords:

```
A.CallTo(Sub() fakeShop.SellSomething())
                       .DoesNothing()

A.CallTo(Func() fakeShop.GetTopSellingCandy())
                        .Returns(lollipop)
```
