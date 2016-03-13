# Limited Call Specifications

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
a [strict fake](strict-fakes.md), but there's a more useful
application.

##Changing behavior between calls

Call specifications act kind of like a stack - they're pushed on the
Fake and then popped off once the number of repetitions defined for a
call have been exhausted. Thus, it's possible to have a call to a Fake
act one way, and then another. In order to test the System Under
Test's retry logic, a Fake service could be configured to fail once
and then function properly thereafter:

```csharp
// set up an action that can run forever, unless superseded
A.CallTo(() => fakeService.DoSomething()).Returns("SUCCESS");

// set up a one-time exception which will be used for the first call
A.CallTo(() => fakeService.DoSomething()).Throws<Exception>().Once;
```
