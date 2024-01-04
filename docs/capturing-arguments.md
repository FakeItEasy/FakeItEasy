# Capturing arguments

Often tests will make [assertions against the calls made to a fake object](assertion.md)
to verify the behavior of the system under test, but you can also capture arguments
supplied in a call to a fake object and examine the values later.

## Simple capture

Take this contrived example:

```csharp title='"production code"'
--8<--
recipes/FakeItEasy.Recipes.CSharp/CapturingArguments.cs:IListLogger

recipes/FakeItEasy.Recipes.CSharp/CapturingArguments.cs:Calculator
--8<--
```

Suppose we want to make sure that we send correct messages to the logger.
We can record the first argument to each `Log` call in a variable of type
`Captured` and verify the values in the "assert" phase of the test.

```csharp title="simple capture" linenums="1" hl_lines="2 6 17 22"
--8<--
recipes/FakeItEasy.Recipes.CSharp/CapturingArguments.cs:SimpleCapture
--8<--
```

This is a fairly standard test with fakes, except we:

* create a `Captured` object to store received argument values
* use the `Captured` object's `_` member to configure the call to capture any values for that argument
  (this is analogous to the `A<T>._` member, and just like it, there is a matching `Ignored` member in
  case you prefer that name)
* use `Values` to access all the captured values so they can be asserted
* (alternative flow) use `GetLastValue` to access the most recent captured value so it can be asserted.
  This will throw a `FakeItEasy.ExpectationException` if no values have been captured.

???+ note "Values are only captured if the call matches the configuration"
    When a call configuration intends to capture one or more arguments, the argument
    values are only captured if this specific call configuration is triggered. If an incoming call
    does not match that configured for the method or property, no arguments are captured.

## Capturing and constraining at the same time

Even though you can interrogate captured values after the fact, you may want to configure
fake behavior to take effect only when incoming arguments meet a constraint. As you might have guessed
based on the references to `_` and `Ignored` above, you can do this using a `That` method just like the
one supplied by the [`A<T>` argument constraints](argument-constraints.md):

```csharp title="constrained capture" linenums="1" hl_lines="6"
--8<--
recipes/FakeItEasy.Recipes.CSharp/CapturingArguments.cs:ConstrainedCapture
--8<--
```

## Capturing mutable arguments

### The challenge

Argument capture is shallow; there's no copying of object state.
If a reference-based argument (e.g. a class instance, not a struct) is captured and
subsequently modified by the test or production code, the "assert" phase of the test
will see the updated state.

```csharp title="capturing mutating values" linenums="1" hl_lines="2 7 13 14 15 16 22 23"
--8<--
recipes/FakeItEasy.Recipes.CSharp/CapturingArguments.cs:NaivelyCaptureMutatedList
--8<--
```

Here a single list instance is passed to the production code twice, but an element is removed
between the calls. The `Captured` object captures a reference to the list each
time, but does not preserve the internals of the list. So by the time the `Values` are examined,
the list has had its first element removed, and this is reflected in the failing assertion.

It's the same effect as running

```c#
List<int> list = [1, 2, 3, 4];
var a = list;
list.RemoveAt(0);
var b = list;

// both a and b point to a list with elements { 2, 3, 4 }
```

### Capturing frozen state

`Captured` objects can be created with a transforming function (or "freezer") that runs on the
argument value before saving it away, thus insulating the captured values
from subsequent mutations.

```csharp title="freezing state of captured values" linenums="1" hl_lines="2 3 20"
--8<--
recipes/FakeItEasy.Recipes.CSharp/CapturingArguments.cs:CaptureCopiedMutatedList
--8<--
```

You can even transform values into a different type:

```csharp title="freezing state of captured values as new type" linenums="1" hl_lines="2 3 20 21"
--8<--
recipes/FakeItEasy.Recipes.CSharp/CapturingArguments.cs:CaptureCopiedMutatedListToNewType
--8<--
```

In this case we supply an updated freezer function as well as the `Captured` typeparams
for both the argument value and the captured value types.
