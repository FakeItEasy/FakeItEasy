# Specifying Return Values

One of the most common tasks on a
[newly-created Fake](creating-fakes.md) is to specify the return value
for some method or property that might be called on it. This is often
done by using the `Returns` method on the result of an `A.CallTo`:

```csharp
A.CallTo(() => fakeShop.GetTopSellingCandy()).Returns(lollipop);
```

Now, whenever the parameterless method `GetTopSellingCandy` is called
on the `fakeShop` Fake, it will return the `lollipop` object.

A `get` property on a Fake can be configured similarly:
```csharp
A.CallTo(() => fakeShop.Address).Returns("123 Fake Street");
```

##Return Values Calculated at Call Time

Sometimes a desired return value won't be known at the time the call
is configured. `ReturnsNextFromSequence` and `ReturnsLazily` can help
with that. `ReturnsNextFromSequence` is the simpler of the two:

```csharp
A.CallTo(() => fakeShop.SellSweetFromShelf())
                       .ReturnsNextFromSequence(lollipop, smarties, wineGums);
```

will first return `lollipop`, then `smarties`, then `wineGums`. The
next call will not take an item from the sequence, but will rely on
other configured (or default) behavior.

On to the very powerful `ReturnsLazily`:

```csharp
// Returns the number of times the method has been called
int sweetsSold = 0;
A.CallTo(() => fakeShop.NumberOfSweetsSoldToday()).ReturnsLazily(() => ++sweetsSold);
```

If a return value depends on input to the method, those values can be
incorporated in the calculation. Convenient overloads exist for
methods of up to four parameters.

```csharp
A.CallTo(() => fakeShop.NumberOfSweetsSoldOn(A<DateTime>.Ignored)) 
                       .ReturnsLazily((DateTime theDate) => 
                                          theDate.DayOfWeek == DayOfWeek.Sunday ? 0 : 200);
```

The convenience methods may be used with methods that take `out` and
`ref` parameters. This means that the previous example would work even
if `NumberOfSweetsSoldOn` took an `out DateTime` or a `ref DateTime`.

Note that the type of the `Func` sent to `ReturnsLazily` isn't checked
at compile time, but any type mismatch will trigger a helpful error
message.

If more advanced decision-making is required, or the method has more
than 4 parameters, the convenience methods won't work. Use the variant
that takes an `IFakeObjectCall` instead:

```charp
A.CallTo(() => fakeShop.SomeCall(…))
                       .ReturnsLazily(objectCall => calculateReturnFrom(objectCall));
```

The `IFakeObjectCall` object provides access to

* information about the `Method` being called, as a `MethodInfo`,
* the `Arguments`, accessed by position or name, and
* the original `FakedObject`
