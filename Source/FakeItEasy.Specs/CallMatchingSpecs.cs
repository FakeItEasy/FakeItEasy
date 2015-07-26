namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_matching_calls_with_parameter_arrays
    {
        static ITypeWithParameterArray fake;

        Establish context = () => fake = A.Fake<ITypeWithParameterArray>();

        Because of = () => fake.MethodWithParameterArray("foo", "bar", "baz");

        It should_be_able_to_match_the_call =
            () => A.CallTo(() => fake.MethodWithParameterArray("foo", "bar", "baz")).MustHaveHappened();

        It should_be_able_to_match_the_call_with_argument_constraints =
            () => A.CallTo(() => fake.MethodWithParameterArray(A<string>._, A<string>._, A<string>._)).MustHaveHappened();

        It should_be_able_to_match_the_call_mixing_constraints_and_values =
            () => A.CallTo(() => fake.MethodWithParameterArray(A<string>._, "bar", A<string>._)).MustHaveHappened();

        It should_be_able_to_match_using_array_syntax =
            () => A.CallTo(() => fake.MethodWithParameterArray("foo", A<string[]>.That.IsSameSequenceAs(new[] { "bar", "baz" }))).MustHaveHappened();

        public interface ITypeWithParameterArray
        {
            void MethodWithParameterArray(string arg, params string[] args);
        }
    }

    public class when_failing_to_match_non_generic_calls
    {
        static IFoo fake;
        static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () =>
        {
            fake.Bar(1);
            fake.Bar(2);
            exception = Catch.Exception(() => A.CallTo(() => fake.Bar(3)).MustHaveHappened());
        };

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.Should().Be(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_failing_to_match_non_generic_calls+IFoo.Bar(3)
  Expected to find it at least once but found it #0 times among the calls:
    1: FakeItEasy.Specs.when_failing_to_match_non_generic_calls+IFoo.Bar(baz: 1)
    2: FakeItEasy.Specs.when_failing_to_match_non_generic_calls+IFoo.Bar(baz: 2)

");

        public interface IFoo
        {
            void Bar(int baz);
        }
    }

    public class when_failing_to_match_generic_calls
    {
        static IFoo fake;
        static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () =>
        {
            fake.Bar(1, 2D);
            fake.Bar(new Generic<bool, long>(), 3);
            exception = Catch.Exception(() => A.CallTo(() => fake.Bar(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened());
        };

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.Should().Be(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_failing_to_match_generic_calls+IFoo.Bar<System.String, System.String>(<Ignored>, <Ignored>)
  Expected to find it at least once but found it #0 times among the calls:
    1: FakeItEasy.Specs.when_failing_to_match_generic_calls+IFoo.Bar<System.Int32, System.Double>(baz1: 1, baz2: 2)
    2: FakeItEasy.Specs.when_failing_to_match_generic_calls+IFoo.Bar<FakeItEasy.Specs.when_failing_to_match_generic_calls+Generic<System.Boolean, System.Int64>, System.Int32>(baz1: FakeItEasy.Specs.when_failing_to_match_generic_calls+Generic`2[System.Boolean,System.Int64], baz2: 3)

");

        public interface IFoo
        {
            void Bar<T1, T2>(T1 baz1, T2 baz2);
        }

        public class Generic<T1, T2>
        {
        }
    }

    public class when_no_non_generic_calls
    {
        static IFoo fake;
        static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () => exception = Catch.Exception(() => A.CallTo(() => fake.Bar(A<int>.Ignored)).MustHaveHappened());

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.Should().Be(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_no_non_generic_calls+IFoo.Bar(<Ignored>)
  Expected to find it at least once but no calls were made to the fake object.

");

        public interface IFoo
        {
            void Bar(int baz);
        }
    }

    public class when_no_generic_calls
    {
        static IFoo fake;
        static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () => exception = Catch.Exception(() => A.CallTo(() => fake.Bar<Generic<string>>(A<Generic<string>>.Ignored)).MustHaveHappened());

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.Should().Be(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_no_generic_calls+IFoo.Bar<FakeItEasy.Specs.when_no_generic_calls+Generic<System.String>>(<Ignored>)
  Expected to find it at least once but no calls were made to the fake object.

");

        public interface IFoo
        {
            void Bar<T>(T baz);
        }

        public class Generic<T>
        {
        }
    }

    public class when_matching_a_call_with_an_out_parameter
    {
        private static IDictionary<string, string> subject;

        Establish context = () =>
        {
            subject = A.Fake<IDictionary<string, string>>();
        };

        Because of = () =>
            {
                string outString = "a constraint string";
                A.CallTo(() => subject.TryGetValue("any key", out outString))
                    .Returns(true);
            };

        It should_match_without_regard_to_out_parameter_value = () =>
        {
            string outString = "a different string";

            subject.TryGetValue("any key", out outString)
                .Should().BeTrue();
        };

        It should_assign_the_constraint_value_to_the_out_parameter = () =>
        {
            string outString = "a different string";

            subject.TryGetValue("any key", out outString);

            outString.Should().Be("a constraint string");
        };
    }

    public class when_failing_to_match_a_call_with_an_out_parameter
    {
        private static Exception exception;

        private static IDictionary<string, string> subject;

        Establish context = () =>
        {
            subject = A.Fake<IDictionary<string, string>>();
        };

        Because of = () =>
            {
                string outString = null;

                exception =
                    Catch.Exception(
                        () => A.CallTo(() => subject.TryGetValue("any key", out outString))
                            .MustHaveHappened());
            };

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.Should().Be(
            @"

  Assertion failed for the following call:
    System.Collections.Generic.IDictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]].TryGetValue(""any key"", <out parameter>)
  Expected to find it at least once but no calls were made to the fake object.

");
    }

    public class when_matching_a_call_with_a_ref_parameter
    {
        private static IHaveInterestingParameters subject;

        Establish context = () =>
        {
            subject = A.Fake<IHaveInterestingParameters>();
        };

        Because of = () =>
        {
            string refString = "a constraint string";
            A.CallTo(() => subject.CheckYourReferences(ref refString))
                .Returns(true);
        };

        It should_match_when_ref_parameter_value_matches = () =>
        {
            string refString = "a constraint string";

            subject.CheckYourReferences(ref refString)
                .Should().BeTrue();
        };

        It should_not_match_when_ref_parameter_value_does_not_match = () =>
        {
            string refString = "a different string";

            subject.CheckYourReferences(ref refString)
                .Should().BeFalse();
        };

        It should_assign_the_constraint_value_to_the_ref_parameter = () =>
        {
            string refString = "a constraint string";

            subject.CheckYourReferences(ref refString);

            refString.Should().Be("a constraint string");
        };

        public interface IHaveInterestingParameters
        {
            bool CheckYourReferences(ref string refString);
        }
    }

    /// <summary>
    /// <see cref="OutAttribute"/> can be applied to parameters that are not
    /// <c>out</c> parameters.
    /// One example is the array parameter in <see cref="System.IO.Stream.Read"/>.
    /// Ensure that such parameters are not confused with <c>out</c> parameters.
    /// </summary>
    public class when_matching_a_call_with_a_parameter_having_an_out_attribute
    {
        private static IHaveInterestingParameters subject;

        Establish context = () =>
        {
            subject = A.Fake<IHaveInterestingParameters>();
        };

        Because of = () =>
        {
            A.CallTo(() => subject.Validate("a constraint string"))
                .Returns(true);
        };

        It should_match_when_ref_parameter_value_matches = () =>
        {
            subject.Validate("a constraint string")
                .Should().BeTrue();
        };

        It should_not_match_when_ref_parameter_value_does_not_match = () =>
        {
            subject.Validate("a different string")
                .Should().BeFalse();
        };

        public interface IHaveInterestingParameters
        {
            bool Validate([Out] string value);
        }
    }
}