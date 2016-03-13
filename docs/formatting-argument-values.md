# Formatting Argument Values

FakeItEasy tries to provide helpful error messages when an
[Assertion](assertion.md) isn't met. For example, when an expected call to a fake
method isn't made, or when an unexpected call _is_ made. Often these
messages are adequate, but sometimes there's a need to improve upon
them, which can be done by writing custom argument value formatters.

## Writing a custom argument value formatter
Just define a class that extends `FakeItEasy.ArgumentValueFormatter<T>`. Here's a sample that formats argument values of type `Book`:
```csharp
class BookArgumentValueFormatter : ArgumentValueFormatter<Book>
{
    protected override string GetStringValue(Book argumentValue)
    {
        return string.Format("'{0}' published on {1:yyyy-MM-dd}",
            argumentValue.Title, argumentValue.PublishedOn);
    }
}
```

This would help FakeItEasy display this error message:
<pre>
Assertion failed for the following call:
  SampleTests.ILibrary.Checkout(<Ignored>)
Expected to find it never but found it #1 times among the calls:
  1: SampleTests.ILibrary.Checkout(<b>book: 'The Ocean at the End of the Lane', published on 2013-06-18</b>)
</pre>
which could make tracking down any failures a little easier.

Compare to the original behaviour:
<pre>
Assertion failed for the following call:
  SampleTests.ILibrary.Checkout(<Ignored>)
Expected to find it never but found it #1 times among the calls:
  1: SampleTests.ILibrary.Checkout(<b>book: SampleTests.Book</b>)
</pre>

In the original form of the message, the Book argument is just
formatted using `Book.ToString()` because FakeItEasy doesn't know any
better.

## How it works

FakeItEasy uses classes that implement the following interface to format argument values:

```csharp
public interface IArgumentValueFormatter
{
    string GetArgumentValueAsString(object argumentValue);
    Type ForType { get; }
    int Priority { get; }
}
```

`GetArgumentValueAsString` does the work, transforming an argument into its formatted representation.  
`ForType` indicates what type of argument a formatter can format.  
`Priority` is discussed below.

Above, we wrote a formatter in the preferred way, by extending
`abstract class ArgumentValueFormatter<T>:
IArgumentValueFormatter`.   `ArgumentValueFormatter<T>` defines a
`GetArgumentValueAsString` method that defers to `GetStringValue`, and
its `ForType` method simply returns `T`. The default implementation of
`Priority` returns `int.MinValue`, but this can be overridden.  It's
possible to write a formatter from scratch, but there's no advantage
to doing so over extending `ArgumentValueFormatter<T>`.

It's possible to create formatters for any type, including concrete
types, abstract types, and interfaces. Formatters defined for base
types and interfaces will be used when formatting values whose types
extend or implement the formatter's type.

## FakeItEasy's default formatter behaviour

Unless custom formatters are provided, FakeItEasy formats argument
values like so:

- the `null` value is formatted as `<null>`,
- the empty `string` is formatted as `string.Empty`,
- other `string` values are formatted as `"the string value"`, including the quotation marks, and
- any other value is formatted as its `ToString()` result

There is no way to change FakeItEasy's behaviour when formatting
`null`, but the other behaviour can be overridden by user-defined
formatters.

## Resolving formatter collisions

It's possible for a solution to contain multiple formatters that would
apply to the same types of arguments. In fact, it's guaranteed to
happen, since FakeItEasy itself defines a formatter that applies to
`object`s and one that applies to `string`s. Any user-defined
formatter will conflict with at least the built-in object formatter,
and maybe others. When there is more than one candidate for formatting
an argument, FakeItEasy picks the best one based on two factors:

- the distance between the argument's type (hereafter _ArgType_) and the type each formatter knows about (hereafter _ForType_), and
- the value of each formatter's `Priority` property

### Lowest distance

When an argument value needs to be formatted, FakeItEasy examines all
known formatters whose ForType is in ArgType's inheritance tree, or
whose ForType is an interface that ArgType implements. The _distance_
between ForType and ArgType is calculated as follows:

- 0 if ForType and ArgType are the same
- 1 if ForType is an interface that ArgType implements<sup>1</sup>
- 2 if `ForType == ArgType.BaseType`, 
- 3 if `ForType == ArgType.BaseType.BaseType`, and so on, adding one for every step in the inheritance chain

The formatter whose ForType has the smallest distance to ArgType is used to format the argument.

### Highest priority

Sometimes more than one formatter is found the same distance from
ArgType. Maybe two formatters actually specify the same `ForType`
property value, or there's a formatter defined for ArgType as well as
for an interface that ArgType implements.

When multiple formatters have the same distance from the argument,
FakeItEasy will select the one with the highest `Priority` property
value. If multiple formatters have the same distance _and_ the same
priority, the behavior is undefined.

The formatters that FakeItEasy includes have `Priority` equal to
`int.MinValue`, as do all classes that extend
`ArgumentValueFormatter<T>`, unless they explicitly override the
property. So, for example, a user-provided alternate formatter for
`string`s should override `Priority`, having it return a higher
value. Otherwise, there's no guarantee which formatter will be used.

###How does FakeItEasy find Argument Value Formatters?

On initialization, FakeItEasy
[looks for Discoverable Extension Points](scanning-for-extension-points.md),
including Argument Value Formatters.

----
1. In FakeItEasy 1.13.1 and earlier, the distance `0` was returned if the ForType and ArgType were the same **or** if ForType was an interface that ArgType implements, so if there were two formatters, each matching one of those conditions, there was no way to tell which one would be used. This was fixed in [Issue 142](https://github.com/FakeItEasy/FakeItEasy/issues/142).
