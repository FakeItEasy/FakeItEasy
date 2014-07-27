namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class AssertionsTests
    {
        public interface ISomething
        {
            void SomethingMethod();
        }

        public interface ISomethingBaz : ISomething
        {
            void BazMethod();
        }

        public interface ISomethingQux : ISomething
        {
            void QuxMethod();
        }

        [Test]
        public void Method_that_is_configured_to_throw_should_still_be_recorded()
        {
            // Arrange
            var fake = A.Fake<IFoo>();

            A.CallTo(() => fake.Bar()).Throws(new InvalidOperationException()).Once();

            // Act
            try
            {
                fake.Bar();
            }
            catch (InvalidOperationException)
            {
            }

            // Assert
            A.CallTo(() => fake.Bar()).MustHaveHappened();
        }

        [Test]
        public void Should_be_able_to_assert_ordered_on_collections_of_calls()
        {
            // Arrange
            using (var scope = Fake.CreateScope())
            {
                var foo = A.Fake<IFoo>();

                // Act
                foo.Bar();
                foo.Baz();

                // Assert            
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => foo.Bar()).MustHaveHappened();
                    A.CallTo(() => foo.Baz()).MustHaveHappened();
                }
            }
        }

        [Test]
        public void Should_fail_when_calls_did_not_happen_in_specified_order()
        {
            // Arrange
            Exception exception;

            using (var scope = Fake.CreateScope())
            {
                var foo = A.Fake<IFoo>();

                // Act
                foo.Baz();
                foo.Bar();

                exception = Record.Exception(() =>
                {
                    using (scope.OrderedAssertions())
                    {
                        A.CallTo(() => foo.Bar()).MustHaveHappened();
                        A.CallTo(() => foo.Baz()).MustHaveHappened();
                    }
                });
            }

            // Assert
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }

        [Test]
        public void Should_fail_when_calls_to_same_method_on_different_instances_did_not_happen_in_specified_order()
        {
            // Arrange
            Exception exception;

            using (var scope = Fake.CreateScope())
            {
                var fooOne = A.Fake<IFoo>();
                var fooTwo = A.Fake<IFoo>();

                // Act
                fooOne.Bar();
                fooTwo.Bar();

                exception = Record.Exception(() =>
                {
                    using (scope.OrderedAssertions())
                    {
                        A.CallTo(() => fooTwo.Bar()).MustHaveHappened();
                        A.CallTo(() => fooOne.Bar()).MustHaveHappened();
                    }
                });
            }

            // Assert
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }

        [Test]
        public void Should_throw_when_starting_new_ordered_assertions_scope_when_one_is_already_opened()
        {
            // Arrange
            Exception exception;

            using (Enumerable.Empty<ICompletedFakeObjectCall>().OrderedAssertions())
            {
                // Act
                exception = Record.Exception(
                    () =>
                    {
                        using (Enumerable.Empty<ICompletedFakeObjectCall>().OrderedAssertions())
                        {
                        }
                    });
            }

            // Assert
            exception.Should().BeAnExceptionOfType<InvalidOperationException>();
        }

        // Reported as issue 182 (https://github.com/FakeItEasy/FakeItEasy/issues/182).
        [Test]
        public void Should_throw_ExpectationException_when_ordered_assertion_is_not_met_and_interfaces_have_common_parent()
        {
            // Arrange
            Exception exception;
            var baz = A.Fake<ISomethingBaz>();
            var qux = A.Fake<ISomethingQux>();

            using (var scope = Fake.CreateScope())
            {
                // Act
                qux.QuxMethod();
                baz.BazMethod();

                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => qux.QuxMethod()).MustHaveHappened();
                    exception = Record.Exception(() => A.CallTo(() => qux.QuxMethod()).MustHaveHappened());
                }
            }

            // Assert
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }
    }
}
