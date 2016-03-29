# Strict fakes

[By default](default-fake-behavior.md), FakeItEasy's fakes support
what is sometimes called "loose mocking". This means that calls to any
of the fake's members are allowed, even if they haven't been
configured.

However, FakeItEasy also supports strict fakes, in which all calls to
unconfigured members are rejected, throwing an
`ExpectationException`. Strict fakes are created by supplying a
[creation option](creating-fakes.md#explicit-creation-options):

```csharp
var foo = A.Fake<IFoo>(x => x.Strict());
```

After you have configured your fake in this fashion you can configure
any "allowed" calls as usual, for example:

```csharp
A.CallTo(() => foo.Bar()).Returns("bar");
```

Strict fakes are useful when it is important to ensure that no calls
are made to your fake other than the ones you are expecting.
