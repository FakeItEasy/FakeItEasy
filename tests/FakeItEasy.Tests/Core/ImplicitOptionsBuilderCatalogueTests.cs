namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class ImplicitOptionsBuilderCatalogueTests
    {
        private readonly List<IFakeOptionsBuilder> availableOptionsBuilders;

        public ImplicitOptionsBuilderCatalogueTests()
        {
            this.availableOptionsBuilders = new List<IFakeOptionsBuilder>();
        }

        [Fact]
        public void GetImplicitOptionsBuilder_should_return_registered_options_builder()
        {
            // Arrange
            this.availableOptionsBuilders.Add(new OptionsBuilderForTypeWithDummyFactory());

            var container = this.CreateOptionsBuilder();

            // Act
            var implicitOptionsBuilder = container.GetImplicitOptionsBuilder(typeof(TypeWithOptionsBuilders));

            // Assert
            implicitOptionsBuilder.Should().BeOfType<OptionsBuilderForTypeWithDummyFactory>();
        }

        [Fact]
        public void GetImplicitOptionsBuilder_should_return_null_when_fake_type_has_no_options_builder_specified()
        {
            // Arrange
            var optionsBuilder = this.CreateOptionsBuilder();

            // Act
            var implicitOptionsBuilder = optionsBuilder.GetImplicitOptionsBuilder(typeof(TypeWithOptionsBuilders));

            // Assert
            implicitOptionsBuilder.Should().BeNull();
        }

        [Fact]
        public void BuildOptions_should_not_fail_when_more_than_one_options_builder_exists_for_a_given_type()
        {
            // Arrange
            this.availableOptionsBuilders.Add(new OptionsBuilderForTypeWithDummyFactory());
            this.availableOptionsBuilders.Add(new DuplicateOptionsBuilderForTypeWithDummyFactory());

            // Act
            var exception = Record.Exception(() => this.CreateOptionsBuilder());

            // Assert
            exception.Should().BeNull();
        }

        private ImplicitOptionsBuilderCatalogue CreateOptionsBuilder()
        {
            return new ImplicitOptionsBuilderCatalogue(this.availableOptionsBuilders);
        }

        private class OptionsBuilderForTypeWithDummyFactory : IFakeOptionsBuilder
        {
            public Priority Priority => Priority.Default;

            public bool CanBuildOptionsForFakeOfType(Type type)
            {
                return type == typeof(TypeWithOptionsBuilders);
            }

            public void BuildOptions(Type typeOfFake, IFakeOptions options)
            {
                if (options == null)
                {
                    return;
                }

                options.ConfigureFake(fake => ((TypeWithOptionsBuilders)fake).WasConfigured = true);
            }
        }

        private class DuplicateOptionsBuilderForTypeWithDummyFactory : IFakeOptionsBuilder
        {
            public Priority Priority => Priority.Default;

            public bool CanBuildOptionsForFakeOfType(Type type)
            {
                return type == typeof(TypeWithOptionsBuilders);
            }

            public void BuildOptions(Type typeOfFake, IFakeOptions options)
            {
                if (options == null)
                {
                    return;
                }

                options.ConfigureFake(fake => ((TypeWithOptionsBuilders)fake).WasConfigured = true);
            }
        }

        private class TypeWithOptionsBuilders
        {
            public bool WasConfigured { get; set; }
        }
    }
}
