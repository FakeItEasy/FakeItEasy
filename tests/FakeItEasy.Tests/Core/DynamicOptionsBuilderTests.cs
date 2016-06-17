namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class DynamicOptionsBuilderTests
    {
        private readonly List<IFakeOptionsBuilder> availableOptionsBuilders;

        public DynamicOptionsBuilderTests()
        {
            this.availableOptionsBuilders = new List<IFakeOptionsBuilder>();
        }

        [Fact]
        public void BuildOptions_should_apply_configuration_from_registered_options_builders()
        {
            // Arrange
            this.availableOptionsBuilders.Add(new OptionsBuilderForTypeWithDummyFactory());

            var container = this.CreateOptionsBuilder();

            var fakeOptions = A.Fake<IFakeOptions>();
            var fakeTypeWithDummyFactory = new TypeWithOptionsBuilders();

            A.CallTo(() => fakeOptions.ConfigureFake(A<Action<object>>._))
                .Invokes((Action<object> action) => action(fakeTypeWithDummyFactory));

            // Act
            container.BuildOptions(typeof(TypeWithOptionsBuilders), fakeOptions);

            // Assert
            fakeTypeWithDummyFactory.WasConfigured.Should().BeTrue("because configuration should be applied");
        }

        [Fact]
        public void BuildOptions_should_do_nothing_when_fake_type_has_no_options_builder_specified()
        {
            // Arrange
            var optionsBuilder = this.CreateOptionsBuilder();
            var fakeOptions = A.Fake<IFakeOptions>();

            // Act
            optionsBuilder.BuildOptions(typeof(TypeWithOptionsBuilders), fakeOptions);

            // Assert
            A.CallTo(fakeOptions).MustNotHaveHappened();
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

        private DynamicOptionsBuilder CreateOptionsBuilder()
        {
            return new DynamicOptionsBuilder(this.availableOptionsBuilders);
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
