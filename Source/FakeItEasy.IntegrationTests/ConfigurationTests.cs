namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using Core;
    using ExtensionSyntax;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;
    using Tests;

    [TestFixture]
    public class ConfigurationTests
    {
        public interface IIndexed
        {
            string this[int index] { get; }
        }

        [Test]
        public void Function_call_can_be_configured_using_predicate_to_validate_arguments()
        {
            // Arrange
            var fake = A.Fake<IFoo>();

            // Act
            fake.Configure()
                .CallsTo(x => x.Baz(null, null))
                .WhenArgumentsMatch(x => true)
                .Returns(100);

            // Assert
            fake.Baz("something", "else").Should().Be(100);
        }

        [Test]
        public void Void_call_can_be_configured_using_predicate_to_validate_arguments()
        {
            // Arrange
            var fake = A.Fake<IFoo>();

            // Act
            fake.Configure()
                .CallsTo(x => x.Bar(null, null))
                .WhenArgumentsMatch(x => true)
                .Throws(new InvalidOperationException());

            // Assert
            var exception = Record.Exception(() => fake.Bar("something", "else"));
            exception.Should()
                .NotBeNull()
                .And.BeOfType<InvalidOperationException>();
        }

        [Test]
        public void Output_and_reference_parameters_can_be_configured()
        {
            // Arrange
            var fake = A.Fake<IDictionary<string, string>>();
            string outputParameter = null;

            // Act
            fake.Configure()
                .CallsTo(x => x.TryGetValue("test", out outputParameter))
                .Returns(true)
                .AssignsOutAndRefParameters("bla");

            // Assert
            fake.TryGetValue("test", out outputParameter);
            outputParameter.Should().Be("bla");
        }

        [Test]
        public void Output_and_reference_parameters_can_be_configured_lazily()
        {
            // Arrange
            var fake = A.Fake<IDictionary<string, string>>();
            string outputParameter = null;

            // Act
            fake.Configure()
                .CallsTo(x => x.TryGetValue("test", out outputParameter))
                .ReturnsLazily((string key, string value) =>
                {
                    return key == "test";
                })
                .AssignsOutAndRefParametersLazily((x) =>
                {
                    return new object[] { (x.Arguments.Get<string>("key") == "test") ? "foo" : "bar" };
                });

            // Assert
            fake.TryGetValue("test", out outputParameter);
            outputParameter.Should().Be("foo");
        }

        [Test]
        public void Void_methods_can_be_configured_by_the_all_in_one_syntax()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            A.CallTo(() => foo.Bar(A<string>._, A<string>._)).Throws(new FormatException());

            // Assert
            var exception = Record.Exception(() => foo.Bar("any", "thing"));
            exception.Should()
                .NotBeNull()
                .And.BeOfType<FormatException>();
        }

        [Test]
        public void Functions_can_be_configured_by_the_all_in_one_syntax()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            A.CallTo(() => foo.Baz(null, null)).WithAnyArguments().Returns(99);

            // Assert
            foo.Baz("any", "thing").Should().Be(99);
        }

        [Test]
        public void Should_be_able_to_configure_any_call_with_a_specific_return_type_to_return_value()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            A.CallTo(foo).WithReturnType<int>().Returns(10);
            A.CallTo(foo).WithReturnType<string>().Returns("foo");

            // Assert
            foo.SomeProperty.Should().Be(10);
            foo.Baz().Should().Be(10);
        }

        [Test]
        public void Should_be_able_to_configure_returns_lazily_when_out_and_ref_modifiers_not_provided()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            int outArg = 3;
            int refArg = 4;

            // Act
            A.CallTo(() => foo.MethodWithOutputAndReference(out outArg, ref refArg))
             .ReturnsLazily((int x, int y) => x + y);

            // Assert
            foo.MethodWithOutputAndReference(out outArg, ref refArg).Should().Be(7);
        }

        [Test]
        public void A_fake_should_be_passed_to_the_container_to_be_configured_when_created()
        {
            // Arrange
            var container = A.Fake<IFakeObjectContainer>();
            A.CallTo(container).WithReturnType<bool>().Returns(false);

            using (Fake.CreateScope(container))
            {
                // Act
                var fake = A.Fake<IFoo>();

                // Assert
                A.CallTo(() => container.ConfigureFake(typeof(IFoo), fake)).MustHaveHappened();
            }
        }

        [Test]
        public void Should_be_able_to_specify_predicates_when_configuring_any_call_on_an_object()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            A.CallTo(foo).Where(x => x.Method.Name.Equals("Bar")).Throws(new InvalidOperationException());
            A.CallTo(() => foo.Bar()).Throws(new InvalidOperationException());

            // Assert
            var bazException = Record.Exception(() => foo.Baz());
            var barException = Record.Exception(() => foo.Bar());

            bazException.Should().BeNull();
            barException.Should()
                .NotBeNull().And
                .BeOfType<InvalidOperationException>();
        }

        [Test]
        public void Should_be_able_to_configure_indexed_properties()
        {
            // Arrange
            var fake = A.Fake<IIndexed>(x => x.Strict());

            // Act
            A.CallTo(() => fake[10]).Returns("ten");

            // Assert
            fake[10].Should().Be("ten");
        }

        [Test]
        public void Should_be_able_to_intercept_protected_method()
        {
            // Arrange
            var fake = A.Fake<TypeWithProtectedMethod>();

            // Act
            A.CallTo(fake).WithReturnType<int>().Where(x => x.Method.Name == "ProtectedMethod").Returns(20);

            // Assert
            fake.CallsProtectedMethod().Should().Be(20);
        }

        public class TypeWithProtectedMethod
        {
            public int CallsProtectedMethod()
            {
                return this.ProtectedMethod();
            }

            protected virtual int ProtectedMethod()
            {
                return 10;
            }
        }
    }
}
