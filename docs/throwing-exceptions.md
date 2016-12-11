# Throwing Exceptions

When it's deployed, you may not want code to throw exceptions, but
often it's necessary to test what happens when libraries your code
interacts with throw them. You can configure a Fake to throw an
exception like this:

```csharp
A.CallTo(() => fakeShop.NumberOfSweetsSoldOn(DateTime.MaxValue))
 .Throws(new InvalidDateException("the date is in the future"));
```

If the exception type has a parameterless constructor, you can use it
like

```csharp
A.CallTo(() => fakeShop.NumberOfSweetsSoldOn(DateTime.MaxValue))
 .Throws<InvalidDateException>();
```

There are also more advanced methods that can throw exceptions based
on values calculated at runtime. These act similarly to how you
[specify return values that are calculated at call time](specifying-return-values.md#return-values-calculated-at-call-time). For
example

```csharp
// Generate the exception at call time.
A.CallTo(() => fakeShop.NumberOfSweetsSoldOn(A<DateTime>._))
 .Throws(() => new InvalidDateException(DateTime.UtcNow + " is in the future"));

// Pass up to 4 original call argument values into the method that creates the exception.
A.CallTo(() => fakeShop.NumberOfSweetsSoldOn(A<DateTime>._))
 .Throws((DateTime when)=>new InvalidDateException(when + " is in the future"));

// Pass an IFakeObjectCall into the creation method for more advanced scenarios.
A.CallTo(() => fakeShop.NumberOfSweetsSoldOn(A<DateTime>._))
 .Throws(callObject => new InvalidDateException(callObject.FakedObject +
                                                " is closed on " +
                                                callObject.Arguments[0]));
```

## Throwing exceptions from an async method

When a method returns a `Task` or `Task<T>`, there are two ways it can indicate
failure via an exception:

- throw the exception synchronously, i.e. not actually return a `Task`
- "throw asynchronously", i.e. return a failed task with the exception.

The former is supported by the `Throws` method described above, in the same way as if the
method was synchronous. The latter can be configured by using the `ThrowsAsync` method:

```csharp
A.CallTo(() => fakeShop.OrderSweetsAsync("cheeseburger"))
 .ThrowsAsync(new ArgumentException("'cheeseburger' isn't a valid sweet category"));
```

This will cause the configured method to return a failed `Task` whose `Exception` property
is set to the exception specified in `ThrowsAsync`.
