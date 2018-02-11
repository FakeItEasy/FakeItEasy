namespace FakeItEasy.Tests.Creation
{
    using System;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Creation.DelegateProxies;
    using FluentAssertions;
    using Xunit;

    public class ProxyGeneratorSelectorTests
    {
        private readonly DelegateProxyGenerator delegateProxyGenerator;

        private readonly IProxyGenerator defaultProxyGenerator;

        private readonly ProxyGeneratorSelector selector;

        public ProxyGeneratorSelectorTests()
        {
            this.delegateProxyGenerator = A.Fake<DelegateProxyGenerator>();
            this.defaultProxyGenerator = A.Fake<IProxyGenerator>();
            this.selector = new ProxyGeneratorSelector(this.delegateProxyGenerator, this.defaultProxyGenerator);
        }

        [Fact]
        public void Should_delegate_calls_to_delegate_generator_when_generating_delegate_proxy()
        {
            // Arrange
            var additionalInterfaces = Type.EmptyTypes;
            var argumentsForConstructor = Array.Empty<object>();
            var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();

            var expected = A.Dummy<ProxyGeneratorResult>();
            A.CallTo(() => this.delegateProxyGenerator.GenerateProxy(typeof(Action), additionalInterfaces, argumentsForConstructor, fakeCallProcessorProvider))
                .Returns(expected);

            // Act
            var result = this.selector.GenerateProxy(typeof(Action), additionalInterfaces, argumentsForConstructor, fakeCallProcessorProvider);

            // Assert
            result.Should().BeSameAs(expected);
        }

        [Fact]
        public void Should_delegate_calls_to_default_generator_when_generating_non_delegate_proxy()
        {
            // Arrange
            var additionalInterfaces = Type.EmptyTypes;
            var argumentsForConstructor = Array.Empty<object>();
            var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();

            var expected = A.Dummy<ProxyGeneratorResult>();
            A.CallTo(() => this.defaultProxyGenerator.GenerateProxy(typeof(object), additionalInterfaces, argumentsForConstructor, fakeCallProcessorProvider))
                .Returns(expected);

            // Act
            var result = this.selector.GenerateProxy(typeof(object), additionalInterfaces, argumentsForConstructor, fakeCallProcessorProvider);

            // Assert
            result.Should().BeSameAs(expected);
        }

        [Fact]
        public void Should_delegate_calls_to_delegate_generator_asking_about_delegate_proxy()
        {
            // Arrange
            var fake = new Func<int>(() => 10);
            var invoke = fake.GetType().GetMethod("Invoke");

            string reason;
            A.CallTo(() => this.delegateProxyGenerator.MethodCanBeInterceptedOnInstance(invoke, fake, out reason))
                .Returns(true).AssignsOutAndRefParameters("reason");

            // Act
            string output;
            var result = this.selector.MethodCanBeInterceptedOnInstance(invoke, fake, out output);

            // Assert
            result.Should().BeTrue();
            output.Should().Be("reason");
        }

        [Fact]
        public void Should_delegate_calls_to_default_generator_asking_about_non_delegate_proxy()
        {
            // Arrange
            var fake = new object();
            var getHashCode = fake.GetType().GetMethod("GetHashCode");

            string reason;
            A.CallTo(() => this.defaultProxyGenerator.MethodCanBeInterceptedOnInstance(getHashCode, fake, out reason)).Returns(true).AssignsOutAndRefParameters("reason");

            // Act
            string output;
            var result = this.selector.MethodCanBeInterceptedOnInstance(getHashCode, fake, out output);

            // Assert
            result.Should().BeTrue();
            output.Should().Be("reason");
        }
    }
}
