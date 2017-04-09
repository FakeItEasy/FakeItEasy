namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class DummyFactoryTests
    {
        [Fact]
        public void Create_with_type_parameter_should_return_object_from_Create()
        {
            var factory = new TestableFakeFactory() as IDummyFactory;
            var created = factory.Create(typeof(SomeType));

            created.Should().BeOfType<SomeType>();
        }

        [Fact]
        public void Create_should_guard_against_bad_type_argument()
        {
            string expectedMessage = string.Format(
                "The {0} can only create dummies of type {1}.*",
                typeof(TestableFakeFactory),
                typeof(SomeType));

            var factory = new TestableFakeFactory() as IDummyFactory;

            var exception = Record.Exception(() => factory.Create(typeof(DummyFactoryTests)));

            exception.Should()
                .BeAnExceptionOfType<ArgumentException>()
                .WithMessage(expectedMessage)
                .And.ParamName.Should().Be("type");
        }

        [Fact]
        public void Built_in_factories_should_have_lower_than_default_priority()
        {
            // Arrange
            var allDummyFactories = typeof(A).GetTypeInformation().Assembly.GetTypes()

                .Where(t => t.CanBeInstantiatedAs(typeof(IDummyFactory)))
                .Select(Activator.CreateInstance)
                .Cast<IDummyFactory>();

            // Act
            var typesWithNonNegativePriority = allDummyFactories
                .Where(f => f.Priority >= Priority.Default)
                .Select(f => f.GetType());

            // Assert
            typesWithNonNegativePriority.Should().BeEmpty("because no built-in factories should have priority equal to or greater than the default");
        }

        public class SomeType
        {
        }

        public class TestableFakeFactory : DummyFactory<SomeType>
        {
            protected override SomeType Create()
            {
                return new SomeType();
            }
        }
    }
}
