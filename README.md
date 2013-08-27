![Are you mocking me?](http://lh5.ggpht.com/_iTnDnty4mRk/TFsS15Nuu7I/AAAAAAAAANM/EuX4rAhiF0s/FakingMock.png)

##It's faking amazing!

    // Creating a fake object is just dead easy!
    // No mocks, no stubs, everything's a fake!
    var lollipop = A.Fake<ICandy>();
    var shop = A.Fake<ICandyShop>();
    
    // To set up a call to return a value is also simple:
    A.CallTo(() => shop.GetTopSellingCandy()).Returns(lollipop);
    
    // Use your fake as you would an actual instance of the faked type.
    var developer = new SweetTooth();
    developer.BuyTastiestCandy(shop);
    
    // Asserting uses the exact same syntax as when configuring calls,
    // no need to teach yourself another syntax.
    A.CallTo(() => shop.BuyCandy(lollipop)).MustHaveHappened();

In this example the lollipop instance is used as a stub and the shop instance is used as a mock but there's no need to know the difference, just fake it! Easy!

Available on [NuGet](https://nuget.org/packages/FakeItEasy/).

##Documentation
Full documentation is avilable on [GitHub](https://github.com/FakeItEasy/FakeItEasy/wiki).

##Description
A .Net dynamic fake framework for creating all types of fake objects, mocks, stubs etc.

* Easier semantics, all fake objects are just that - fakes - the use of the fakes determines whether they're mocks or stubs.
* Context aware fluent interface guides the developer.
* Full VB.Net support.

Designed for ease of use and for compatibility with both C# and VB.Net.

##Syntax
**Creating a fake object:**

You can create fake objects in two ways in FakeItEasy, either by calls to A.Fake-methods.

    IFoo foo = A.Fake<IFoo>();

Or you can create a fake object, that is a wrapper around the faked object, this object provides an api for configuring and asserting on the faked object, like this:

    Fake<IFoo> fake = new Fake<IFoo>();
    IFoo = fake.FakedObject;

**Configuring a method on the fake object to return something:**

    A.CallTo(() => foo.Bar()).Returns("test");

**Configuring calls to any method on an object:**

    A.CallTo(foo).Throws(new Exception());
    A.CallTo(foo).WithReturnType<string>().Returns("hello world");

**When matching calls you can mix argument constraints and concrete arguments that are matched by equality:**

    A.CallTo(() => foo.Bar(A<string>.Ignored, "second argument")).Throws(new Exception());

**Return values can be produced at call time:**

    int counter = 0;
    A.CallTo(() => foo.Baz()).Returns(() => counter++);

**Assertion:**

    A.CallTo(() => foo.Bar()).MustHaveHappened();
    A.CallTo(() => foo.Bar()).MustNotHaveHappened();
    A.CallTo(() => foo.Bar()).MustHaveHappened(Repeated.AtLeast.Once);
    A.CallTo(() => foo.Bar()).MustHaveHappened(Repeated.Never);
    A.CallTo(() => foo.Bar()).MustHaveHappened(Repeated.NoMoreThan.Times(4));
    A.CallTo(() => foo.Bar()).MustHaveHappened(Repeated.Exactly.Twice);

**Faking a class that takes arguments to constructor, no untyped object array, safe for refactoring:**

In order to pass arguments to the constructor of fakes of classes you'd use a lambda expression rather than the common method of passing object arrays representing the arguments. The expression will actually never be invoked so the constructor call in the following example will not be invoked but the arguments will be extracted from it.

    var foo = A.Fake<Foo>(x => x.WithArgumentsForConstructor(() => new Foo("string passed to constructor")));

    // Specifying arguments for constructor using IEnumerable<object>.
    var foo = A.Fake<Foo>(x => x.WithArgumentsForConstructor(new object[] { "foo" }));


**Faking an interface and assigning additional custom attributes to the faked class:**

    //Get Constructor for our attribute with no parameters
    var constructor = typeof(FooAttribute).GetConstructor(new Type[0]);
    //Create a builder with our constructor and no arguments
    var builder = new CustomAttributeBuilder(constructor, new object[0]);
    var builders = new List<CustomAttributeBuilder>() { test };
    //Foo and Foo's type should both have "FooAttribute"
    var foo = A.Fake<IFoo>(x => x.WithAdditionalAttributes(builders));
  
**To raise an event on a fake object:**

    foo.SomethingHappened += Raise.With(EventArgs.Empty).Now;

**To raise an event on a fake object in VB:**

    AddHandler foo.SomethingHappened, AddressOf Raise.With(EventArgs.Empty).Now

    'If the event is an EventHandler(Of T) you can use the shorter syntax:

    AddHandler foo.SomethingHappened, Raise.With(EventArgs.Empty).Go

**Configuring a "Sub" call in VB:**

    NextCall.To(foo).WithAnyArguments().Throws(New Exception())
    foo.Bar(null, null)

**Asserting on a "Sub" call in VB:**

    NextCall.To(foo).WithAnyArguments().MustHaveHappened()
    foo.Bar(null, null)

**In .Net 4 VB supports lambda-subs as well:**

    A.CallTo(Sub() foo.Bar(A<object>.Ignored, A<object>.Ignored)).MustHaveHappened()

**Configuring a "Function" in VB is just like in C#:**

    A.CallTo(Function() foo.Baz()).Returns(10)
