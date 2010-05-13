namespace FakeItEasy.Tests.Core.Creation
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class ProxyResultTests
    {
        private TestableProxyResult CreateResult(Type typeOfProxy)
        {
            return new TestableProxyResult(typeOfProxy);
        }

        [Test]
        public void Constructor_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new TestableProxyResult(typeof(string)));
        }

        [Test]
        [SetCulture("en-US")]
        public void Proxy_should_throw_when_set_to_object_that_is_not_of_the_specified_proxy_type()
        {
            var result = this.CreateResult(typeof(IFoo));
            
            var thrown = Assert.Throws<ArgumentException>(() =>
                result.SetProxy((IFakedProxy)A.Fake<IFormatProvider>()));

            Assert.That(thrown.Message, Is.EqualTo("The specified proxy is not of the correct type."));
        }

        [Test]
        public void Constructor_should_set_the_proxied_type()
        {
            var result = this.CreateResult(typeof(IFoo));

            Assert.That(result.TypeOfProxy, Is.EqualTo(typeof(IFoo)));
        }

        [Test]
        public void Proxy_should_throw_if_null_is_set()
        {
            var result = this.CreateResult(typeof(IFoo));

            Assert.Throws<ArgumentNullException>(() =>
                result.SetProxy(null));
        }

        public class TestableProxyResult
            : ProxyResult
        {
            public TestableProxyResult(Type typeOfProxy)
                : base(typeOfProxy)
            {
                
            }

            public override event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;

            public void SetProxy(IFakedProxy proxy)
            {
                this.Proxy = proxy;
            }
        }
    }
}
