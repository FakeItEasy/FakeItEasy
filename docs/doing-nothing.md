# Doing Nothing

Sometimes you want a call to be ignored. That can be configured like so:
```csharp
A.CallTo(() => aFake.SomeMethodThatShouldDoNothing())
 .DoesNothing();
```

This is quite close to what a default Fake's unconfigured method will do, but there a few situations where you may need to make the `DoesNothing` call explicitly.

If the [Fake is strict](strict-fakes.md), an unconfigured call will throw an exception, so `DoesNothing` can be used to allow an exception.

Or, `DoesNothing` can be used to change the behavior that an already-configured call is supposed to have. For example, if a call is [set to throw an exception](throwing-exceptions.md), that can be overridden. For more on this kind of thing, see how to [override the behavior for a call](changing-behavior-between-calls.md#overriding-the-behavior-for-a-call).
