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

## Object members

It can sometimes be inconvenient that *all* methods throw an exception
if not configured. You might want to allow calls to methods inherited
from `System.Object` (`Equals`, `GetHashCode` and `ToString`), because
they're used all the time, often implicitly, and in most cases there's
no real value in configuring them manually.

To achieve this, pass a `StrictFakeOptions` value to the `Strict`
method when you create the fake:

```csharp
// Allow calls to all object methods
var foo = A.Fake<IFoo>(x => x.Strict(StrictFakeOptions.AllowObjectMethods));

// Allow calls to ToString
var foo = A.Fake<IFoo>(x => x.Strict(StrictFakeOptions.AllowToString));

// Allow calls to Equals and GetHashCode
var foo = A.Fake<IFoo>(x => x.Strict(StrictFakeOptions.AllowEquals | StrictFakeOptions.AllowGetHashCode));
```

## Events

By default, calls to event accessors of a strict fake will fail if the
calls are not configured. Although you can [manually handle event subscription
or unsubscription](raising-events.md#limitations), there's often not much value
in doing this manually. You can allow a strict fake to manage events
automatically by passing the `AllowEvents` flag to the `Strict` method:

```csharp
var foo = A.Fake<IFoo>(x => x.Strict(StrictFakeOptions.AllowEvents));
```
