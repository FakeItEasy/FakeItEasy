namespace FakeItEasy.Tests.Creation.DelegateProxies;

using System;
using FakeItEasy.Core;
using FakeItEasy.Creation.DelegateProxies;
using FluentAssertions;
using Xunit;

public class DelegateProxyGeneratorTests
{
    [Fact]
    public void Should_ensure_fake_call_processor_is_initialized_but_not_fetched_when_no_method_on_fake_is_called()
    {
        // Arrange
        var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();

        // Act
        var proxyGeneratorResult = DelegateProxyGenerator.GenerateProxy(typeof(Action), fakeCallProcessorProvider);

        // Assert
        var generatedProxy = proxyGeneratorResult.GeneratedProxy;
        generatedProxy.Should().NotBeNull("we need a proxy");

        A.CallTo(() => fakeCallProcessorProvider.Fetch(A<object>._)).MustNotHaveHappened();
        A.CallTo(() => fakeCallProcessorProvider.EnsureInitialized(generatedProxy!)).MustHaveHappened();
    }
}
