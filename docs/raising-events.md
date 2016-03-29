# Raising Events

Let's say - for argument's sake - that we have an interface that has
an event defined:

```csharp
public interface IRobot
{ 
    event EventHandler FellInLove;
}
```

Now in a test where we have a faked instance of this interface we can
raise that event whenever we want, specifying sender and event
args. We could also omit the sender and the fake will be passed as
sender to the event handler. There's also a convenience method for
raising with empty event args.

```csharp
var robot = A.Fake<IRobot>();
            
robot.FellInLove += (s, e) =>
    {
        Console.WriteLine("Yay!");
    };
         
// Raise the event!
robot.FellInLove += Raise.With(EventArgs.Empty);

// Use the overload for empty event args
robot.FellInLove += Raise.WithEmpty();

// Specify sender and event args explicitly:
robot.FellInLove += Raise.With(sender: robot, e: EventArgs.Empty);
```

Events of type `EventHandler<TEventArgs>` may be raised in exactly the same way. 

If an event is defined using a custom delegate, such as `delegate void
CustomEventHandler(object sender, FileSystemEventArgs eventArgs)`, it
needs to be raised using `Now`:

```csharp
robot.FoundANewFile += Raise.With(robot, new FileSystemEventArgs(…)).Now;
```

Just as when we're trying to
[ov erride a method's behavior](what-can-be-faked.md#what-members-can-be-overriden),
_for FakeItEasy to raise an event, the event must be virtual (if
defined on a class) or defined on an interface_.

## VB.Net

```
AddHandler robot.FellInLove, AddressOf Raise.With(EventArgs.Empty)
```
