# Calling base methods

The `CallsBaseMethod` configuration method can be used to make a method execute the implementation of the faked class:

```csharp
A.CallTo(() => fakeShop.SellSmarties())
 .CallsBaseMethod();
```

Configuring a method to call its base method only makes sense if the method is actually implemented, so this technique cannot be used on an abstract class method or on any method from a faked interface—a faked abstract method told to call its base method will throw a `NotImplementedException`.

## Configuring all methods at once

Perhaps you want to have all or nearly all of a fake's (fakeable) methods defer to the original implementation. Rather than using `CallsBaseMethod` a dozen times, the [fake creation option](creating-fakes.md#explicit-creation-options) `CallsBaseMethods` can do all the work at once:

```csharp
var fakeShop = A.Fake<CandyShop>(options => options.CallsBaseMethods());
```

And then [selectively override some of them](limited-call-specifications.md#changing-behavior-between-calls)

```chsharp
A.CallTo(() => fakeShop.SellRockets()).Throws<Exception>();
```
