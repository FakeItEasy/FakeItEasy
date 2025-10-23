# Argument constraints

When configuring and [asserting](assertion.md) calls in FakeItEasy,
the arguments of the call can be constrained so that only calls to the
configured method where the arguments matches the constraint are
selected.

## Matching values exactly

Assume the following interface exists:
```csharp
public interface IFoo
{
    void Bar(string s, int i);
}
```

Then the arguments to Bar can be constrained to limit call matching:
```csharp
var foo = A.Fake<IFoo>();

A.CallTo(() => foo.Bar("hello", 17)).MustHaveHappened();
```

Then FakeItEasy will look _only_ for calls made with the arguments
`"hello"` and `17` - no other calls will match the rule.

When checking for argument equality, FakeItEasy uses the first applicable equality
test from this list:

* if both values are `null`, they are considered equal,
* if one value is `null` and the other isn't, they are not considered equal,
* the highest-priority [custom argument equality comparer](custom-argument-equality.md)
  that can compare the example object's type, or
* if both values are `string`, `string.Equals` (as an optimization),
* if the example object's type implements `System.Collections.IEnumerable`, the values
  are considered equal if and only if the actual object's type also implements
  `System.Collections.IEnumerable` and `System.Linq.Enumerable.SequenceEqual` evaluates
  to true, or
* `Object.Equals`

