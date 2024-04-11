# Assertion

Assertion uses exactly the same syntax as configuration to specify the
call to be asserted, followed by a method call beginning with `.MustHaveHappened`.

The two most common forms of assertion are :

* `MustHaveHappened()` (no arguments) asserts that the call was made 1 or more times, and 
* `MustNotHaveHappened()` asserts that the specified call did not happen at all.

Arguments are constrained using
[Argument Constraints](argument-constraints.md) just like when
configuring calls.

## Syntax

```csharp
A.CallTo(() => foo.Bar()).MustHaveHappened();
A.CallTo(() => foo.Bar()).MustNotHaveHappened();

A.CallTo(() => foo.Bar()).MustHaveHappenedOnceExactly();
A.CallTo(() => foo.Bar()).MustHaveHappenedOnceOrMore();
A.CallTo(() => foo.Bar()).MustHaveHappenedOnceOrLess();

A.CallTo(() => foo.Bar()).MustHaveHappenedTwiceExactly();
A.CallTo(() => foo.Bar()).MustHaveHappenedTwiceOrMore();
A.CallTo(() => foo.Bar()).MustHaveHappenedTwiceOrLess();

A.CallTo(() => foo.Bar()).MustHaveHappened(4, Times.Exactly);
A.CallTo(() => foo.Bar()).MustHaveHappened(6, Times.OrMore);
A.CallTo(() => foo.Bar()).MustHaveHappened(7, Times.OrLess);

A.CallTo(() => foo.Bar()).MustHaveHappenedANumberOfTimesMatching(n => n % 2 == 0);
```

## Asserting Calls Made with Mutable Arguments

When BlairItEasy records a method (or property) call, it remembers
which objects were used as argument, but does not take a snapshot of
the objects' state. This means that if an object is changed after
being used as an argument, but before argument constraints are
checked, expected matches may not happen. For example,

```csharp
var aList = new List<int> {1, 2, 3};

A.CallTo(() => myFake.SaveList(A<List<int>>._))
    .Returns(true);

myFake.SaveList(aList);
aList.Add(4);

A.CallTo(() => myFake.SaveList(A<List<int>>.That.IsThisSequence(1, 2, 3)))
    .MustHaveHappened();
```

The `MustHaveHappened` will fail, because at the time the
`IsThisSequence` check is made, `aList` has 4 elements, not 3, and
`IsThisSequence` only has the reference to `aList` to use in its
check, not a deep copy or some other form of snapshot—it has to work
with the _current_ state.

If your test or production code must mutate call arguments between the
time of the call and the assertion time, you must look for some other
way to verify the call. Perhaps using `IsSameAs` will suffice, if the
correct behavior of the System Under Test can otherwise be
inferred. Or consider using [Invokes](invoking-custom-code.md) to
create a snapshot of the object and interrogate it later:

```csharp
var aList = new List<int> {1, 2, 3};

List<int> capturedList;
A.CallTo(() => myFake.SaveList(A<List<int>>._))
    .Invokes((List<int> list) => capturedList = new List<int>(list))
    .Returns(true);

myFake.SaveList(aList);
aList.Add(4);

Assert.That(capturedList, Is.EqualTo(new List<int> {1, 2, 3}));
```

## More advanced assertions

If the built-in assertion API isn't sufficient, you can also examine the list of recorded calls directly, as described in [Getting the list of calls made on a fake](advanced-usage.md#getting-the-list-of-calls-made-on-a-fake).

## VB.NET

```
' Functions and Subs can be asserted using their respective keywords
A.CallTo(Function() foo.Bar()).MustHaveHappened()
A.CallTo(Sub() foo.Baz(A(Of String).Ignored)).MustHaveHappened()
```
