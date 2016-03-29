# Quickstart

Getting started with FakeItEasy is very simple:

* Open the Package Manager Console:  
Tools → Library Package Manager → Package Manager Console
* Execute `Install-Package FakeItEasy`
* Start faking dependencies in your tests. Here's a sample test class with fakes:

```csharp
namespace FakeItEasyQuickstart
{
    using FakeItEasy;
    using NUnit; // any test framework will do

    public class SweetToothTests
    {
        [Test]
        public void BuyTastiestCandy_should_buy_top_selling_candy_from_shop
        {
            // make some fakes for the test
            var lollipop = A.Fake<ICandy>();
            var shop = A.Fake<ICandyShop>();

            // set up a call to return a value
            A.CallTo(() => shop.GetTopSellingCandy()).Returns(lollipop);

            // use the fake as an actual instance of the faked type
            var developer = new SweetTooth();
            developer.BuyTastiestCandy(shop);

            // asserting uses the exact same syntax as when configuring calls—
            // no need to learn another syntax
            A.CallTo(() => shop.BuyCandy(lollipop)).MustHaveHappened();
        }
    }
}
```

* Most FakeItEasy functionality is reached from a common entry point: the `A` class.
* In this example the `lollipop` instance is used as a stub and the `shop` instance is used as a mock but there's no need to know the difference, just fake it! Easy!
* Fluent, easy-to-use syntax guides you as you configure fakes.