If this list is not satisfactory, you may have to use the `That.Matches`
method described in [Custom matching](#custom-matching). Be
particularly careful of types whose `Equals` methods perform reference
equality rather than value equality. In that case, the objects have to
be _the same object_ in order to match, and this sometimes produces
unexpected results. When in doubt, verify the type's `Equals`
behavior manually.

## Other matchers
### Ignoring arguments

Suppose the value of the integer in the `Bar` call wasn't important,
but the string was. Then the following constraint could be used:

```csharp
A.CallTo(() => foo.Bar("hello", A<int>.Ignored)).MustHaveHappened();
```

Then any call will match, so long as the string value was
`"hello"`. The `Ignored` property can be used on any type.

An underscore (`_`) can be used as a shorthand for `Ignored` as well:
```csharp
A.CallTo(() => foo.Bar("hello", A<int>._)).MustHaveHappened();
```

### More convenience matchers

If more complicated constraints are needed, the `That` method can be
used. There are a few built-in matchers:

|Matcher|Tests for|
|:------|:--------|
|IsNull()|`null`|
|IsNotNull()|not `null`|
|IsEqualTo(other)|object equality using `object.Equals`|
|IsEqualTo(other, equalityComparer)|object equality using `equalityComparer.Equals`|
|IsSameAs(other)|object identity - like `object.ReferenceEquals`|
|IsInstanceOf(type)|an argument that can be assigned to a variable of type `type`|
|Contains(string)|substring match with ordinal string comparison|
|Contains(string, comparisonType)|substring match with the specified comparison type|
|StartsWith(string)|substring at beginning of string with ordinal string comparison|
|StartsWith(string, comparisonType)|substring at beginning of string with the specified comparison type|
|EndsWith(string)|substring at end of string with ordinal string comparison|
|EndsWith(string, comparisonType)|substring at end of string with the specified comparison type|
|IsNullOrEmpty()|`null` or `""`|
|IsEmpty()|empty enumerable|
|Contains(item)|item's presence in an enumerable|
|IsSameSequenceAs(enumerable)|sequence equality, like `System.Linq.Enumerable.SequenceEqual`|
|IsSameSequenceAs(value1, value2, ...)|sequence equality, like `System.Linq.Enumerable.SequenceEqual`|
|Not|inverts the sense of the matcher|

### Custom matching

If none of the canned matchers are sufficient, you can provide a
predicate to perform custom matching using `That.Matches`. Like in
this rather contrived example:

```csharp
A<string>.That.Matches(s => s.Length == 3 && s[1] == 'X');
```

FakeItEasy will evaluate the predicate against any supplied
argument. The predicate can be supplied as an `Expression<Func<T, bool>>`
or as a `Func<T, bool>`. FakeItEasy can generate a description
of the matcher when an `Expression` is supplied (although you may
supply your own as well), but you must supply a description when using
a `Func`.

### Always place `Ignored` and `That` inside `A.CallTo`

The `Ignored` (and `_`) and `That` matchers must be placed within the
expression inside the `A.CallTo` call. This is because these special
constraint methods do not return an actual matcher object. They tell
FakeItEasy how to match the parameter via a special event that's fired
when the constraint method is invoked. FakeItEasy only listens to the
events in the context of an `A.CallTo`.

So, tempting as it might be to save one of the constraints away in a
handy variable, don't do it.

## Using correct grammar

FakeItEasy's API attempts to imitate the English language, so that call
configurations and assertions read naturally. In that spirit, it's
possible to use `An<T>` instead of `A<T>` for types whose name starts
with a vowel sound. For instance:

```csharp
An<Apple>.That.Matches(a => a.Color == "Red")
```

`A<T>` and `An<T>` are exact synonyms and can be used exactly the same
way.

## `out` parameters

The incoming argument value of `out` parameters is ignored when matching
calls. The incoming value of an `out` parameter can't be seen by the
method body anyhow, so there's no advantage to constraining by it.

For example, this test passes:

```csharp
string configurationValue = "lollipop";
A.CallTo(() => aFakeDictionary.TryGetValue(theKey, out configurationValue))
 .Returns(true);

string fetchedValue = "licorice";
var success = aFakeDictionary.TryGetValue(theKey, out fetchedValue);

Assert.That(success, Is.True);
```

See
[Implicitly Assigning `out` Parameter Values](assigning-out-and-ref-parameters.md#implicitly-assigning-out-parameter-values)
to learn how the initial `configurationValue` is used in this case.

## `ref` parameters

Due to the limitations of working with `ref` parameters in C#, only exact-value matching is possible using argument constraints,
and the argument value must be compared against a local variable or a field:

```csharp
int someValue = 3;
A.CallTo(() => aFake.aMethod(ref someValue)).Returns(true);
```

To perform more sophisticated matching of `ref` parameter values in C#, use constraints that work on the entire call, such as `WithAnyArguments`
or `WhenArgumentsMatch`, described below.

In addition to constraining by `ref` argument values, calls can be explicitly configured to
[assign outgoing `ref` argument values](assigning-out-and-ref-parameters.md).


## Arguments of anonymous types

Anonymous types are not shared between assemblies, so it's not possible to make an argument
constraint that matches an anonymous type from another assembly. Instead, you may use
`A.CallTo(fake).Where(...)` as described in
[Specifying a call to any method or property](specifying-a-call-to-configure.md#specifying-a-call-to-any-method-or-property).


## Overriding argument matchers

Sometimes individually constraining arguments isn't sufficient. In
such a case, other methods may be used to determine which calls match
the fake's configuration.

_When using the following methods, any inline argument constraints are ignored, and only the
special method is used to match the call._ Some arguments have to be supplied in order to satisfy the compiler,
but the values will not be used, so you can supply whatever values make the code easiest for you to read.

`WithAnyArguments` ensures that no argument constraints will be applied when matching calls:

```csharp
A.CallTo(() => foo.Bar(null, 7)).WithAnyArguments().MustHaveHappened();
```

The example above will match any call to `foo.Bar`, regardless of the arguments. The
`Ignored` property performs the same task, and is more flexible, but
some people prefer the look of `WithAnyArguments`.


`WhenArgumentsMatch` accepts a predicate that operates on the entire
collection of call arguments.  For example, to have a Fake throw an
exception when a call is made to `Bar` where the first arguments is a
string representation of the second, use

```csharp
A.CallTo(() => fake.Bar(null, 0))
    .WhenArgumentsMatch(args =>
        args.Get<string>("theString")
            .Equals(args.Get<int>("theInt").ToString()))
    .Throws<Exception>();
```

Strongly typed overloads of `WhenArgumentsMatch` are also available for methods of up to 8
parameters (if a method has more parameters, use the variant described above):

```csharp
A.CallTo(() => fake.Bar(null, 0))
    .WhenArgumentsMatch((string theString, int theInt) =>
        theString.Equals(theInt.ToString()))
    .Throws<Exception>();
```

## Capturing arguments

It is also possible to [capture arguments passed to a call](capturing-arguments.md), whether you
also want to constrain those arguments or not, and verify the passed values after the fact.

## Nested argument constraints

Note that an argument constraint cannot be "nested" in an argument; the
constraint has to be the whole argument. For instance, the following call
configurations are invalid and will throw an exception:

```
A.CallTo(() => fake.Foo(new Bar(A<int>._))).Returns(42);
A.CallTo(() => fake.Foo(new Bar { X = A<string>.That.Contains("hello") })).Returns(42);
```

To achieve the desired effect, you can do this instead:

```
A.CallTo(() => fake.Foo(A<Bar>._)).Returns(42);
A.CallTo(() => fake.Foo(A<Bar>.That.Matches(bar => bar.X.Contains("hello")))).Returns(42);
```
