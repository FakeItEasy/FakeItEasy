# Changing behavior between calls

## Limited call specifications

When [specifying return values](specifying-return-values.md) or
configuring [exceptions to be thrown](throwing-exceptions.md) and so
on, it's possible to define the number of times the action can
occur. By default, omitting the number of repetitions is the same as
saying "forever", so after specifying
`A.CallTo(() =>fakeShop.Address).Returns("123 Fake Street")`,
`fakeShop.Address` will return the same value _every time it's
called. Forever._

This can be changed, though:
```csharp
A.CallTo(() => fakeShop.Address).Returns("123 Fake Street").Once();
A.CallTo(() => fakeShop.Address).Returns("123 Fake Street").Twice();
A.CallTo(() => fakeShop.Address).Returns("123 Fake Street").NumberOfTimes(17);
```

This could be useful if you want to allow a limited number of calls on
a [strict fake](strict-fakes.md), but there's a more useful application.

## Specifying different behaviors for successive calls

In some cases, you might want to specify different behaviors for successive
calls to the same method. For instance, in order to test the System Under
Test's retry logic, a Fake service could be configured to fail once and then
function properly thereafter. This can be done by chaining behaviors like this:

```csharp
// Configure the method to throw an exception once, then succeed forever
A.CallTo(() => fakeService.DoSomething())
    .Throws<Exception>().Once()
    .Then
    .Returns("SUCCESS");
```

Note that you can only use `Then` after specifying that some behavior should
only occur a limited number of times.


## Overriding the behavior for a call

Call specifications act kind of like a stack - they're pushed on the
Fake and then popped off once the number of repetitions defined for a
call have been exhausted.

Thus, it's possible to have a call to a Fake act one way, and then another. For
instance, the same effect as the previous sample can be achieved by overriding
the behavior of the fake:

```csharp
// set up an action that can run forever, unless superseded
A.CallTo(() => fakeService.DoSomething()).Returns("SUCCESS");

// set up a one-time exception which will be used for the first call
A.CallTo(() => fakeService.DoSomething()).Throws<Exception>().Once();
```

This can be useful when you are unable to use `Then` to specify a different
behavior for successive calls. For example, when you have a fake with a default
behavior (configured in a test `Setup` method or a
[`FakeOptionsBuilder`](implicit-creation-options.md)), and you need to override
this behavior for a specific test.
