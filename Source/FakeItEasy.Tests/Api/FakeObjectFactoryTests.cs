namespace FakeItEasy.Tests.Api
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class FakeObjectFactoryTests
    {
        IFakeObjectContainer container;
        IProxyGenerator proxyGenerator;
        FakeObject.Factory fakeObjectFactory;
        FakeObject fakeObject;

        [SetUp]
        public void SetUp()
        {
            this.container = A.Fake<IFakeObjectContainer>();
            this.proxyGenerator = A.Fake<IProxyGenerator>();
            this.fakeObject = A.Fake<FakeObject>();
            this.fakeObjectFactory = () => this.fakeObject;
        }

        private FakeObjectFactory CreateFactory()
        {
            return new FakeObjectFactory(this.container, this.proxyGenerator, this.fakeObjectFactory);
        }

        [Test]
        public void CreateFake_should_return_fake_from_container_when_non_proxyied_objects_are_allowed_and_container_contains_type()
        {
            var factory = this.CreateFactory();

            object result = null;
            A.CallTo(() => this.container.TryCreateFakeObject(typeof(int), out result)).Returns(true).AssignsOutAndRefParameters(1);

            Assert.That(factory.CreateFake(typeof(int), null, true), Is.EqualTo(1));
        }

        [Test]
        public void CreateFake_should_return_fake_from_proxy_generator_when_container_contains_type_but_non_proxied_objects_are_not_allowed()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            A.CallTo(() => this.container.TryCreateFakeObject(typeof(IFoo), out Null<object>.Out)).Returns(true).AssignsOutAndRefParameters(A.Fake<IFoo>());
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, this.container)).Returns(returned);

            Assert.That(factory.CreateFake(typeof(IFoo), null, false), Is.SameAs(returned.Proxy));
        }

        [Test]
        public void CreateFake_should_return_fake_from_proxy_generator_when_container_does_not_contain_type()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, this.container)).Returns(returned);

            Assert.That(factory.CreateFake(typeof(IFoo), null, true), Is.SameAs(returned.Proxy));
        }

        [Test]
        public void CreateFake_should_set_generated_proxy_to_fake_object()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, A<IEnumerable<object>>.That.IsThisSequence("foo").Argument)).Returns(returned);

            factory.CreateFake(typeof(IFoo), new object[] { "foo" }, false);

            A.CallTo(() => this.fakeObject.SetProxy(returned)).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void CreateFake_should_throw_exception_when_fake_cant_be_resolved_from_container_or_generated()
        {
            var factory = this.CreateFactory();

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), this.fakeObject, this.container)).Returns(new FailedProxyResult(typeof(IFoo)));

            var thrown = Assert.Throws<ArgumentException>(() =>
                factory.CreateFake(typeof(IFoo), null, true));

            Assert.That(thrown.Message, Is.EqualTo("Can not create fake of the type 'FakeItEasy.Tests.IFoo', it's not registered in the current container and the current IProxyGenerator can not generate the fake."));
        }

        [Test]
        public void CreateFake_should_pass_created_proxy_to_ConfigureFake_on_container()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, this.container)).Returns(returned);

            factory.CreateFake(typeof(IFoo), null, true);

            A.CallTo(() => this.container.ConfigureFake(typeof(IFoo), returned.Proxy)).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void CreateFake_should_return_fake_from_proxy_generator_when_arguments_for_constructor_is_specified_even_though_non_proxied_fakes_are_accepted()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, A<IEnumerable<object>>.That.IsThisSequence("argument for constructor").Argument)).Returns(returned);

            factory.CreateFake(typeof(IFoo), new object[] { "argument for constructor" }, true);

            A.CallTo(() => this.container.TryCreateFakeObject(typeof(IFoo), out Null<object>.Out)).MustHaveHappened(Repeated.Never);
        }

        [Test]
        public void CreateFake_should_throw_when_proxy_generator_can_not_generate_fake_with_arguments_for_constructor()
        {
            // Arrange
            var factory = this.CreateFactory();

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(string), A<FakeObject>.Ignored, A<IEnumerable<object>>.Ignored.Argument)).Returns(new FailedProxyResult(typeof(string)));

            // Act
            
            // Assert
            Assert.Throws<ArgumentException>(() =>
                factory.CreateFake(typeof(string), new object[] { }, false));
        }

        private class FailedProxyResult
            : ProxyResult
        {
            public FailedProxyResult(Type proxiedType)
                : base(proxiedType)
            {
                this.ProxyWasSuccessfullyCreated = false;
            }

            public override event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
        }

        private class SuccessfulProxyResult
            : ProxyResult
        {
            public SuccessfulProxyResult(Type proxiedType)
                : base(proxiedType)
            {
                this.ProxyWasSuccessfullyCreated = true;
            }

            public override event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
        }
    }
}
