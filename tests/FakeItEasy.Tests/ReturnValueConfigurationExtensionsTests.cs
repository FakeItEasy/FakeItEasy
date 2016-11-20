namespace FakeItEasy.Tests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class ReturnValueConfigurationExtensionsTests
    {
        public interface IInterface
        {
            Task<int> RequestOfTask();

            int RequestOfOne(int number);

            Task<int> RequestOfOneTask(int number);

            string RequestOfOne(string text);

            string RequestOfOneWithOutput(out string text);

            string RequestOfOneWithReference(ref string text);

            int RequestOfTwo(int number1, int number2);

            Task<int> RequestOfTwoTask(int number, int text);

            string RequestOfTwo(string text1, string text2);

            string RequestOfTwoWithOutputAndReference(out string text1, ref string text2);

            int RequestOfThree(int number1, int number2, int number3);

            Task<int> RequestOfThreeTask(int number, int number2, int number3);

            string RequestOfThree(string text1, string text2, string text3);

            string RequestOfThreeWithOutputAndReference(out string text1, ref string text2, string text3);

            int RequestOfFour(int number1, int number2, int number3, int number4);

            Task<int> RequestOfFourTask(int number1, int number2, int number3, int number4);

            string RequestOfFour(string text1, string text2, string text3, string text4);

            string RequestOfFourWithOutputAndReference(string text1, string text2, ref string text3, out string text4);
        }

        [Fact]
        public void Returns_should_return_configuration_returned_from_passed_in_configuration()
        {
            // Arrange
            var expectedConfig = A.Fake<IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<int>>>();
            var config = A.Fake<IReturnValueConfiguration<int>>();
            A.CallTo(() => config.ReturnsLazily(A<Func<IFakeObjectCall, int>>.That.Matches(x => x.Invoke(null) == 10))).Returns(expectedConfig);

            // Act
            var returned = config.Returns(10);

            // Assert
            returned.Should().BeSameAs(expectedConfig);
        }

        [Fact]
        public void Returns_should_return_configuration_returned_from_passed_in_configuration_task()
        {
            // Arrange
            var expectedConfig = A.Fake<IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<int>>>>();
            var config = A.Fake<IReturnValueConfiguration<Task<int>>>();
            A.CallTo(() => config.ReturnsLazily(A<Func<IFakeObjectCall, Task<int>>>.That.Matches(x => x.Invoke(null).Result == 10))).Returns(expectedConfig);

            // Act
            var returned = config.Returns(10);

            // Assert
            returned.Should().BeSameAs(expectedConfig);
        }

        [Fact]
        public void Returns_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IReturnValueConfiguration<string>>().Returns(null);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void ReturnsLazily_with_task_of_t_return_type_should_support_func_of_t_valueProducer()
        {
            // Arrange
            const int ReturnValue = 5;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTask()).ReturnsLazily(() => ReturnValue);

            // Act
            var result = fake.RequestOfTask();

            // Assert
            result.Result.Should().Be(ReturnValue);
        }

        [Fact]
        public void ReturnsLazily_with_1_argument_should_use_returns_lazily_ReturnsLazily_with_action_having_1_argument()
        {
            // Arrange
            const int Argument = 2;
            const int ReturnValue = 5;
            int? collectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(Argument)).ReturnsLazily((int i) =>
                {
                    collectedArgument = i;
                    return ReturnValue;
                });

            // Act
            var result = fake.RequestOfOne(Argument);

            // Assert
            result.Should().Be(ReturnValue);
            collectedArgument.Should().Be(Argument);
        }

        [Fact]
        public void ReturnsLazily_with_1_argument_should_support_overloads()
        {
            // Arrange
            const string Argument = "argument";
            const string ReturnValue = "Result";
            string collectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(Argument)).ReturnsLazily((string s) =>
                {
                    collectedArgument = s;
                    return ReturnValue;
                });

            // Act
            var result = fake.RequestOfOne(Argument);

            result.Should().Be(ReturnValue);
            collectedArgument.Should().Be(Argument);
        }

        [Fact]
        public void ReturnsLazily_with_1_argument_should_support_out_parameter()
        {
            // Arrange
            const string ReturnValue = "Result";
            string argument = "argument";
            string collectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOneWithOutput(out argument)).ReturnsLazily((string s) =>
            {
                collectedArgument = s;
                return ReturnValue;
            });

            // Act
            var result = fake.RequestOfOneWithOutput(out argument);

            // Assert
            result.Should().Be(ReturnValue);
            collectedArgument.Should().Be(argument);
        }

        [Fact]
        public void ReturnsLazily_with_1_argument_should_support_ref_parameter()
        {
            // Arrange
            const string ReturnValue = "Result";
            string argument = "argument";
            string collectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOneWithReference(ref argument)).ReturnsLazily((string s) =>
            {
                collectedArgument = s;
                return ReturnValue;
            });

            // Act
            var result = fake.RequestOfOneWithReference(ref argument);

            // Assert
            result.Should().Be(ReturnValue);
            collectedArgument.Should().Be(argument);
        }

        [Fact]
        public void ReturnsLazily_with_1_argument_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .ReturnsLazily((int i) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfTwo(5, 8);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_1_argument_should_throw_exception_when_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(A<int>._))
                .ReturnsLazily((string s) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfOne(5);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.String)");
        }

        [Fact]
        public void ReturnsLazily_with_task_of_t_return_type_with_1_argument_should_support_func_of_t_valueProducer()
        {
            // Arrange
            const int Argument1 = 2;
            const int ReturnValue = 5;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOneTask(Argument1)).ReturnsLazily((int argument) => ReturnValue);

            // Act
            var result = fake.RequestOfOneTask(Argument1);

            // Assert
            result.Result.Should().Be(ReturnValue);
        }

        [Fact]
        public void ReturnsLazily_with_2_arguments_should_use_returns_lazily_ReturnsLazily_with_action_having_2_arguments()
        {
            // Arrange
            const int FirstArgument = 5;
            const int SecondArgument = 8;
            const int ReturnValue = 0;

            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .ReturnsLazily((int i, int j) =>
                    {
                        firstCollectedArgument = i;
                        secondCollectedArgument = j;

                        return ReturnValue;
                    });

            // Act
            var result = fake.RequestOfTwo(FirstArgument, SecondArgument);

            // Assert
            result.Should().Be(ReturnValue);
            firstCollectedArgument.Should().HaveValue().And.Be(FirstArgument);
            secondCollectedArgument.Should().HaveValue().And.Be(SecondArgument);
        }

        [Fact]
        public void ReturnsLazily_with_2_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "first argument";
            const string SecondArgument = "second argument";
            const string ReturnValue = "Result";

            string firstCollectedArgument = null;
            string secondCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<string>._, A<string>._))
                .ReturnsLazily((string s, string t) =>
                {
                    firstCollectedArgument = s;
                    secondCollectedArgument = t;

                    return ReturnValue;
                });

            // Act
            var result = fake.RequestOfTwo(FirstArgument, SecondArgument);

            // Assert
            result.Should().Be(ReturnValue);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
        }

        [Fact]
        public void ReturnsLazily_with_2_arguments_should_support_out_and_ref()
        {
            // Arrange
            const string ReturnValue = "Result";
            string firstArgument = "first argument";
            string secondArgument = "second argument";

            string firstCollectedArgument = null;
            string secondCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwoWithOutputAndReference(out firstArgument, ref secondArgument))
                .ReturnsLazily((string s, string t) =>
                {
                    firstCollectedArgument = s;
                    secondCollectedArgument = t;

                    return ReturnValue;
                });

            // Act
            var result = fake.RequestOfTwoWithOutputAndReference(out firstArgument, ref secondArgument);

            // Assert
            result.Should().Be(ReturnValue);
            firstCollectedArgument.Should().Be(firstArgument);
            secondCollectedArgument.Should().Be(secondArgument);
        }

        [Fact]
        public void ReturnsLazily_with_2_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(A<int>._))
                .ReturnsLazily((int i, int j) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfOne(5);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.Int32, System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_2_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .ReturnsLazily((string s, int i) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfTwo(5, 8);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.String, System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_2_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .ReturnsLazily((int i, string s) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfTwo(5, 8);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.String)");
        }

        [Fact]
        public void ReturnsLazily_with_task_of_t_return_type_with_2_argument_should_support_func_of_t_valueProducer()
        {
            // Arrange
            const int Argument1 = 1;
            const int Argument2 = 2;
            const int ReturnValue = 5;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwoTask(Argument1, Argument2)).ReturnsLazily((int argument1, int argument2) => ReturnValue);

            // Act
            var result = fake.RequestOfTwoTask(Argument1, Argument2);

            // Assert
            result.Result.Should().Be(ReturnValue);
        }

        [Fact]
        public void ReturnsLazily_with_3_arguments_should_use_returns_lazily_ReturnsLazily_with_action_having_3_arguments()
        {
            // Arrange
            const int FirstArgument = 5;
            const int SecondArgument = 8;
            const int ThirdArgument = 13;
            const int ReturnValue = 0;

            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int? thirdCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, int j, int k) =>
                {
                    firstCollectedArgument = i;
                    secondCollectedArgument = j;
                    thirdCollectedArgument = k;

                    return ReturnValue;
                });

            // Act
            var result = fake.RequestOfThree(FirstArgument, SecondArgument, ThirdArgument);

            // Assert
            result.Should().Be(ReturnValue);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
        }

        [Fact]
        public void ReturnsLazily_with_3_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "first argument";
            const string SecondArgument = "second argument";
            const string ThirdArgument = "third argument";
            const string ReturnValue = "Result";

            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<string>._, A<string>._, A<string>._))
                .ReturnsLazily((string s, string t, string u) =>
                {
                    firstCollectedArgument = s;
                    secondCollectedArgument = t;
                    thirdCollectedArgument = u;

                    return ReturnValue;
                });

            // Act
            var result = fake.RequestOfThree(FirstArgument, SecondArgument, ThirdArgument);

            // Assert
            result.Should().Be(ReturnValue);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
        }

        [Fact]
        public void ReturnsLazily_with_3_arguments_should_support_out_and_ref()
        {
            // Arrange
            string firstArgument = "first argument";
            string secondArgument = "second argument";
            const string ThirdArgument = "third argument";
            const string ReturnValue = "Result";

            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThreeWithOutputAndReference(out firstArgument, ref secondArgument, A<string>._))
                .ReturnsLazily((string s, string t, string u) =>
                {
                    firstCollectedArgument = s;
                    secondCollectedArgument = t;
                    thirdCollectedArgument = u;

                    return ReturnValue;
                });

            // Act
            var result = fake.RequestOfThreeWithOutputAndReference(out firstArgument, ref secondArgument, ThirdArgument);

            // Assert
            result.Should().Be(ReturnValue);
            firstCollectedArgument.Should().Be(firstArgument);
            secondCollectedArgument.Should().Be(secondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
        }

        [Fact]
        public void ReturnsLazily_with_3_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .ReturnsLazily((int i, int j, int k) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfTwo(5, 8);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_3_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((string s, int i, int j) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfThree(5, 8, 13);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_3_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, string s, int j) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfThree(5, 8, 13);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_3_arguments_should_throw_exception_when_third_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, string s, int j) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfThree(5, 8, 13);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_task_of_t_return_type_with_3_argument_should_support_func_of_t_valueProducer()
        {
            // Arrange
            const int Argument1 = 1;
            const int Argument2 = 2;
            const int Argument3 = 3;
            const int ReturnValue = 5;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThreeTask(Argument1, Argument2, Argument3)).ReturnsLazily((int argument1, int argument2, int argument3) => ReturnValue);

            // Act
            var result = fake.RequestOfThreeTask(Argument1, Argument2, Argument3);

            // Assert
            result.Result.Should().Be(ReturnValue);
        }

        [Fact]
        public void ReturnsLazily_with_4_arguments_should_use_returns_lazily_ReturnsLazily_with_action_having_4_arguments()
        {
            // Arrange
            const int FirstArgument = 5;
            const int SecondArgument = 8;
            const int ThirdArgument = 13;
            const int FourthArgument = 21;
            const int ReturnValue = 0;

            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int? thirdCollectedArgument = null;
            int? fourthCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, int j, int k, int l) =>
                {
                    firstCollectedArgument = i;
                    secondCollectedArgument = j;
                    thirdCollectedArgument = k;
                    fourthCollectedArgument = l;

                    return ReturnValue;
                });

            // Act
            var result = fake.RequestOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            // Assert
            result.Should().Be(ReturnValue);
            firstCollectedArgument.Should().HaveValue().And.Be(FirstArgument);
            secondCollectedArgument.Should().HaveValue().And.Be(SecondArgument);
            thirdCollectedArgument.Should().HaveValue().And.Be(ThirdArgument);
            fourthCollectedArgument.Should().HaveValue().And.Be(FourthArgument);
        }

        [Fact]
        public void ReturnsLazily_with_4_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "first argument";
            const string SecondArgument = "second argument";
            const string ThirdArgument = "third argument";
            const string FourthArgument = "fourth argument";
            const string ReturnValue = "Result";

            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;
            string fourthCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<string>._, A<string>._, A<string>._, A<string>._))
                .ReturnsLazily((string s, string t, string u, string v) =>
                {
                    firstCollectedArgument = s;
                    secondCollectedArgument = t;
                    thirdCollectedArgument = u;
                    fourthCollectedArgument = v;

                    return ReturnValue;
                });

            // Act
            var result = fake.RequestOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            // Assert
            result.Should().Be(ReturnValue);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
            fourthCollectedArgument.Should().Be(FourthArgument);
        }

        [Fact]
        public void ReturnsLazily_with_4_arguments_should_support_out_and_ref()
        {
            // Arrange
            const string FirstArgument = "first argument";
            const string SecondArgument = "second argument";
            string thirdArgument = "third argument";
            string fourthArgument = "fourth argument";
            const string ReturnValue = "Result";

            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;
            string fourthCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFourWithOutputAndReference(A<string>._, A<string>._, ref thirdArgument, out fourthArgument))
                .ReturnsLazily((string s, string t, string u, string v) =>
                {
                    firstCollectedArgument = s;
                    secondCollectedArgument = t;
                    thirdCollectedArgument = u;
                    fourthCollectedArgument = v;

                    return ReturnValue;
                });

            // Act
            var result = fake.RequestOfFourWithOutputAndReference(FirstArgument, SecondArgument, ref thirdArgument, out fourthArgument);

            // Assert
            result.Should().Be(ReturnValue);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(thirdArgument);
            fourthCollectedArgument.Should().Be(fourthArgument);
        }

        [Fact]
        public void ReturnsLazily_with_4_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, int j, int k, int l) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfThree(5, 8, 13);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_4_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((string s, int i, int j, int k) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfFour(5, 8, 13, 21);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32, System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_4_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, string s, int j, int k) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfFour(5, 8, 13, 21);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32, System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_4_arguments_should_throw_exception_when_third_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, int j, string s, int k) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfFour(5, 8, 13, 21);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.String, System.Int32)");
        }

        [Fact]
        public void ReturnsLazily_with_4_arguments_should_throw_exception_when_fourth_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, int j, int k, string s) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            Action act = () => fake.RequestOfFour(5, 8, 13, 21);

            // Act, Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.String)");
        }

        [Fact]
        public void ReturnsLazily_with_task_of_t_return_type_with_4_argument_should_support_func_of_t_valueProducer()
        {
            // Arrange
            const int Argument1 = 1;
            const int Argument2 = 2;
            const int Argument3 = 3;
            const int Argument4 = 4;
            const int ReturnValue = 5;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFourTask(Argument1, Argument2, Argument3, Argument4)).ReturnsLazily((int argument1, int argument2, int argument3, int argument4) => ReturnValue);

            // Act
            var result = fake.RequestOfFourTask(Argument1, Argument2, Argument3, Argument4);

            // Assert
            result.Result.Should().Be(ReturnValue);
        }

        [Fact]
        public void Curried_ReturnsLazily_returns_value_from_curried_function()
        {
            // Arrange
            var config = A.Fake<IReturnValueConfiguration<int>>();
            int currentValue = 10;

            // Act
            config.ReturnsLazily(() => currentValue);

            // Assert
            var curriedFunction = Fake.GetCalls(config).Single().Arguments.Get<Func<IFakeObjectCall, int>>(0);

            curriedFunction.Invoke(A.Dummy<IFakeObjectCall>()).Should().Be(currentValue);
            currentValue = 20;
            curriedFunction.Invoke(A.Dummy<IFakeObjectCall>()).Should().Be(currentValue);
        }

        [Fact]
        public void Curried_ReturnsLazily_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IReturnValueConfiguration<int>>().ReturnsLazily(() => 10);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void ReturnsNextFromSequence_should_call_returns_with_factory_that_returns_next_from_sequence_for_each_call()
        {
            // Arrange
            var sequence = new[] { 1, 2, 3 };
            var config = A.Fake<IReturnValueConfiguration<int>>();
            var call = A.Fake<IFakeObjectCall>();

            // Act
            config.ReturnsNextFromSequence(sequence);

            // Assert
            Func<Func<IFakeObjectCall, int>> factoryValidator = () => A<Func<IFakeObjectCall, int>>.That.Matches(
                x =>
                {
                    var producedSequence = new[] { x.Invoke(call), x.Invoke(call), x.Invoke(call) };
                    return producedSequence.SequenceEqual(sequence);
                },
                "Predicate");

            A.CallTo(() => config.ReturnsLazily(factoryValidator.Invoke())).MustHaveHappened();
        }

        [Fact]
        public void ReturnsNextFromSequence_should_call_returns_with_factory_that_returns_next_from_sequence_for_each_call_task()
        {
            // Arrange
            var sequence = new[] { 1, 2, 3 };
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTask()).ReturnsNextFromSequence(sequence);

            // Act
            var firstInvocationValue = fake.RequestOfTask();
            var secondInvocationValue = fake.RequestOfTask();
            var thirdInvocationValue = fake.RequestOfTask();
            var fourthInvocationValue = fake.RequestOfTask();

            // Assert
            firstInvocationValue.Result.Should().Be(sequence[0]);
            secondInvocationValue.Result.Should().Be(sequence[1]);
            thirdInvocationValue.Result.Should().Be(sequence[2]);
            fourthInvocationValue.Result.Should().Be(default(int));
        }

        [Fact]
        public void ReturnsNextFromSequence_should_set_repeat_to_the_number_of_values_in_sequence()
        {
            // Arrange
            var config = A.Fake<IReturnValueConfiguration<int>>();
            var returnedConfig = A.Fake<IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<int>>>();

            A.CallTo(() => config.ReturnsLazily(A<Func<IFakeObjectCall, int>>._)).Returns(returnedConfig);

            // Act
            config.ReturnsNextFromSequence(1, 2, 3);

            // Assert
            A.CallTo(() => returnedConfig.NumberOfTimes(3)).MustHaveHappened();
        }

        private static void AssertThatSignatureMismatchExceptionIsThrown(Action act, string fakeSignature, string returnsLazilySignature)
        {
            // Arrange
            var expectedMessage = "The faked method has the signature " + fakeSignature + ", but returns lazily was used with " + returnsLazilySignature + ".";

            var exception = Record.Exception(act);

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                .And.Message.Should().Be(expectedMessage);
        }
    }
}
