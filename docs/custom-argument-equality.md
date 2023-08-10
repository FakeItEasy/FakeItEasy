# Custom argument equality

## Default behavior when comparing argument values

By default, FakeItEasy compares argument values using `Object.Equals`. For
instance, consider this call configuration:

```csharp
A.CallTo(() => fake.DoSomething("hello")).Returns(42);
```

When comparing the argument value from the actual call with the configured
value `"hello"`, the values are compared with the default comparison rules
(using `String.Equals` in this case).

In most cases, that's what you want, but there are scenarios where it can be
inconvenient. For example, you might want to compare instances based on the
values of their properties, but by default the objects are compared using
reference equality. If you don't own the type, you can't override `Equals` to
implement the desired behavior. So you end up having to compare the properties
explicitly in an
[argument constraint](argument-constraints.md#custom-matching):

```csharp
Foo expectedFoo = ...;
A.CallTo(() =>
    fake.DoSomethingElse(A<Foo>.That.Matches(foo => foo.Bar == expectedFoo.Bar)))
    .Returns(42);
```

This is quite verbose, and not very readable. Ideally you would write it like
this:

```csharp
Foo expectedFoo = ...;
A.CallTo(() => fake.DoSomethingElse(expectedFoo)).Returns(42);
```

And it would just do the right thing.

FakeItEasy offers a way to override the default behavior by providing a custom
argument equality comparer.

## Writing a custom argument equality comparer

Just define a class that inherits `ArgumentEqualityComparer<T>`, and override
the `AreEqual` method:

```csharp
public class FooComparer : ArgumentEqualityComparer<Foo>
{
    protected override bool AreEqual(Foo expectedValue, Foo argumentValue)
    {
        return expectedValue.Bar == argumentValue.Bar;
    }
}
```

FakeItEasy will automatically discover this class and use it to compare
instances of `Foo`.

## How it works

FakeItEasy uses classes that implement the following interface to compare argument values:

```csharp
public interface IArgumentEqualityComparer
{
    bool CanCompare(Type type);
    bool AreEqual(object expectedValue, object argumentValue);
    Priority Priority { get; }
}
```

When FakeItEasy needs to compare a non-null argument value with a non-null expected value,
it looks at all known `IArgumentEqualityComparer` implementations for which
`CanCompare` returns true for the parameter type. If multiple implementations
match, the one with the highest `Priority` is used.

If all that's needed is an Argument Equality Comparer that specifies how to
compare instances of a specific type,  extending `abstract class
ArgumentEqualityComparer<T>: IArgumentEqualityComparer` is preferred. It
provides default implementations of `Priority` and `CanCompare` (although
they can be overridden if needed).

However, if you want to provide custom equality comparison for a variety of
types, you may prefer to implement `IArgumentEqualityComparer` directly. For
example, if you wanted all types in a specific namespace to be compared by
their string representation, you might write something like this:

```csharp
class ToStringArgumentEqualityComparer : IArgumentEqualityComparer
{
    public bool CanCompare(Type type) => type.Namespace == "MySpecialNamespace";

    public bool AreEqual(object expectedValue, object argumentValue)
    {
        return expectedValue.ToString() == argumentValue.ToString();
    }

    public Priority Priority => Priority.Default;
}
```

## How does FakeItEasy find Argument Equality Comparers?

On initialization, FakeItEasy
[looks for Discoverable Extension Points](scanning-for-extension-points.md),
including Argument Equality Comparers.
