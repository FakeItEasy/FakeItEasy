namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class OutAndRefParametersConfigurationExtensionsTests
    {
        public interface IInterface
        {
            void RequestOfOne(out int numberOut);

            void RequestOfOne(out string textOut);

            void RequestOfTwo(int number1, out int numberOut);

            void RequestOfTwo(string text1, out string textOut);

            void RequestOfThree(int number1, int number2, out int numberOut);

            void RequestOfThree(string text1, string text2, out string textOut);

            void RequestOfFour(int number1, int number2, int number3, out int numberOut);

            void RequestOfFour(string text1, string text2, string text3, out string textOut);
        }

        [Fact]
        public void AssignsOutAndRefParameters_should_return_configuration_returned_from_passed_in_configuration()
        {
            // Arrange
            var expectedConfig = A.Fake<IAfterCallConfiguredWithOutAndRefParametersConfiguration<IVoidConfiguration>>();
            var config = A.Fake<IOutAndRefParametersConfiguration<IVoidConfiguration>>();
            A.CallTo(() => config.AssignsOutAndRefParametersLazily(
                    A<Func<IFakeObjectCall, ICollection<object>>>.That.Matches(x => (int)x.Invoke(null).First() == 10)))
                .Returns(expectedConfig);

            // Act
            var returned = config.AssignsOutAndRefParameters(10);

            // Assert
            returned.Should().BeSameAs(expectedConfig);
        }

        [Fact]
        public void AssignsOutAndRefParameters_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IOutAndRefParametersConfiguration<IVoidConfiguration>>().AssignsOutAndRefParameters(null);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_1_argument_should_use_ReturnsOutAndRefParameters_with_action_having_1_argument()
        {
            // Arrange
            const int OutValue = 5;
            int fakeOut;
            int result;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((int oi) => new object[] { OutValue });

            // Act
            fake.RequestOfOne(out result);

            // Assert
            result.Should().Be(OutValue);
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_1_argument_should_support_overloads()
        {
            // Arrange
            const string OutValue = "Result";
            string fakeOut;
            string result;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((string oi) => new object[] { OutValue });

            // Act
            fake.RequestOfOne(out result);

            // Assert
            result.Should().Be(OutValue);
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_1_argument_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            int fakeOut;
            A.CallTo(() => fake.RequestOfTwo(5, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((int io) =>
                {
                    throw new InvalidOperationException(
                        "assigns out and ref parameters lazily action should not be executed");
                });

            Action act = () => fake.RequestOfTwo(5, out fakeOut);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32&)", "(System.Int32)");
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_1_argument_should_throw_exception_when_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            int fakeOut;
            A.CallTo(() => fake.RequestOfOne(out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((string s) =>
                {
                    throw new InvalidOperationException(
                        "assigns out and ref parameters lazily action should not be executed");
                });

            Action act = () => fake.RequestOfOne(out fakeOut);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32&)", "(System.String)");
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_2_arguments_should_use_ReturnsOutAndRefParameters_with_action_having_2_arguments()
        {
            // Arrange
            const int Argument = 2;
            const int OutValue = 5;
            int? collectedArgument = null;
            int fakeOut;
            int result;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(Argument, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((int i, int oi) =>
                {
                    collectedArgument = i;
                    return new object[] { OutValue };
                });

            // Act
            fake.RequestOfTwo(Argument, out result);

            // Assert
            result.Should().Be(OutValue);
            collectedArgument.Should().Be(Argument);
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_2_arguments_should_support_overloads()
        {
            // Arrange
            const string Argument = "argument";
            const string OutValue = "Result";
            string collectedArgument = null;
            string fakeOut;
            string result;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(Argument, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((string s, string oi) =>
                {
                    collectedArgument = s;
                    return new object[] { OutValue };
                });

            // Act
            fake.RequestOfTwo(Argument, out result);

            // Assert
            result.Should().Be(OutValue);
            collectedArgument.Should().Be(Argument);
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_2_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            int fakeOut;
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((int i, int io) =>
                {
                    throw new InvalidOperationException(
                        "assigns out and ref parameters lazily action should not be executed");
                });

            Action act = () => fake.RequestOfThree(5, 8, out fakeOut);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(
                act, "(System.Int32, System.Int32, System.Int32&)", "(System.Int32, System.Int32)");
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_2_arguments_should_throw_exception_when_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            int fakeOut;
            A.CallTo(() => fake.RequestOfTwo(A<int>._, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((string s, int io) =>
                {
                    throw new InvalidOperationException(
                        "assigns out and ref parameters lazily action should not be executed");
                });

            Action act = () => fake.RequestOfTwo(5, out fakeOut);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(
                act, "(System.Int32, System.Int32&)", "(System.String, System.Int32)");
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_3_arguments_should_use_ReturnsOutAndRefParameters_with_action_having_3_arguments()
        {
            // Arrange
            const int FirstArgument = 2;
            const int SecondArgument = 8;
            const int OutValue = 5;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int fakeOut;
            int result;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(FirstArgument, SecondArgument, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((int i1, int i2, int oi) =>
                {
                    firstCollectedArgument = i1;
                    secondCollectedArgument = i2;
                    return new object[] { OutValue };
                });

            // Act
            fake.RequestOfThree(FirstArgument, SecondArgument, out result);

            // Assert
            result.Should().Be(OutValue);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_3_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "argument1";
            const string SecondArgument = "argument2";
            const string OutValue = "Result";
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string fakeOut;
            string result;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(FirstArgument, SecondArgument, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((string s1, string s2, string oi) =>
                {
                    firstCollectedArgument = s1;
                    secondCollectedArgument = s2;
                    return new object[] { OutValue };
                });

            // Act
            fake.RequestOfThree(FirstArgument, SecondArgument, out result);

            // Assert
            result.Should().Be(OutValue);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_3_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            int fakeOut;
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((int i1, int i2, int io) =>
                {
                    throw new InvalidOperationException(
                        "assigns out and ref parameters lazily action should not be executed");
                });

            Action act = () => fake.RequestOfFour(5, 8, 9, out fakeOut);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(
                act,
                "(System.Int32, System.Int32, System.Int32, System.Int32&)",
                "(System.Int32, System.Int32, System.Int32)");
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_3_arguments_should_throw_exception_when_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            int fakeOut;
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((string s1, string s2, int io) =>
                {
                    throw new InvalidOperationException(
                        "assigns out and ref parameters lazily action should not be executed");
                });

            Action act = () => fake.RequestOfThree(5, 8, out fakeOut);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(
                act, "(System.Int32, System.Int32, System.Int32&)", "(System.String, System.String, System.Int32)");
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_4_arguments_should_use_ReturnsOutAndRefParameters_with_action_having_4_arguments()
        {
            // Arrange
            const int FirstArgument = 2;
            const int SecondArgument = 8;
            const int ThirdArgument = 4;
            const int OutValue = 5;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int? thirdCollectedArgument = null;
            int fakeOut;
            int result;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(FirstArgument, SecondArgument, ThirdArgument, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((int i1, int i2, int i3, int oi) =>
                {
                    firstCollectedArgument = i1;
                    secondCollectedArgument = i2;
                    thirdCollectedArgument = i3;
                    return new object[] { OutValue };
                });

            // Act
            fake.RequestOfFour(FirstArgument, SecondArgument, ThirdArgument, out result);

            // Assert
            result.Should().Be(OutValue);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_4_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "argument1";
            const string SecondArgument = "argument2";
            const string ThirdArgument = "argument3";
            const string OutValue = "Result";
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;
            string fakeOut;
            string result;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(FirstArgument, SecondArgument, ThirdArgument, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((string s1, string s2, string s3, string oi) =>
                {
                    firstCollectedArgument = s1;
                    secondCollectedArgument = s2;
                    thirdCollectedArgument = s3;
                    return new object[] { OutValue };
                });

            // Act
            fake.RequestOfFour(FirstArgument, SecondArgument, ThirdArgument, out result);

            // Assert
            result.Should().Be(OutValue);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_4_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            int fakeOut;
            A.CallTo(() => fake.RequestOfTwo(A<int>._, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((int i1, int i2, int i3, int io) =>
                {
                    throw new InvalidOperationException(
                        "assigns out and ref parameters lazily action should not be executed");
                });

            Action act = () => fake.RequestOfTwo(5, out fakeOut);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(
                act, "(System.Int32, System.Int32&)", "(System.Int32, System.Int32, System.Int32, System.Int32)");
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_with_4_arguments_should_throw_exception_when_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            int fakeOut;
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, out fakeOut))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((string s1, string s2, string s3, int io) =>
                {
                    throw new InvalidOperationException(
                        "assigns out and ref parameters lazily action should not be executed");
                });

            Action act = () => fake.RequestOfFour(5, 8, 4, out fakeOut);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(
                act,
                "(System.Int32, System.Int32, System.Int32, System.Int32&)",
                "(System.String, System.String, System.String, System.Int32)");
        }

        private static void AssertThatSignatureMismatchExceptionIsThrown(
            Action act, string fakeSignature, string outAndRefSignature)
        {
            // Arrange
            var expectedMessage =
                "The faked method has the signature " +
                fakeSignature +
                ", but assigns out and ref parameters lazily was used with " +
                outAndRefSignature +
                ".";

            // Act
            var exception = Record.Exception(act);

            // Assert
            exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                .And.Message.Should().Be(expectedMessage);
        }
    }
}
