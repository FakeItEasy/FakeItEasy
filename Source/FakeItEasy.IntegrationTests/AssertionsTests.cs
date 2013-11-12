namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Tests;
    using NUnit.Framework;

    [TestFixture]
    public class AssertionsTests
    {
        public interface ITypeWithWriteOnlyProperty
        {
            [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Required for testing.")]
            object SetOnly { set; }
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
    }
}
