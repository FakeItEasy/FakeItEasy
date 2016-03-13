# Default fake behavior

Fake objects come with useful default behavior as soon as
[they are created](creating-fakes.md). Knowing the default behavior
can make the fakes easier to work with and can lead to more concise
tests.

## Non-overideable members cannot be faked

Methods and properties can only be faked if they are declared on a
faked interface, or are declared abstract or virtual on a faked
class. If none of these conditions hold, then a member cannot be
faked, just as it could not be overridden in a derived class.

When such a member is invoked on the fake, the original behavior will be invoked.

## Overrideable members are faked

When a method or property is declared on a faked interface, or is
declared as abstract or virtual on a faked class, and the member is
invoked on the fake, no action will be taken by the fake. It is as if
the body of the member were empty. If the member has a return type (or
is a get property), the return value will depend on the type `T` of
the member:
  
* If `T` can be made into a [Dummy](dummies.md), then a Dummy `T` will
  be returned. Note that this may be a Fake or an instance of a
  concrete, pre-existing type;
* othewise, `default(T)` will be returned.

## Examples

Suppose we have the following interface definition

```csharp
public interface Interface
{
    bool BooleanFunction();
    int IntProperty { get; set; }
    string StringFunction();
    FakeableClass FakeableClassFunction();
    UnfakeableClass UnfakeableClassProperty { get; set; }
    Struct StructFunction();
}
```

Then the following test will pass

```csharp
public void Members_should_return_empty_string_default_or_fake_another_fake()
{
    var fakeLibrary = A.Fake<Interface>();

    Assert.AreEqual(default(bool), fakeLibrary.BooleanFunction());

    Assert.AreEqual(default(int), fakeLibrary.IntProperty);

    Assert.AreEqual(typeof(string), fakeLibrary.StringFunction().GetType()); 
    Assert.AreEqual(string.Empty, fakeLibrary.StringFunction());

    Assert.IsInstanceOfType(fakeLibrary.FakeableClassFunction(), typeof(FakeableClass));
    Assert.AreEqual("FakeableClassProxy",
                    fakeLibrary.FakeableClassFunction().GetType().Name); // to show it's a fake

    Assert.IsNull(fakeLibrary.UnfakeableClassProperty);

    Assert.AreEqual(default(Struct), fakeLibrary.StructFunction());
}
```
