# Invoking Custom Code

Sometimes a faked method's desired behavior can't be satisfactorily
defined just by
[specifying return values](specifying-return-values.md),
[throwing exceptions](throwing-exceptions.md),
[assigning out and ref parameters](assigning-out-and-ref-parameters.md)
or even [doing nothing](doing-nothing.md). Maybe you need to simulate
some kind of side effect, either for the benefit of the System Under
Test or to make writing a test easier (or possible). Let's see what
that's like.

```csharp
A.CallTo(() => fakeShop.SellSmarties())
 .Invokes(() => OrderMoreSmarties()) // simulate Smarties stock falling too low
 .Returns(20);
```

Now when the System Under Test calls `SellSmarties`, the Fake will
call `OrderMoreSmarties`.

If the method being configured has a return value, it will continue to return the
[default value for an unconfigured fake](default-fake-behavior.md#overrideable-members-are-faked)
unless you override it with `Returns` or `ReturnsLazily`.

There are also more advanced variants that can invoke actions based on
arguments supplied to the faked method. These act similarly to how you
specify return values that are calculated at call time. For example

```csharp
// Pass up to 4 original call argument values into the callback method.
A.CallTo(() => fakeShop.NumberOfSweetsSoldOn(A<DateTime>._))
 .Invokes((DateTime when) => System.Console.Out.WriteLine("showing sweet sales for " + when))
 .Returns(17);

// Pass an IFakeObjectCall into the callback for more advanced scenarios,
// including configuring methods that have more than 4 parameters.
A.CallTo(() => fakeShop.NumberOfSweetsSoldOn(A<DateTime>._))
 .Invokes(callObject => System.Console.Out.WriteLine(callObject.FakedObject +
                                                     " is closed on " +
                                                     callObject.Arguments[0]));
```
