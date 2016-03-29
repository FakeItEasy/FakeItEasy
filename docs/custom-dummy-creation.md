# Custom Dummy Creation

FakeItEasy has built-in [Dummy](dummies.md) creation rules that
provide usable non-null values to be used in tests. However, if the
default dummy creation behavior isn't adequate, you can provide your
own. Here's an example:

```csharp
class DummyBookDefinition : DummyDefinition<Book>
{
    protected override Book CreateDummy()
    {
        return new Book { Title = "Some Book", PublishedOn = new DateTime(2000, 1, 1) };
    }
}
```

### How it works

FakeItEasy uses classes that implement the following interface to
create Dummies:

```csharp
public interface IDummyDefinition
{
    Type ForType { get; }
    object CreateDummy();
}
```

When FakeItEasy tries to create a Dummy, it looks at all known
`IDummyDefinition` implementations and if one of them has a `ForType`
that matches the desired type, `CreateDummy` is used.

Although it's possible to implement `IDummyDefinition` explicitly, the
preferred approach is to extend `abstract class DummyDefinition<T>:
IDummyDefinition`, where `T` is the type of dummy to produce, as in
the example above. `DummyDefinition<T>` implements `ForType`
(returning `T`), so all that's needed is to implement the abstract
`CreateDummy` method, having it return the Dummy.

### How does FakeItEasy find the Dummy Definitions?

On initialization, FakeItEasy
[looks for Discoverable Extension Points](scanning-for-extension-points.md),
including Dummy Definitions.
