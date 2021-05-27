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

## "Free-form" events using arbitrary delegates

It is also possible to raise events defined using a **custom delegate** (a.k.a
"free-form delegate"), like these:

```csharp
public delegate void FreeformEventHandler(int count);
public delegate void CustomEventHandler(object sender, CustomEventArgs e);
…
event FreeformEventHandler FreeformEvent;
event CustomEventHandler CustomEvent;
```

### From C&#x23;
To raise a free-form event from C#, use `Raise.FreeForm.With`, which automatically infers the correct delegate type:

```csharp
fake.FreeformEvent += Raise.FreeForm.With(7);
fake.CustomEvent += Raise.FreeForm.With(fake, sampleCustomEventArgs);
```

Due to language limitations, `Raise.Freeform.With` does not work in VB.NET, and it uses late binding, so you need a reference to the `Microsoft.CSharp` assembly in order to use it.

### From VB.NET
To raise a free-form event from VB.NET, you must use `Raise.FreeForm(Of TEventHandler).With`:

```vbnet
AddHandler fake.FreeformEvent, Raise.FreeForm(Of FreeformEventHandler).With(7)
AddHandler fake.CustomEvent, Raise.FreeForm(Of CustomEventHandler).With(fake, sampleCustomEventArgs)
```

Specifying the type of the event handler gets around the language restrictions in VB.NET.
This method may also be used from C# if you don't want to rely on late binding.

## Limitations

The approach described above for raising events doesn't work in some situations:

- Wrapping fakes
- Fakes configured to call base methods

This is because the calls (including event subscription and unsubscription) are
forwarded to another implementation (wrapped object or base class) that
FakeItEasy has no control over, so the fake doesn't know about the handlers and
cannot call them.

Similarly, strict fakes don't handle any call unless explicitly configured,
including event subscription or unsubscription, so FakeItEasy also can't raise
events on strict fakes.

To work around this limitation, you have two options:

- You can explicitly enable the default event behavior on the fake, for a
specific event or for all events of the fake:

```csharp
Manage.Event("FellInLove").Of(robot);
Manage.AllEvents.Of(robot);
```

- If you need more control, you can explicitly configure the calls to the event
accessors to handle event subscription yourself, as in the example below:

```csharp
// Declare a delegate to store the event handlers
EventHandler handlers = null;

// Configure event subscription on the fake
A.CallTo(robot, EventAction.Add("FellInLove")).Invokes((EventHandler h) => handlers += h);
A.CallTo(robot, EventAction.Remove("FellInLove")).Invokes((EventHandler h) => handlers -= h);

// Raise the event
handlers?.Invoke(robot, EventArgs.Empty);
```
