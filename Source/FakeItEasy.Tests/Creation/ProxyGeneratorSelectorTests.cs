namespace FakeItEasy.Tests.Creation
{
    using System;
    using FakeItEasy.Creation;
    using FakeItEasy.Creation.DelegateProxies;
    using NUnit.Framework;

    [TestFixture]
    public class ProxyGeneratorSelectorTests
    {
        [Fake]
        private DelegateProxyGenerator delegateProxyGenerator;
        
        [Fake]
        private IProxyGenerator defaultProxyGenerator;

        [UnderTest]
        private ProxyGeneratorSelector selector;

        [SetUp]
        public void SetUp()
        {
            Fake.InitializeFixture(this);
        }

        [Test]
        public void Should_delegate_calls_to_delegate_generator_when_generating_delegate_proxy()
        {
            // Arrange
            var additionalInterfaces = new Type[] { };
            var argumentsForConstructor = new object[] { };

            var expected = A.Dummy<ProxyGeneratorResult>();
            A.CallTo(() => this.delegateProxyGenerator.GenerateProxy(typeof(Action), additionalInterfaces, argumentsForConstructor))
                .Returns(expected);

            // Act
            var result = this.selector.GenerateProxy(typeof(Action), additionalInterfaces, argumentsForConstructor);

            // Assert
            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void Should_delegate_calls_to_default_generator_when_generating_non_delegate_proxy()
        {
            // Arrange
            var additionalInterfaces = new Type[] { };
            var argumentsForConstructor = new object[] { };

            var expected = A.Dummy<ProxyGeneratorResult>();
            A.CallTo(() => this.defaultProxyGenerator.GenerateProxy(typeof(object), additionalInterfaces, argumentsForConstructor))
                .Returns(expected);

            // Act
            var result = this.selector.GenerateProxy(typeof(object), additionalInterfaces, argumentsForConstructor);

            // Assert
            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void Should_delegate_calls_to_delegate_generator_asking_about_delegate_proxy()
        {
            // Arrange
            var fake = new Func<int>(() => 10);
            var invoke = fake.GetType().GetMethod("Invoke");

            var expected = A.Dummy<ProxyGeneratorResult>();
            string reason = null;
            A.CallTo(() => this.delegateProxyGenerator.MethodCanBeInterceptedOnInstance(invoke, fake, out reason))
                .Returns(true).AssignsOutAndRefParameters("reason");

            // Act
            string output = null;
            var result = this.selector.MethodCanBeInterceptedOnInstance(invoke, fake, out output);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(output, Is.EqualTo("reason"));
        }

        [Test]
        public void Should_delegate_calls_to_default_generator_asking_about_non_delegate_proxy()
        {
            // Arrange
            var fake = new object();
            var getHashCode = fake.GetType().GetMethod("GetHashCode");

            var expected = A.Dummy<ProxyGeneratorResult>();
            string reason = null;
            A.CallTo(() => this.defaultProxyGenerator.MethodCanBeInterceptedOnInstance(getHashCode, fake, out reason))
                .Returns(true).AssignsOutAndRefParameters("reason");

            // Act
            string output = null;
            var result = this.selector.MethodCanBeInterceptedOnInstance(getHashCode, fake, out output);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(output, Is.EqualTo("reason"));
        }
    }
}
