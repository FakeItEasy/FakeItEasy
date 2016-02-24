namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

    public static class CallMatchingSpecs
    {
        public interface ITypeWithParameterArray
        {
            void MethodWithParameterArray(string arg, params string[] args);
        }

        public interface IHaveNoGenericParameters
        {
            void Bar(int baz);
        }

        public interface IHaveOneGenericParameter
        {
            void Bar<T>(T baz);
        }

        public interface IHaveTwoGenericParameters
        {
            void Bar<T1, T2>(T1 baz1, T2 baz2);
        }

        public interface IHaveARefParameter
        {
            [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Required for testing.")]
            bool CheckYourReferences(ref string refString);
        }

        public interface IHaveAnOutParameter
        {
            [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Required for testing.")]
            bool Validate([Out] string value);
        }

        [Scenario]
        public static void ParameterArrays(
            ITypeWithParameterArray fake)
        {
            "establish"
                .x(() => fake = A.Fake<ITypeWithParameterArray>());

            "when matching calls with parameter arrays"
                .x(() => fake.MethodWithParameterArray("foo", "bar", "baz"));

            "it should be able to match the call"
                .x(() => A.CallTo(() => fake.MethodWithParameterArray("foo", "bar", "baz")).MustHaveHappened());

            "it should be able to match the call with argument constraints"
                .x(() => A.CallTo(() => fake.MethodWithParameterArray(A<string>._, A<string>._, A<string>._)).MustHaveHappened());

            "it should be able to match the call mixing constraints and values"
                .x(() => A.CallTo(() => fake.MethodWithParameterArray(A<string>._, "bar", A<string>._)).MustHaveHappened());

            "it should be able to match using array syntax"
                .x(() => A.CallTo(() => fake.MethodWithParameterArray("foo", A<string[]>.That.IsSameSequenceAs(new[] { "bar", "baz" }))).MustHaveHappened());
        }

        [Scenario]
        public static void FailingMatchOfNonGenericCalls(
            IHaveNoGenericParameters fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IHaveNoGenericParameters>());

            "when failing to match non generic calls"
                .x(() =>
                    {
                        fake.Bar(1);
                        fake.Bar(2);
                        exception = Record.Exception(() => A.CallTo(() => fake.Bar(3)).MustHaveHappened());
                    });

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following call:
    FakeItEasy.Specs.CallMatchingSpecs+IHaveNoGenericParameters.Bar(3)
  Expected to find it at least once but found it #0 times among the calls:
    1: FakeItEasy.Specs.CallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 1)
    2: FakeItEasy.Specs.CallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 2)

"));
        }

        [Scenario]
        public static void FailingMatchOfGenericCalls(
            IHaveTwoGenericParameters fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IHaveTwoGenericParameters>());

            "when failing to match generic calls"
                .x(() =>
                    {
                        fake.Bar(1, 2D);
                        fake.Bar(new Generic<bool, long>(), 3);
                        exception = Record.Exception(() => A.CallTo(() => fake.Bar(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened());
                    });

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following call:
    FakeItEasy.Specs.CallMatchingSpecs+IHaveTwoGenericParameters.Bar<System.String, System.String>(<Ignored>, <Ignored>)
  Expected to find it at least once but found it #0 times among the calls:
    1: FakeItEasy.Specs.CallMatchingSpecs+IHaveTwoGenericParameters.Bar<System.Int32, System.Double>(baz1: 1, baz2: 2)
    2: FakeItEasy.Specs.CallMatchingSpecs+IHaveTwoGenericParameters.Bar<FakeItEasy.Specs.CallMatchingSpecs+Generic<System.Boolean, System.Int64>, System.Int32>(baz1: FakeItEasy.Specs.CallMatchingSpecs+Generic`2[System.Boolean,System.Int64], baz2: 3)

"));
        }

        [Scenario]
        public static void NoNonGenericCalls(
            IHaveNoGenericParameters fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IHaveNoGenericParameters>());

            "when no non generic calls"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(A<int>.Ignored)).MustHaveHappened()));

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following call:
    FakeItEasy.Specs.CallMatchingSpecs+IHaveNoGenericParameters.Bar(<Ignored>)
  Expected to find it at least once but no calls were made to the fake object.

"));
        }

        [Scenario]
        public static void NoGenericCalls(
            IHaveOneGenericParameter fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IHaveOneGenericParameter>());

            "when no generic calls"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar<Generic<string>>(A<Generic<string>>.Ignored)).MustHaveHappened()));

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following call:
    FakeItEasy.Specs.CallMatchingSpecs+IHaveOneGenericParameter.Bar<FakeItEasy.Specs.CallMatchingSpecs+Generic<System.String>>(<Ignored>)
  Expected to find it at least once but no calls were made to the fake object.

"));
        }

        [Scenario]
        public static void OutParameter(
            IDictionary<string, string> subject)
        {
            "establish"
                .x(() => subject = A.Fake<IDictionary<string, string>>());

            "when matching a call with an out parameter"
                .x(() =>
                    {
                        string outString = "a constraint string";
                        A.CallTo(() => subject.TryGetValue("any key", out outString))
                            .Returns(true);
                    });

            "it should match without regard to out parameter value"
                .x(() =>
                    {
                        string outString = "a different string";

                        subject.TryGetValue("any key", out outString)
                            .Should().BeTrue();
                    });

            "it should assign the constraint value to the out parameter"
                .x(() =>
                    {
                        string outString = "a different string";

                        subject.TryGetValue("any key", out outString);

                        outString.Should().Be("a constraint string");
                    });
        }

        [Scenario]
        public static void FailingMatchOfOutParameter(
            IDictionary<string, string> subject,
            Exception exception)
        {
            "establish"
                .x(() => subject = A.Fake<IDictionary<string, string>>());

            "when failing to match a call with an out parameter"
                .x(() =>
                    {
                        string outString = null;

                        exception =
                            Record.Exception(
                                () => A.CallTo(() => subject.TryGetValue("any key", out outString))
                                    .MustHaveHappened());
                    });

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following call:
    System.Collections.Generic.IDictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]].TryGetValue(""any key"", <out parameter>)
  Expected to find it at least once but no calls were made to the fake object.

"));
        }

        [Scenario]
        public static void RefParameter(
            IHaveARefParameter subject)
        {
            "establish"
                .x(() => subject = A.Fake<IHaveARefParameter>());

            "when matching a call with a ref parameter"
                .x(() =>
                    {
                        string refString = "a constraint string";
                        A.CallTo(() => subject.CheckYourReferences(ref refString))
                            .Returns(true);
                    });

            "it should match when ref parameter value matches"
                .x(() =>
                    {
                        string refString = "a constraint string";

                        subject.CheckYourReferences(ref refString)
                            .Should().BeTrue();
                    });

            "it should not match when ref parameter value does not match"
                .x(() =>
                    {
                        string refString = "a different string";

                        subject.CheckYourReferences(ref refString)
                            .Should().BeFalse();
                    });

            "it should assign the constraint value to the ref parameter"
                .x(() =>
                    {
                        string refString = "a constraint string";

                        subject.CheckYourReferences(ref refString);

                        refString.Should().Be("a constraint string");
                    });
        }

        /// <summary>
        /// <see cref="OutAttribute"/> can be applied to parameters that are not
        /// <c>out</c> parameters.
        /// One example is the array parameter in <see cref="System.IO.Stream.Read"/>.
        /// Ensure that such parameters are not confused with <c>out</c> parameters.
        /// </summary>
        /// <param name="subject">The subject of the test.</param>
        [Scenario]
        public static void ParameterHavingAnOutAttribute(
             IHaveAnOutParameter subject)
        {
            "establish"
                .x(() => subject = A.Fake<IHaveAnOutParameter>());

            "when matching a call with a parameter having an out attribute"
                .x(() => A.CallTo(() => subject.Validate("a constraint string"))
                             .Returns(true));

            "it should match when ref parameter value matches"
                .x(() => subject.Validate("a constraint string")
                             .Should().BeTrue());

            "it should not match when ref parameter value does not match"
                .x(() => subject.Validate("a different string")
                             .Should().BeFalse());
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "InvalidOperationException", Justification = "It's an identifier")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CallTo", Justification = "It's an identifier")]
        [Scenario]
        public static void IgnoredArgumentConstraintOutsideCallSpec(
            Exception exception)
        {
            "When A<T>.Ignored is used outside a call specification"
                .x(() => exception = Record.Exception(() => A<string>.Ignored));

            "Then it should throw an InvalidOperationException"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());

            "And the exception message should explain why it's invalid"
                .x(() => exception.Message.Should().Be("A<T>.Ignored, A<T>._, and A<T>.That can only be used in the context of a call specification with A.CallTo()"));
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "InvalidOperationException", Justification = "Because it's the way it should be written")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CallTo", Justification = "Because it's the way it should be written")]
        [Scenario]
        public static void ThatArgumentConstraintOutsideCallSpec(
            Exception exception)
        {
            "When A<T>.That is used outside a call specification"
                .x(() => exception = Record.Exception(() => A<string>.That.Not.IsNullOrEmpty()));

            "Then it should throw an InvalidOperationException"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());

            "And the exception message should explain why it's invalid"
                .x(() => exception.Message.Should().Be("A<T>.Ignored, A<T>._, and A<T>.That can only be used in the context of a call specification with A.CallTo()"));
        }

        public class Generic<T>
        {
        }

        public class Generic<T1, T2>
        {
        }
    }
}
