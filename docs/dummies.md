# Dummies

A Dummy is an object that FakeItEasy can provide when an object of a
certain type is required, but the actual behavior of the object is not
important.

## How to use them in your tests

Consider this example. Say that you want to test the following class:

```csharp
public class Library
{
    public bool Checkout(Card patronCard, Book someBook);
}
```

Maybe in one of your tests you want to invoke `Checkout` with an
expired library card. The checkout should fail, regardless of the book
being checked out&mdash;only the status of the card matters. Instead
of writing

```csharp
library.Checkout(MakeExpiredCard(),
                 new Book { Title = "The Ocean at the End of the Lane" } );
```

You can write:

```csharp
library.Checkout(MakeExpiredCard(), A.Dummy<Book>());
```

This signals that the actual value of the `Book` is really not
important. The code is intention-revealing.

You can also create a collection of dummies by writing:

```csharp
A.CollectionOfDummy<Book>(10);
```

This will return an `IList` containing 10 dummy `Book` instances.

For cases where the type of dummy isn't statically known, non-generic methods are also available. These are usually only required when writing extensions for FakeItEasy, so they live in the `FakeItEasy.Sdk` namespace:
```csharp
using FakeItEasy.Sdk;
...
var type = GetTypeOfDummy();
object dummy = Create.Dummy(type);
IList<object> dummies = Create.CollectionOfDummy(type, 10);
```

## How FakeItEasy uses them

When [creating Fakes](creating-fakes.md) or Dummies of class types,
FakeItEasy needs to invoke the classes' constructors. If the
constructors take arguments, FakeItEasy needs to generate appropriate
argument values. It uses Dummies.

## How are the Dummies made?

When FakeItEasy needs to access a Dummy of type `T`, it tries a number
of approaches in turn, until one succeeds:

1. If there's a user-supplied
  [dummy factory](custom-dummy-creation.md) for `T`,
  return whatever it makes.
1. If `T` is `String`, return an empty string.
1. If `T` is `Task` or `ValueTask`, the returned Dummy will be an actual `Task` or `ValueTask`
   that is already completed.
1. If `T` is `Task<TResult>` or `ValueTask<TResult>`, the returned Dummy will be an actual
  `Task<TResult>` or `ValueTask<TResult>` that is already completed and whose `Result` is a
  Dummy of type `TResult`, or a default `TResult` if no  Dummy can be made for `TResult`.
1. If `T` is a `Lazy<TValue>` the returned Dummy will be an actual
  `Lazy<TValue>` whose `Value` is a Dummy of type
  `TValue`, or a default `TValue` if no Dummy can be made
  for `TValue`.
1. If `T` is a tuple type (`Tuple<>` or `ValueTuple<>`), the Dummy will
be a tuple whose elements are dummies, or default values when dummies
can't be made.
1. If `T` is a value type, the Dummy will be the default value for that type (i.e. `new T()`).
1. If `T` is [fakeable](what-can-be-faked.md), the Dummy will be a
  Fake `T`.
1. If nothing above matched, then `T` is a class. Loop over all its
  public constructors in _descending order of argument list length_.
  For each constructor, attempt to get Dummies to satisfy the argument
  list. If the Dummies can be found, create an instance using that constructor,
  supplying the Dummies as the argument list. If the argument list can't be satisfied,
  then try the next constructor.

If none of these strategies yield a viable Dummy, then FakeItEasy
can't make a Dummy of type `T`.
