# Quickstart

## Getting started with FakeItEasy

### Installation

#### Package Manager

Run following command in Package Manager Console:

```ps1
Install-Package FakeItEasy
```

#### .NET CLI

Run following command in terminal:

```bat
dotnet add package FakeItEasy
```

#### PackageReference

 For projects that support PackageReference, copy this XML node into the csproj file to reference the package.

```xml
<PackageReference Include="FakeItEasy" Version="5.2.0" />
```

### Simple faking dependencies

```csharp
namespace FakeItEasyQuickstart
{
    using FakeItEasy;
    using NUnit; // any test framework will do

    public class SweetToothTests
    {
        [Test]
        public void BuyTastiestCandy_should_buy_top_selling_candy_from_shop()
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
