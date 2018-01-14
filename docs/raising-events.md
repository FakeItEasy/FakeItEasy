# Raising events

FakeItEasy can be used to simulate the raising of an event from a Fake object, assuming the event is virtual or abstract, or defined on an interface.

## `EventHandler`-based events

Suppose a standard `EventHandler`-based event such as this one:

```csharp
public interface IRobot
{
    event EventHandler FellInLove;
}
```

You can raise that event, specifying sender and event
arguments. You could also omit the sender and the Fake will be passed as
sender to the event handler, and there's also a convenience method for
raising with empty event arguments:

```csharp
var robot = A.Fake<IRobot>();

// Somehow use the fake from the code being tested

// Raise the event!
robot.FellInLove += Raise.With(someEventArgs); // the "sender" will be robot

// Use the overload for empty event args. Sender will be robot, args will be EventArgs.Empty
robot.FellInLove += Raise.WithEmpty();

// Specify sender and event args explicitly:
robot.FellInLove += Raise.With(sender: robot, e: someEventArgs);
```

### VB.NET syntax

```vbnet
' Raise the event!
AddHandler robot.FellInLove, Raise.With(someEventArgs) ' the "sender" will be robot

' Use the overload for empty event args. Sender will be robot, args will be EventArgs.Empty
AddHandler robot.FellInLove, Raise.WithEmpty()

' Specify sender and event args explicitly:
AddHandler robot.FellInLove, Raise.With(sender, someEventArgs)
```

## Raising `EventHandler<TEventArgs>`

Events of type `EventHandler<TEventArgs>` can be raised in exactly the same way.

Note that for the .NET 4.0 version of the FakeItEasy library, `TEventArgs` needs to extend `EventArgs` to support the above syntax. Otherwise, the event must be raised as if it were a "free-form" event as described below.

## "Free-form" events using arbitrary delegates

It is also possible to raise events defined using a **custom delegate** (a.k.a
"free-form delegate"), like these:

```csharp
public delegate void FreeformEventHandler(int count);
public delegate void CustomEventHandler(object sender, CustomEventArgs e); // considered a custom
                                                                           // delegate in .NET 4.0
…
event FreeformEventHandler FreeformEvent;
event CustomEventHandler CustomEvent;
```

### From C#
To raise a free-form event from C#, use `Raise.FreeForm.With`, which automatically infers the correct delegate type:

```csharp
fake.FreeformEvent += Raise.FreeForm.With(7);
fake.CustomEvent += Raise.FreeForm.With(fake, sampleCustomEventArgs);
```

Due to language limitations, `Raise.Freeform.With` does not work in VB.NET, and it uses late binding, so you need a reference to the `Microsoft.CSharp` assembly in order to use it.

### From VB.NET
To raise a free-form event from VB.NET, you must use `Raise.FreeForm<TEventHandler>.With`:

```csharp
fake.FreeformEvent += Raise.FreeForm<FreeformEventHandler>.With(7);
fake.CustomEvent += Raise.FreeForm<CustomEventHandler>.With(fake, sampleCustomEventArgs);
```

Specifying the type of the event handler gets around the language restrictions in VB.NET.
This method may also be used from C# if you don't want to rely on late binding.
