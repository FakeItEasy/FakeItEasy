# Assertion

Assertion uses exactly the same syntax as configuration to specify the
call to be asserted, followed by `.MustHaveHappened(Repeated)`, where
`Repeated` specifies the number of expected repetitions.

Two extension methods are provided for convenience:

* `MustHaveHappened()` (no arguments) ignores the number of times the call was made, and 
* `MustNotHaveHappened()` asserts that the specified call did not happen at all.

Arguments are constrained using
[Argument Constraints](argument-constraints.md) just like when
configuring calls.

#Details
##Syntax

```csharp
// Asserting that a call has happened at least once.
// The following two lines are equivalent.
A.CallTo(() => foo.Bar()).MustHaveHappened(Repeated.AtLeast.Once);    // or
A.CallTo(() => foo.Bar()).MustHaveHappened();

// To contrast, assert that a call has happened exactly once.
A.CallTo(() => foo.Bar()).MustHaveHappened(Repeated.Exactly.Once);

// Asserting that a call has not happened.
// The following two lines are equivalent.
A.CallTo(() => foo.Bar()).MustNotHaveHappened();    // or
A.CallTo(() => foo.Bar()).MustHaveHappened(Repeated.Never);
```

#Specifying Repeat

```csharp
// Using the Repeated class:
Repeated.AtLeast.Once // The call must have happened once or more.
Repeated.Exactly.Once // The call must have happened exaclty one time
    
Repeated.AtLeast.Twice // The call must have happened twice or more.
Repeated.Exactly.Twice // The call must have happened twice exactly.
Repeated.NoMoreThan.Twice // The call must have happened zero, one, or two times.

Repeated.AtLeast.Times(10) // The call must have happened ten times or more
Repeated.Exactly.Times(10) // The call must have happened ten times exactly
Repeated.NoMoreThan.Times(10) // The call must have happened any number of times between zero and ten.
    
// Using a predicate.
Repeated.Like(x => x % 2 == 0) // The call must have happened an even number of times.
```

# Asserting Calls Made with Mutable Arguments

When FakeItEasy records a method (or property) call, it remembers
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
    .MustHaveHappend();
```

The `MustHaveHappened` will fail, because at the time the
`IsThisSequence` check is made, `aList` has 4 elements, not 3, and
`IsThisSequence` only has the reference to `aList` to use in its
check, not a deep copy or some other form of snapshot—it has to work
with the _current_ state.

If your test or production code must mutate call arguments between the
time of the call and the assertion time, you must look for some other
way to very the call. Perhaps using `IsSameAs` will suffice, if the
correct behaviour of the System Under Test can otherwise be
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

#VB.Net

```
' Functions and Subs can be asserted using their respective keywords
A.CallTo(Function() foo.Bar()).MustHaveHappened()
A.CallTo(Sub() foo.Baz(A(Of String).Ignored)).MustHaveHappened()
```
