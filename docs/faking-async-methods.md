# Faking async methods

The faking of `async` methods is fully supported in FakeItEasy.

```csharp
public class Foo
{
    public virtual async Task<string> Bar()
    {
        // await something...
    }
}
```

A call to a non-configured async method on a fake will return a
[Dummy](dummies.md#how-are-the-dummies-made) `Task` or `Task<T>`, just
as if it were any other method that returns a `Task` or `Task<T>`. For
example:

```csharp
var foo = A.Fake<Foo>();
var bar = await foo.Bar(); // will return immediately and return string.Empty
```

Of course, you can still configure calls to `async` methods as you would normally:

```csharp
A.CallTo(() => foo.Bar()).Returns(Task.FromResult("bar"));
```
