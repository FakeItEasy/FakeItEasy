namespace FakeItEasy.Tests.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultFakeAndDummyManagerTests
    {
        private IFakeObjectContainer container;
        private IProxyGeneratorNew proxyGenerator;
        private FakeObject.Factory fakeObjectFactory;
        private FakeObject fakeObject;

        private DefaultFakeAndDummyManager fakeAndDummyManager;

        [SetUp]
        public void SetUp()
        {
            this.container = A.Fake<IFakeObjectContainer>();
            this.proxyGenerator = A.Fake<IProxyGeneratorNew>();
            this.fakeObject = A.Fake<FakeObject>();
            this.fakeObjectFactory = () => this.fakeObject;

            this.fakeAndDummyManager = A.Fake<DefaultFakeAndDummyManager>(x => x.WithArgumentsForConstructor(() =>
                new DefaultFakeAndDummyManager(this.container, this.proxyGenerator, this.fakeObjectFactory)));

            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(A<Type>.Ignored, A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).CallsBaseMethod();
        }

        private static ProxyResult CreateFakeProxyResult()
        {
            var proxyResult = A.Fake<ProxyResult>();
            A.CallTo(() => proxyResult.ProxyWasSuccessfullyCreated).Returns(true);
            A.CallTo(() => proxyResult.Proxy).Returns((IFakedProxy)A.Fake<IFoo>());
            return proxyResult;
        }

        [Test]
        public void CreateDummy_should_return_dummy_from_container_when_available()
        {
            // Arrange
            var fake = A.Fake<IFoo>();

            A.CallTo(() => this.container.TryCreateFakeObject(typeof(IFoo), out Null<object>.Out))
                .Returns(true)
                .AssignsOutAndRefParameters(fake);

            // Act
            var result = this.fakeAndDummyManager.CreateDummy(typeof(IFoo));

            // Assert
            Assert.That(result, Is.SameAs(fake));
        }

        [Test]
        public void CreateDummy_should_create_proxy_when_container_doesnt_contain_type()
        {
            // Arrange
            var result = A.Fake<IFoo>();

            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), null, true)).Returns(result);
            
            // Act
            var returned = this.fakeAndDummyManager.CreateDummy(typeof(IFoo));

            // Assert
            Assert.That(returned, Is.SameAs(result));
        }

        [Test]
        public void CreateProxy_should_pass_fake_object_from_factory_to_proxy_generator()
        {
            // Arrange
            
            // Act
            this.fakeAndDummyManager.CreateProxy(typeof(IFoo), null, false);

            // Assert
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), this.fakeObject, null)).MustHaveHappened();
        }

        [Test]
        public void CreateProxy_should_set_proxy_from_generator_to_fake_object()
        {
            var returned = CreateFakeProxyResult();

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, A<IEnumerable<object>>.Ignored.Argument)).Returns(returned);

            this.fakeAndDummyManager.CreateProxy(typeof(IFoo), null, false);

            A.CallTo(() => this.fakeObject.SetProxy(returned)).MustHaveHappened();
        }

        [Test]
        public void CreateProxy_should_pass_arguments_for_constructor_to_proxy_generator()
        {
            // Arrange
            
            // Act
            this.fakeAndDummyManager.CreateProxy(typeof(IFoo), new [] { "constructor", "arguments" }, false);

            // Assert
            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>.Ignored, A<FakeObject>.Ignored, A<IEnumerable<object>>.That.IsThisSequence("constructor", "arguments").Argument)).MustHaveHappened();
        }

        [Test]
        public void CreateFake_should_return_value_from_create_proxy()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(foo);

            // Act
            var returned = this.fakeAndDummyManager.CreateFake(typeof(IFoo), A.Dummy<FakeOptions>());

            // Assert
            Assert.That(returned, Is.SameAs(foo));
        }

        [Test]
        public void CreateFake_should_send_arguments_for_constructor_from_options_to_CreateProxy()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(foo);

            // Act
            var returned = this.fakeAndDummyManager.CreateFake(typeof(IFoo), new FakeOptions { ArgumentsForConstructor = new[] { "a", "b" } });

            // Assert
            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<object>>.That.IsThisSequence("a", "b").Argument, A<bool>.Ignored)).MustHaveHappened();
        }

        [Test]
        [SetCulture("en-US")]
        public void CreateProxy_should_fail_when_proxy_can_not_be_generated_and_throwOnFailure_is_set_to_true()
        {
            // Arrange
            var proxyResult = CreateFakeProxyResult();
            A.CallTo(() => proxyResult.ProxyWasSuccessfullyCreated).Returns(false);
            A.CallTo(() => proxyResult.ErrorMessage).Returns(@"Error message
with two lines.");

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), this.fakeObject, null)).Returns(proxyResult);

            // Act
            
            // Assert
            var ex = Assert.Throws<FakeCreationException>(() =>
                this.fakeAndDummyManager.CreateProxy(typeof(IFoo), null, true));

            Assert.That(ex.Message, Is.EqualTo(@"

  FakeItEasy failed to create fake object of type FakeItEasy.Tests.IFoo.
  The current IProxyGenerator failed with the following error message:

    Error message
    with two lines.

"));
        }

        [Test]
        public void TryCreateDummy_should_return_true_when_container_contains_dummy_type()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            object result = null;
            
            A.CallTo(() => this.container.TryCreateFakeObject(typeof(IFoo), out result)).Returns(true).AssignsOutAndRefParameters(foo);
            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), null, false)).ReturnsNull();

            // Act
            var success = this.fakeAndDummyManager.TryCreateDummy(typeof(IFoo), out result);

            // Assert
            Assert.That(success, Is.True);
        }

        [Test]
        public void TryCreateDummy_should_return_false_when_create_proxy_returns_null()
        {
            // Arrange
            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), null, false)).ReturnsNull();
            object result = null;

            // Act
            var success = this.fakeAndDummyManager.TryCreateDummy(typeof(IFoo), out result);

            // Assert
            Assert.That(success, Is.False);
        }

        [Test]
        public void TryCreateDummy_should_return_true_when_create_proxy_returns_result()
        {
            // Arrange
            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), null, false)).Returns(A.Fake<IFoo>());
            object result = null;

            // Act
            var success = this.fakeAndDummyManager.TryCreateDummy(typeof(IFoo), out result);

            // Assert
            Assert.That(success, Is.True);
        }

        //[Test]
        //public void CreateFake_should_throw_exception_when_fake_cant_be_resolved_from_container_or_generated()
        //{
        //    var factory = this.CreateFactory();

        //    A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), this.fakeObject, this.container)).Returns(new FailedProxyResult(typeof(IFoo)));

        //    var thrown = Assert.Throws<ArgumentException>(() =>
        //        factory.CreateFake(typeof(IFoo), null, true));

        //    Assert.That(thrown.Message, Is.EqualTo("Can not create fake of the type 'FakeItEasy.Tests.IFoo', it's not registered in the current container and the current IProxyGenerator can not generate the fake."));
        //}

        //[Test]
        //public void CreateFake_should_pass_created_proxy_to_ConfigureFake_on_container()
        //{
        //    var factory = this.CreateFactory();

        //    var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

        //    A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, this.container)).Returns(returned);

        //    factory.CreateFake(typeof(IFoo), null, true);

        //    A.CallTo(() => this.container.ConfigureFake(typeof(IFoo), returned.Proxy)).MustHaveHappened(Repeated.Once);
        //}

        //[Test]
        //public void CreateFake_should_return_fake_from_proxy_generator_when_arguments_for_constructor_is_specified_even_though_non_proxied_fakes_are_accepted()
        //{
        //    var factory = this.CreateFactory();

        //    var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

        //    A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, A<IEnumerable<object>>.That.IsThisSequence("argument for constructor").Argument)).Returns(returned);

        //    factory.CreateFake(typeof(IFoo), new object[] { "argument for constructor" }, true);

        //    A.CallTo(() => this.container.TryCreateFakeObject(typeof(IFoo), out Null<object>.Out)).MustHaveHappened(Repeated.Never);
        //}

        //[Test]
        //public void CreateFake_should_throw_when_proxy_generator_can_not_generate_fake_with_arguments_for_constructor()
        //{
        //    // Arrange
        //    var factory = this.CreateFactory();

        //    A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(string), A<FakeObject>.Ignored, A<IEnumerable<object>>.Ignored.Argument)).Returns(new FailedProxyResult(typeof(string)));

        //    // Act

        //    // Assert
        //    Assert.Throws<ArgumentException>(() =>
        //        factory.CreateFake(typeof(string), new object[] { }, false));
        //}
    }
}
