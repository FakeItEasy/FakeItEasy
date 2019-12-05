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

            int Bar(HasCustomValueFormatter x);

            int Bar(HasCustomValueFormatterThatReturnsNull x);

            int Bar(HasCustomValueFormatterThatThrows x);

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
                .x(() => exception.Message.Should().MatchModuloLineEndings(
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
                .x(() => exception.Message.Should().MatchModuloLineEndings(
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
                .x(() => exception.Message.Should().MatchModuloLineEndings(
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
                .x(() => exception.Message.Should().MatchModuloLineEndings(
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
                .x(() => exception.Message.Should().MatchModuloLineEndings(
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
                .x(() => exception.Message.Should().MatchModuloLineEndings(
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
                .x(() => exception.Message.Should().MatchModuloLineEndings(
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
                .x(() => exception.Message.Should().MatchModuloLineEndings(
                    "*\r\n    1: FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Baz = 42\r\n*"));
        }

        [Scenario]
        public void AssertedCallDescriptionWithCustomValueFormatter(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And the fake has a method with a parameter that has a custom argument value formatter"
                .See<IFoo>(foo => foo.Bar(new HasCustomValueFormatter()));

            "And no call is made to the fake"
                .x(() => { });

            "When I assert that the method was called"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(new HasCustomValueFormatter())).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception should correctly describe the asserted call, using the appropriate argument value formatter"
                .x(() => exception.Message.Should().MatchModuloLineEndings(
                    "*\r\n    FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Bar(x: hello world)\r\n*"));
        }

        [Scenario]
        public void AssertedCallDescriptionWithCustomValueFormatterThatReturnsNull(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And the fake has a method with a parameter that has a custom argument value formatter that returns null"
                .See<IFoo>(foo => foo.Bar(new HasCustomValueFormatterThatReturnsNull()));

            "And no call is made to the fake"
                .x(() => { });

            "When I assert that the method was called"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(new HasCustomValueFormatterThatReturnsNull())).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception should correctly describe the asserted call, using the next best argument value formatter"
                .x(() => exception.Message.Should().MatchModuloLineEndings(
                    "*\r\n    FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Bar(x: hello world)\r\n*"));
        }

        [Scenario]
        public void ExceptionInCustomArgumentValueFormatter(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And the fake has a method with a parameter that has a custom argument value formatter that throws"
                .See<IFoo>(foo => foo.Bar(new HasCustomValueFormatterThatThrows()));

            "And no call is made to the fake"
                .x(() => { });

            "When I assert that the method was called"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(new HasCustomValueFormatterThatThrows())).MustHaveHappened()));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception should correctly describe the asserted call, using the next best argument value formatter"
                .x(() => exception.Message.Should().MatchModuloLineEndings(
                    "*\r\n    FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+IFoo.Bar(x: FakeItEasy.Specs.CallDescriptionsInAssertionSpecs+HasCustomValueFormatterThatThrows)\r\n*"));
        }

        public class HasCustomValueFormatter
        {
        }

        public class HasCustomValueFormatterValueFormatter : ArgumentValueFormatter<HasCustomValueFormatter>
        {
            protected override string GetStringValue(HasCustomValueFormatter argumentValue)
            {
                return "hello world";
            }
        }

        public class HasCustomValueFormatterThatReturnsNull : HasCustomValueFormatter
        {
        }

        public class HasCustomValueFormatterThatReturnsNullValueFormatter
            : ArgumentValueFormatter<HasCustomValueFormatterThatReturnsNull>
        {
            protected override string GetStringValue(HasCustomValueFormatterThatReturnsNull argumentValue) =>
                null!;
        }

        public class HasCustomValueFormatterThatThrows
        {
        }

        public class HasCustomValueFormatterThatThrowsValueFormatter
            : ArgumentValueFormatter<HasCustomValueFormatterThatThrows>
        {
            protected override string GetStringValue(HasCustomValueFormatterThatThrows argumentValue) =>
                throw new Exception("Oops");
        }
    }
}
