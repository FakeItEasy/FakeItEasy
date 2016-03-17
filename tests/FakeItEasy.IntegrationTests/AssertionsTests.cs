namespace FakeItEasy.IntegrationTests
{
    using System;
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
            var foo = A.Fake<IFoo>();

            // Act
            foo.Bar();
            foo.Baz();

            // Assert
            var context = A.SequentialCallContext();
            A.CallTo(() => foo.Bar()).MustHaveHappened().InOrder(context);
            A.CallTo(() => foo.Baz()).MustHaveHappened().InOrder(context);
        }

        [Test]
        public void Should_fail_when_calls_did_not_happen_in_specified_order()
        {
            // Arrange
            Exception exception;
            var foo = A.Fake<IFoo>();

            // Act
            foo.Baz();
            foo.Bar();

            exception = Record.Exception(() =>
            {
                var context = A.SequentialCallContext();
                A.CallTo(() => foo.Bar()).MustHaveHappened().InOrder(context);
                A.CallTo(() => foo.Baz()).MustHaveHappened().InOrder(context);
            });

            // Assert
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }

        [Test]
        public void Should_fail_when_calls_to_same_method_on_different_instances_did_not_happen_in_specified_order()
        {
            // Arrange
            Exception exception;
            var fooOne = A.Fake<IFoo>();
            var fooTwo = A.Fake<IFoo>();

            // Act
            fooOne.Bar();
            fooTwo.Bar();

            exception = Record.Exception(() =>
            {
                var context = A.SequentialCallContext();
                A.CallTo(() => fooTwo.Bar()).MustHaveHappened().InOrder(context);
                A.CallTo(() => fooOne.Bar()).MustHaveHappened().InOrder(context);
            });

            // Assert
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }

        // Reported as issue 182 (https://github.com/FakeItEasy/FakeItEasy/issues/182).
        [Test]
        public void Should_throw_ExpectationException_when_ordered_assertion_is_not_met_and_interfaces_have_common_parent()
        {
            // Arrange
            Exception exception;
            var baz = A.Fake<ISomethingBaz>();
            var qux = A.Fake<ISomethingQux>();

            // Act
            qux.QuxMethod();
            baz.BazMethod();

            var context = A.SequentialCallContext();
            A.CallTo(() => qux.QuxMethod()).MustHaveHappened().InOrder(context);
            exception = Record.Exception(() => A.CallTo(() => qux.QuxMethod()).MustHaveHappened().InOrder(context));

            // Assert
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }

        [Test]
        public void Should_not_throw_when_asserting_while_calls_are_being_made_on_the_fake()
        {
            var fake = A.Fake<ISomething>();

            fake.SomethingMethod();

            A.CallTo(fake)
                .Where(
                    call =>
                    {
                        // Simulate a concurrent call being made in another thread while the `MustHaveHappened` assertion
                        // is being evaluated. This method is unorthodox, but is deterministic, not relying on the
                        // vagaries of the thread scheduler to ensure that the calls overlap.
                        fake.SomethingMethod();
                        return true;
                    },
                    output => { })
                .MustHaveHappened();
        }
    }
}
