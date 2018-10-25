namespace FakeItEasy.Tests
{
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class ServiceLocatorTests
    {
        [Fact]
        public void Should_be_registered_as_singleton()
        {
            var first = ServiceLocator.Resolve<IFakeAndDummyManager>();
            var second = ServiceLocator.Resolve<IFakeAndDummyManager>();

            second.Should().BeSameAs(first);
        }
    }
}
