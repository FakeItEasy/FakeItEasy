namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class AssertionsTests
    {
        public interface ITypeWithWriteOnlyProperty
        {
            [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Required for testing.")]
            object SetOnly { set; }
        }

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
            var fake = A.Fake<IFoo>();

            A.CallTo(() => fake.Bar()).Throws(new InvalidOperationException()).Once();

            try
            {
                fake.Bar();
            }
            catch (InvalidOperationException)
            {
            }

            A.CallTo(() => fake.Bar()).MustHaveHappened();
        }

        [Test]
        public void Should_be_able_to_assert_ordered_on_collections_of_calls()
        {
            using (var scope = Fake.CreateScope())
            {
                // Arrange
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
            using (var scope = Fake.CreateScope())
            {
                // Arrange
                var foo = A.Fake<IFoo>();

                // Act
                foo.Baz();
                foo.Bar();

                // Assert       
                Assert.That(
                    () =>
                    {
                        using (scope.OrderedAssertions())
                        {
                            A.CallTo(() => foo.Bar()).MustHaveHappened();
                            A.CallTo(() => foo.Baz()).MustHaveHappened();
                        }
                    },
                    Throws.Exception.InstanceOf<ExpectationException>());
            }
        }

        [Test]
        public void Should_throw_when_starting_new_ordered_assertions_scope_when_one_is_already_opened()
        {
            // Arrange
            using (var outerScope = Enumerable.Empty<ICompletedFakeObjectCall>().OrderedAssertions())
            {
                // Act, Assert
                Assert.That(
                    () =>
                    {
                        using (Enumerable.Empty<ICompletedFakeObjectCall>().OrderedAssertions())
                        {
                        }
                    },
                    Throws.Exception.TypeOf<InvalidOperationException>());
            }
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
            Assert.That(exception, Is.InstanceOf<ExpectationException>());
        }
    }
}
