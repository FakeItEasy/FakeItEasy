namespace FakeItEasy.Tests
{
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class ServiceLocatorTests
    {
        [Fact]
        public void Current_should_not_be_null()
        {
            ServiceLocator.Current.Should().NotBeNull();
        }

        [Fact]
        public void Should_be_registered_as_singleton()
        {
            var first = ServiceLocator.Current.Resolve<IFakeAndDummyManager>();
            var second = ServiceLocator.Current.Resolve<IFakeAndDummyManager>();

            second.Should().BeSameAs(first);
        }
    }
}
