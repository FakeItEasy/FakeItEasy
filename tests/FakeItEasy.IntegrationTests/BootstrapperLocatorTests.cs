namespace FakeItEasy.IntegrationTests
{
    using System.Collections;
    using FluentAssertions;
    using Xunit;

    public class BootstrapperLocatorTests
    {
        [Fact]
        public void Should_find_bootstrapper_in_app_domain_during_initialization()
        {
            // Arrange

            // Act
            A.Fake<IList>(); // to make sure we've initialized FakeItEasy

            // Assert
            TestingBootstrapper.WasInstantiated.Should().BeTrue();
        }

        /// <summary>
        /// A class intended to be located during initialization.
        /// Provides no special bootstrapping behavior in order not to
        /// disrupt the other tests, but does have <see cref="WasInstantiated"/>
        /// so we can verify that it was instantiated.
        /// </summary>
        public class TestingBootstrapper : DefaultBootstrapper
        {
            public TestingBootstrapper()
            {
                WasInstantiated = true;
            }

            public static bool WasInstantiated { get; set; }
        }
    }
}
