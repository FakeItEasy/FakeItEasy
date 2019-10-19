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

There are also convenience overloads of `Returns` and `ReturnsLazily` that let you specify a value rather
than a task, and configure the method to return a completed task whose result is the specified value:

```csharp
A.CallTo(() => foo.Bar()).Returns("bar");
```

These overloads of `Returns` and `ReturnsLazily` also exist for `ValueTask<T>`. If your test project
targets a framework compatible with .NET Standard 2.1 or higher, they're built into FakeItEasy itself;
otherwise, they're in a separate package:
[`FakeItEasy.Extensions.ValueTask`](https://www.nuget.org/packages/FakeItEasy.Extensions.ValueTask).

## Throwing exceptions

To configure an async method to throw an exception, see
[Throwing exceptions from an async method](throwing-exceptions.md#throwing-exceptions-from-an-async-method).
