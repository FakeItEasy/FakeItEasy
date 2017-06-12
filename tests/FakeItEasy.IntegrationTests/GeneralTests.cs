namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class GeneralTests
    {
        public interface ITypeWithFakeableProperties
        {
            IEnumerable<object> Collection { get; }

            IFoo Foo { get; }
        }

        [Fact]
        public void Faked_object_with_fakeable_properties_should_have_fake_as_default_value()
        {
            // Arrange

            // Act
            var fake = A.Fake<ITypeWithFakeableProperties>();

            // Assert
            fake.Collection.Should().NotBeNull();
            fake.Foo.Should().NotBeNull();
        }

        [Fact]
        public void Faked_object_with_method_that_returns_fakeable_type_should_return_fake_by_default()
        {
            // Arrange
            var fake = A.Fake<IEnumerable<string>>();

            // Act
            var enumerator = fake.GetEnumerator();

            // Assert
            enumerator.Should().NotBeNull();
            Fake.GetFakeManager(enumerator).Should().NotBeNull();
        }

        [Fact]
        public void Faked_object_with_additional_attributes_should_create_a_type_with_those_attributes()
        {
            // Arrange

            // Act
            var fake = A.Fake<IEmpty>(a => a.WithAttributes(() => new ForTestAttribute()));

            // Assert
            fake.GetType().GetTypeInfo().GetCustomAttributes(typeof(ForTestAttribute), true).Should().HaveCount(1);
        }

        [Fact]
        public void Additional_attributes_should_not_be_added_to_the_next_fake()
        {
            // Arrange
            A.Fake<IEmpty>(a => a.WithAttributes(() => new ForTestAttribute()));

            // Act
            var secondFake = A.Fake<IFormattable>();

            // Assert
            secondFake.GetType().GetTypeInfo().GetCustomAttributes(typeof(ForTestAttribute), true).Should().BeEmpty();
        }

        [Fact]
        public void Should_not_be_able_to_fake_a_guid()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => A.Fake<Guid>());

            // Assert
            exception.Should().BeAnExceptionOfType<FakeCreationException>();
        }

        [Fact]
        public void ErrorMessage_when_type_cannot_be_faked_should_specify_non_resolvable_constructor_arguments()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => A.Fake<NonResolvableType>());

            // Assert
            const string ExpectedMessage = @"
    The following constructors were not tried:
      (FakeItEasy.Tests.IFoo, *FakeItEasy.IntegrationTests.GeneralTests+NoInstanceType)
      (*FakeItEasy.IntegrationTests.GeneralTests+NoInstanceType)

      Types marked with * could not be resolved. Please provide a Dummy Factory to enable these constructors.

";
            exception.Should()
                .BeAnExceptionOfType<FakeCreationException>().And
                .Message.Should().Contain(ExpectedMessage);
        }

        [Fact]
        public void ErrorMessage_when_configuring_void_call_that_cannot_be_configured_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => fake.NonVirtualVoidMethod(string.Empty, 1)).DoesNothing());

            // Assert
            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                .Message.Should().Contain("Non-virtual");
        }

        [Fact]
        public void ErrorMessage_when_configuring_function_call_that_cannot_be_configured_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => fake.NonVirtualFunction(string.Empty, 1)).Returns(10));

            // Assert
            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                .Message.Should().Contain("Non-virtual");
        }

        [Fact]
        public void ErrorMessage_when_configuring_generic_function_call_that_cannot_be_configured_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => fake.GenericNonVirtualFunction<int>()).Returns(10));

            // Assert
            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                .Message.Should().Contain("Non-virtual");
        }

        [Fact]
        public void Should_be_able_to_generate_class_fake_that_implements_additional_interface()
        {
            // Arrange
            var fake = A.Fake<FakeableClass>(x => x.Implements(typeof(IFoo)).Implements(typeof(IFormattable)));

            // Act

            // Assert
            fake.Should()
                .BeAssignableTo<IFoo>().And
                .BeAssignableTo<IFormattable>().And
                .BeAssignableTo<FakeableClass>();
        }

        [Fact]
        public void Should_be_able_to_generate_class_fake_that_implements_additional_interface_using_generic()
        {
            // Arrange
            var fake = A.Fake<FakeableClass>(x => x.Implements<IFoo>().Implements<IFormattable>());

            // Act

            // Assert
            fake.Should()
                .BeAssignableTo<IFoo>().And
                .BeAssignableTo<IFormattable>().And
                .BeAssignableTo<FakeableClass>();
        }

        [Fact]
        public void Should_be_able_to_generate_interface_fake_that_implements_additional_interface()
        {
            // Arrange
            var fake = A.Fake<IFoo>(x => x.Implements(typeof(IFormatProvider)).Implements(typeof(IFormattable)));

            // Act

            // Assert
            fake.Should()
                .BeAssignableTo<IFoo>().And
                .BeAssignableTo<IFormattable>().And
                .BeAssignableTo<IFormatProvider>();
        }

        [Fact]
        public void Should_be_able_to_generate_interface_fake_that_implements_additional_interface_using_generic()
        {
            // Arrange
            var fake = A.Fake<IFoo>(x => x.Implements<IFormatProvider>().Implements<IFormattable>());

            // Act

            // Assert
            fake.Should()
                .BeAssignableTo<IFoo>().And
                .BeAssignableTo<IFormattable>().And
                .BeAssignableTo<IFormatProvider>();
        }

        [Fact]
        public void FakeCollection_should_return_list_where_all_objects_are_fakes()
        {
            // Arrange

            // Act
            var result = A.CollectionOfFake<IFoo>(10);

            // Assert
            result.Should().BeAssignableTo<IList<IFoo>>().And
                .OnlyContain(foo => foo != null && Fake.GetFakeManager(foo) != null);
        }

        [Fact]
        public void DummyCollection_should_return_correct_number_of_dummies()
        {
            // Arrange

            // Act
            var result = A.CollectionOfDummy<IFoo>(10);

            // Assert
            result.Should().HaveCount(10);
        }

        [Fact]
        public void Returns_from_sequence_only_applies_the_number_as_many_times_as_the_number_of_specified_values()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            A.CallTo(() => foo.Baz()).Throws(new InvalidOperationException());
            A.CallTo(() => foo.Baz()).ReturnsNextFromSequence(1, 2);

            foo.Baz();
            foo.Baz();

            // Act
            var exception = Record.Exception(() => foo.Baz());

            // Assert
            exception.Should().BeAnExceptionOfType<InvalidOperationException>();
        }

        public class NonResolvableType
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "foo", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "bar", Justification = "Required for testing.")]
            public NonResolvableType(IFoo foo, NoInstanceType bar)
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "bar", Justification = "Required for testing.")]
            protected NonResolvableType(NoInstanceType bar)
            {
            }
        }

        public class NoInstanceType
        {
            private NoInstanceType()
            {
            }
        }

        public class FakeableClass
        {
            public virtual void Foo()
            {
            }
        }

        public class TypeWithNonConfigurableMethod
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "otherArgument", Justification = "Required for testing.")]
            public void NonVirtualVoidMethod(string argument, int otherArgument)
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "otherArgument", Justification = "Required for testing.")]
            public int NonVirtualFunction(string argument, int otherArgument)
            {
                return 1;
            }

            public T GenericNonVirtualFunction<T>()
            {
                return default(T);
            }
        }
    }
}
