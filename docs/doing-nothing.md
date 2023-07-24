# Doing Nothing

Sometimes you want a call to be ignored. That can be configured like so:
```csharp
A.CallTo(() => aFake.SomeVoidMethodThatShouldDoNothing())
 .DoesNothing();
```

This is quite close to what a default Fake's
[unconfigured member will do](default-fake-behavior.md#overrideable-members-are-faked),
but there a few situations where you may need to make the `DoesNothing` call
explicitly.

If the [Fake is strict](strict-fakes.md), an unconfigured call will throw an exception, so `DoesNothing` can be used to allow a call to a member on the Fake.

Or, `DoesNothing` can be used to change the behavior that an already-configured call is supposed to have. For example, if a call is [set to throw an exception](throwing-exceptions.md), that can be overridden. For more on this kind of thing, see how to [override the behavior for a call](changing-behavior-between-calls.md#overriding-the-behavior-for-a-call).

**Note that `DoesNothing` is only applicable when configuring members that have a
`void` return (or `Task`, which is the async equivalent of `void`).** To override
behavior on already-configured (or strict) members that return values, you must
instead [configure a preferred return value](specifying-return-values.md).
