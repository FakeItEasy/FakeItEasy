namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public class CallDescriptionsInAssertionSpecs
    {
        public interface IFoo
        {
            int Bar(int i);

            int Baz { get; set; }
        }

        [Scenario]
        public void AssertedCallDescriptionForMethod(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And no call is made to the fake"
                .x(() => { });

            "When I assert that a method was called"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(42)).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception should correctly describe the asserted call"
                .x(() => exception.Message.Should().Match(
                    "*\r\n    FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Bar(i: 42)\r\n*"));
        }

        [Scenario]
        public void ActualCallDescriptionForMethod(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make a call to Bar with argument 0"
                .x(() => fake.Bar(42));

            "When I assert that the Bar method was called with argument 42"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(0)).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception should correctly describe the actual call that was made"
                .x(() => exception.Message.Should().Match(
                    "*\r\n    1: FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Bar(i: 42)\r\n*"));
        }

        [Scenario]
        public void AssertedCallDescriptionForPropertyGetter(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And no call is made to the fake"
                .x(() => { });

            "When I assert that a property getter was called"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Baz).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception should correctly describe the asserted call"
                .x(() => exception.Message.Should().Match(
                    "*\r\n    FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Baz\r\n*"));
        }

        [Scenario]
        public void ActualCallDescriptionForPropertyGetter(IFoo fake, Exception exception, int baz)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make a call to a property getter"
                .x(() => { baz = fake.Baz; });

            "When I assert that a different call was made"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(0)).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception correctly describes the actual call that was made"
                .x(() => exception.Message.Should().Match(
                    "*\r\n    1: FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Baz\r\n*"));
        }

        [Scenario]
        public void AssertedCallDescriptionForPropertySetterWithAnyValue(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And no call is made to the fake"
                .x(() => { });

            "When I assert that a property setter was called"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => fake.Baz).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception should correctly describe the asserted call"
                .x(() => exception.Message.Should().Match(
                    "*\r\n    FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Baz = <Ignored>\r\n*"));
        }

        [Scenario]
        public void AssertedCallDescriptionForPropertySetterWithConstantValue(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And no call is made to the fake"
                .x(() => { });

            "When I assert that a property setter was called"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => fake.Baz).To(42).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception should correctly describe the asserted call"
                .x(() => exception.Message.Should().Match(
                    "*\r\n    FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Baz = 42\r\n*"));
        }

        [Scenario]
        public void AssertedCallDescriptionForPropertySetterWithConstrainedValue(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And no call is made to the fake"
                .x(() => { });

            "When I assert that a property setter was called"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => fake.Baz).To(() => A<int>.That.Matches(i => i % 2 == 0, "an even number")).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception should correctly describe the asserted call"
                .x(() => exception.Message.Should().Match(
                    "*\r\n    FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Baz = <an even number>\r\n*"));
        }

        [Scenario]
        public void ActualCallDescriptionForPropertySetterWithConstantValue(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make a call to a property setter"
                .x(() => { fake.Baz = 42; });

            "When I assert that a different call was made"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(0)).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception correctly describes the actual call that was made"
                .x(() => exception.Message.Should().Match(
                    "*\r\n    1: FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Baz = 42\r\n*"));
        }
    }
}
