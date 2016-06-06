# Implicit Creation Options

While it's possible to provide
[explicit creation options](creating-fakes.md#explicit-creation-options),
this can sometimes be tedious. Sometimes you want to have every Fake
of a particular type start with some basic configuration, using a
`FakeOptionsBuilder`. Here's an example:

```csharp
public class RobotRunsAmokEventFakeOptionsBuilder : FakeOptionsBuilder<RobotRunsAmokEvent>
{
    protected override void BuildOptions(IFakeOptions<RobotRunsAmokEvent> options)
    {
        options.ConfigureFake(fake =>
        {
            A.CallTo(() => fake.CalculateTimestamp())
                .Returns(new DateTime(1997, 8, 29, 2, 14, 03));
            robotRunsAmokEvent.ID = Guid.NewGuid();
        });
    }
}
```

This will ensure that any new `RobotRunsAmokEventFakeOptionsBuilder`
will have an
[appropriate date](https://en.wikipedia.org/wiki/Skynet_(Terminator)#Before_Judgment_Day)
applied and will have a unique ID.

In addition to `ConfigureFake`, any
[explicit creation option](creating-fakes.md#explicit-creation-options)
can be used in `BuildOptions`, including implementing interfaces,
providing constructor arguments, and more.

### How it works

FakeItEasy uses classes that implement the following interface to configure Fakes:

```csharp
public interface IFakeOptionsBuilder
{
    bool CanBuildOptionsForFakeOfType(Type type);
    void BuildOptions(Type typeOfFake, IFakeOptions options);
    Priority Priority { get; }
}
```

When FakeItEasy creates a Fake, it looks at all known
`IFakeOptionsBuilder` implementations for which
`CanBuildOptionsForFakeOfType` returns `true`. Then it passes an empty
`options` object to `BuildOptions`. If multiple implementations match,
the one with the highest `Priority`is used.

If all that's needed is a Fake Options Builder that configures a
single explicit type, extending `abstract class FakeOptionsBuilder<T>:
IFakeOptionsBuilder` is preferred, as was done above. This abstract
class provides default implementations of `Priority` and
`CanBuildOptionsForFakeOfType` (although the `Priority` can be
overridden if needed). If you want to configure a vaierty of Fake
types, you may prefer to extend `IFakeOptionsBuilder` directly. For
example, if you wanted all Fakes to be Strict, you might write
something like this:

```csharp
class MakeEverythingStrictOptionsBuilder : IFakeOptionsBuilder
{
    public bool CanBuildOptionsForFakeOfType(Type type)
    {
        return true;
    }

    public void BuildOptions(Type typeOfFake, IFakeOptions options)
    {
        options.Strict();
    }

    public Priority Priority
    {
        get { return Priority.Default; } // equivalent to value 0
    }
}
```

This method provides additional power, in that the Fake Options
Builder can be applied to more types, but it sacrifices compile-time
typesafety.  Of course, it's possible to perform more sophisticated
analysis on the types, perhaps having `CanBuildOptionsForFakeOfType`
accept only types whose name match a pattern. In this way,
conventions-based faking could be accomplished.

Note that once the type of Fake being created is identified, say as
`FakedType`, it's possible to cast `options` to a
`IFakeOptions<FakedType>` and operate on it, but the `FakedType` must
be the _exact_ type being faked, not just something in the inheritance
tree.

### How does FakeItEasy find the Fake Options Builders?

On initialization, FakeItEasy
[looks for Discoverable Extension Points](scanning-for-extension-points.md),
including Fake Options Builders.
