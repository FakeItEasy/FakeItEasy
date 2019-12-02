# Calling wrapped methods

By default, calls to a wrapping fake that have not been explicitly configured will be forwarded to the wrapped object.
However, if this behavior has been overridden by another configuration, or if you need to invoke a callback before
calling the wrapped method, you can explicitly configure the call to be forwarded to the wrapped object with the
`CallsWrappedMethod` configuration method.

```csharp
var realShop = new CandyShop();
var fakeShop = A.Fake<ICandyShop>(o => o.Wrapping(realShop));
A.CallTo(() => fakeShop.SellSmarties())
    .Invokes(() => Console.WriteLine("Selling smarties!"))
    .CallsWrappedMethod();
```
