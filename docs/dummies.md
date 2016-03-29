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

## How FakeItEasy uses them

When [creating Fakes](creating-fakes.md) or Dummies of class types,
FakeItEasy needs to invoke the classes' constructors. If the
constructors take arguments, FakeItEasy needs to generate appropriate
argument values. It uses Dummies.

## How are the Dummies made?

When FakeItEasy needs to access a Dummy of type `T`, it tries a number
of approaches in turn, until one succeeds:

1. see if there's a user-supplied
  [custom Dummy creation](custom-dummy-creation.md) mechanism for `T`

1. if `T` is `Task`, the returned Dummy will be an actual `Task` that
  completes immediately<sup>1</sup>

1. if `T` is `Task<TResult>`
    * if `TResult` can be made into a Dummy, then returned Dummy will be an actual
      `Task<TResult>` that completes immediately<sup>1</sup> and whose
      `Result` is a Dummy of type `TResult`
    * if `TResult` cannot be made into a Dummy, an unconfigured Fake
      `Task<TResult>` will be returned. If this causes problems,
      [consider upgrading now](https://nuget.org/packages/FakeItEasy/)
    
1. if `T` is a `Lazy<TValue>`, then

    * if `TValue` can be made into a Dummy and has a parameterless
      constructor, the returned Dummy will be an actual `Lazy<TValue>`
      whose `Value` is a Dummy of type `TValue`.

    * if `TValue` can't be made into a Dummy, an unconfigured Fake
      `Lazy<TResult>` will be returned. If this causes problems,
      [consider upgrading now](https://nuget.org/packages/FakeItEasy/)

    * if `TValue` doesn't have a parameterless constructor, then the
      `Lazy` will not behave well. If this causes problems,
      [consider upgrading now](https://nuget.org/packages/FakeItEasy/)
      
1. if `T` is [fakeable](what-can-be-faked.md), the Dummy will be a
  Fake `T`

1. if `T` is a value type, the Dummy will be a `T` created via
  `Activator.CreateInstance`

1. if nothing above matched, then `T` is a class. Loop over all its constructors in _descending order of argument list length_.  
  For each constructor, attempt to get Dummies to satisfy the argument
  list. If the Dummies can be found, use `Activator.CreateInstance` to
  create the Dummy, supplying the Dummies as the argument list. If the
  argument list can't be satisfied, then try the next constructor.

1. if none of the previous strategies yield a viable Dummy, then
  FakeItEasy can't make a Dummy of type `T`.

----
1. In FakeItEasy 1.12 or earlier, the `Task` returned from a
  non-configured fake method would never be completed and (for
  example) an `await` would never be satisfied. If you are using 1.12
  or earlier, [upgrade now](https://nuget.org/packages/FakeItEasy/).

