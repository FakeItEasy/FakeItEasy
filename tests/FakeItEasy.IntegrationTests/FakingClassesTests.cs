namespace FakeItEasy.IntegrationTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class FakingClassesTests
    {
        [Fact]
        public void Should_be_able_to_get_a_fake_value_of_uri_type()
        {
            // Arrange

            // Act
            var fake = A.Fake<Uri>();

            // Assert
            Fake.GetFakeManager(fake).Should().NotBeNull("because we should be able to create a fake Uri");
        }
    }
}
