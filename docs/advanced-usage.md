# Advanced usage

FakeItEasy exposes a few APIs that aren't commonly needed, but can be useful in certain scenarios.

## Restoring a fake's configuration to its initial state.

It's generally better to discard a fake and create a new one than to
"unconfigure" a fake so it has the behavior it did when first created.
If this is not feasible, consider one of the following approaches.

To alter the behavior of a fake in the middle of a test, it's usually best to
explicitly indicate how you want the behavior to change. See
[Changing behavior between calls](changing-behavior-between-calls.md).

If you wish to restore the configuration between tests, but the fake cannot be
replaced, perhaps because it is held by a dependency injection container, use
`Fake.ResetToInitialConfiguration`, which will restore the fake to its initial
configuration, preserving

* [strictness](strict-fakes.md),
* [object wrapping](calling-wrapped-methods.md),
* [redirecting to base methods](calling-base-methods.md), and
* other [explicit](creating-fakes.md#explicit-creation-options) or
  [implicit](implicit-creation-options.md) creation options.

`Fake.ResetToInitialConfiguration` will also restore
[read/write property](default-fake-behavior.md#readwrite-properties) values
to those set during the fake's creation.

## Clearing a fake's recorded calls

The `Fake.ClearRecordedCalls` method clears all recorded calls from a fake. Subsequent call assertions won't see these calls.

```csharp
var foo = A.Fake<IFoo>();
foo.Bar();
A.CallTo(() => foo.Bar()).MustHaveHappened();
Fake.ClearRecordedCalls(foo);
A.CallTo(() => foo.Bar()).MustNotHaveHappened();
```

## Getting the list of calls made on a fake

Sometimes, the call assertion API offered by FakeItEasy isn't enough, and you need to manually check the calls made on a fake.
The `Fake.GetCalls` method returns a fake's recorded calls as a list of `ICompletedFakeObjectCall` objects that you can examine yourself:

```csharp
var foo = A.Fake<IFoo>();
foo.Bar();
foo.Baz();
var calls = Fake.GetCalls(foo).ToList();
Assert.Equal(2, calls.Count);
Assert.Equal("Bar", calls[0].Method.Name);
Assert.Equal("Baz", calls[1].Method.Name);
```

## The `FakeManager` object

The `Fake.GetFakeManager` method returns a `FakeManager` object that can be used to get information on the fake and manipulate its call rules.

`Fake.GetFakeManager` throws an exception if the provided object is not a fake. To test if an object is a fake you can call `Fake.IsFake`
or try get the `FakeManager` with `Fake.TryGetFakeManager` which will return true if the provided object is a fake and also give you the `FakeManager`
for that object via an out parameter.

### Getting the type of the fake

The `FakeManager.FakeObjectType` property returns the type of the fake, i.e. the type that was passed to `A.Fake`. This can be useful
if you're writing code that dynamically manipulates fakes.

```csharp
var foo = A.Fake<IFoo>();
var manager = Fake.GetFakeManager(foo);
Assert.Equal(typeof(IFoo), manager.FakeObjectType);
```

### Getting the fake from the fake manager

The `FakeManager.Object` property returns the fake object managed by this `FakeManager`.

```csharp
var foo = A.Fake<IFoo>();
var manager = Fake.GetFakeManager(foo);
Assert.Equal(foo, manager.Object);
```

### Manipulating a fake's call rules

The `FakeManager.Rules` property returns all the rules configured on a fake.

```csharp
var foo = A.Fake<IFoo>();
A.CallTo(() => foo.Bar()).Returns(42);
A.CallTo(() => foo.Baz()).DoesNothing();
var manager = Fake.GetFakeManager(foo);
var rules = manager.Rules.ToList();
Assert.Equal(2, rules.Count);
```

It is also possible to add custom rules for advanced scenarios, using `AddRuleFirst` and `AddRuleLast`. `AddRuleFirst` adds a rule at the beginning of the rule list, so that it's considered before any other rule (which is the normal behavior when configuring a fake with  `A.CallTo(...)`). `AddRuleLast` adds a rule at the end of the rule list, so that it's considered after any other rule.

```csharp
class MyRule : IFakeObjectCallRule
{
    public int? NumberOfTimesToCall => null;

    public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
    {
        fakeObjectCall.SetReturnValue(42);
    }

    public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
    {
        return fakeObjectCall.Method.DeclaringType == typeof(IFoo)
            && fakeObjectCall.Method.Name == "Bar";
    }
}

...

var foo = A.Fake<IFoo>();
var manager = Fake.GetFakeManager(foo);
manager.AddRuleFirst(new MyRule());
Assert.Equal(42, foo.Bar());
```

You can also remove a rule using the `RemoveRule` method.

### Intercepting calls

Using the `FakeManager.AddInterceptionListener` method, you can add a listener that is called every time a fake method is called.

```csharp
class MyListener : IInterceptionListener
{
    public void OnBeforeCallIntercepted(IFakeObjectCall interceptedCall)
    {
        Console.WriteLine($"A call to '{interceptedCall.Method}' is about to be processed");
    }

    public void OnAfterCallIntercepted(ICompletedFakeObjectCall interceptedCall)
    {
        Console.WriteLine($"A call to '{interceptedCall.Method}' has been processed");
    }
}

...

var foo = A.Fake<IFoo>();
A.CallTo(() => foo.Baz()).Invokes(() => Console.WriteLine("Hello world"));
var manager = Fake.GetFakeManager(foo);
manager.AddInterceptionListener(new MyListener());
foo.Baz();
```

The code above prints:

```
A call to 'Void Baz()' is about to be processed
Hello world
A call to 'Void Baz()' has been processed
```
