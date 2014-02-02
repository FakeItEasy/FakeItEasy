namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Configuration;
    using FluentAssertions;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class CallbackConfigurationExtensionsTests
    {
        public interface IInterface
        {
            void Action();

            void ActionOfOne(int number);

            void ActionOfOne(string text);

            void ActionOfTwo(int number1, int number2);

            void ActionOfTwo(string text1, string text2);

            void ActionOfThree(int number1, int number2, int number3);

            void ActionOfThree(string text1, string text2, string text3);

            void ActionOfFour(int number1, int number2, int number3, int number4);

            void ActionOfFour(string text1, string text2, string text3, string text4);

            int Request();

            int RequestOfOne(int number);

            int RequestOfTwo(int number1, int number2);

            int RequestOfThree(int number1, int number2, int number3);

            int RequestOfFour(int number1, int number2, int number3, int number4);
        }

        [Test]
        public void Invokes_should_support_omitting_arguments_when_they_are_not_used()
        {
            // Arrange
            bool actionIsInvoked = false;

            var fake = A.Fake<IInterface>();
            
            // Act
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Invokes(() => actionIsInvoked = true);

            // Assert
            fake.ActionOfOne(5);
            actionIsInvoked.Should().BeTrue();
        }

        [Test]
        public void Invokes_with_no_argument_should_use_invokes_with_action_having_no_arguments()
        {
            // Arrange
            bool actionIsInvoked = false;

            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.Action())
                .Invokes(() => actionIsInvoked = true);
            
            // Assert
            fake.Action();
            actionIsInvoked.Should().BeTrue();
        }

        [Test]
        public void Invokes_with_no_argument_and_no_returns_should_return_default_return_value()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.Request())
                .Invokes(() => { });

            // Assert
            var result = fake.Request();
            result.Should().Be(default(int));
        }

        [Test]
        public void Invokes_with_no_argument_should_support_return_value()
        {
            // Arrange
            const int ReturnValue = 0;

            var fake = A.Fake<IInterface>();
            
            // Act
            A.CallTo(() => fake.Request())
                .Invokes(() => { })
                .Returns(ReturnValue);

            // Assert
            var result = fake.Request();
            result.Should().Be(ReturnValue);
        }

        [Test]
        public void Invokes_with_1_argument_should_use_invokes_with_action_having_1_argument()
        {
            // Arrange
            const int Argument = 5;
            bool actionIsInvoked = false;
            int? collectedArgument = null;

            var fake = A.Fake<IInterface>();
            
            // Act
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Invokes((int i) =>
                             {
                                 actionIsInvoked = true;
                                 collectedArgument = i;
                             });

            // Assert
            fake.ActionOfOne(Argument);

            actionIsInvoked.Should().BeTrue();
            collectedArgument.Should().Be(Argument);
        }

        [Test]
        public void Invokes_with_1_argument_should_support_overloads()
        {
            // Arrange
            const string Argument = "argument";
            bool actionIsInvoked = false;
            string collectedArgument = null;

            var fake = A.Fake<IInterface>();
            
            // Act
            A.CallTo(() => fake.ActionOfOne(A<string>._))
                .Invokes((string s) =>
                            {
                                actionIsInvoked = true;
                                collectedArgument = s;
                            });
            
            // Assert
            fake.ActionOfOne(Argument);

            actionIsInvoked.Should().BeTrue();
            collectedArgument.Should().Be(Argument);
        }

        [Test]
        public void Invokes_with_1_argument_should_support_return_value()
        {
            // Arrange
            const int ReturnValue = 0;
            const int Argument = 5;

            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.RequestOfOne(A<int>._))
                .Invokes((int i) => { })
                .Returns(ReturnValue);

            // Assert
            var result = fake.RequestOfOne(Argument);

            result.Should().Be(ReturnValue);
        }

        [Test]
        public void Invokes_with_1_argument_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Invokes((int i) => { });

            // Assert
            Action act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32)");
        }

        [Test]
        public void Invokes_with_1_argument_should_throw_exception_when_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Invokes((string s) => { });

            // Assert
            Action act = () => fake.ActionOfOne(5);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.String)");
        }

        [Test]
        public void Invokes_with_2_arguments_should_use_invokes_with_action_having_2_arguments()
        {
            // Arrange
            const int FirstArgument = 5;
            const int SecondArgument = 8;

            bool actionIsInvoked = false;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            
            // Act
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Invokes((int i, int j) =>
                             {
                                 actionIsInvoked = true;
                                 firstCollectedArgument = i;
                                 secondCollectedArgument = j;
                             });

            // Assert
            fake.ActionOfTwo(FirstArgument, SecondArgument);

            actionIsInvoked.Should().BeTrue();
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
        }

        [Test]
        public void Invokes_with_2_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "first argument";
            const string SecondArgument = "second argument";

            bool actionIsInvoked = false;
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            
            // Act
            A.CallTo(() => fake.ActionOfTwo(A<string>._, A<string>._))
                .Invokes((string s, string t) =>
                            {
                                actionIsInvoked = true;
                                firstCollectedArgument = s;
                                secondCollectedArgument = t;
                            });

            // Assert
            fake.ActionOfTwo(FirstArgument, SecondArgument);

            actionIsInvoked.Should().BeTrue();
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
        }

        [Test]
        public void Invokes_with_2_arguments_should_support_return_value()
        {
            // Arrange
            const int ReturnValue = 0;

            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .Invokes((int i, int j) => { })
                .Returns(ReturnValue);

            // Assert
            var result = fake.RequestOfTwo(5, 8);

            result.Should().Be(ReturnValue);
        }

        [Test]
        public void Invokes_with_2_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Invokes((int i, int j) => { });

            // Assert
            Action act = () => fake.ActionOfOne(5);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_2_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Invokes((string s, int i) => { });

            // Assert
            Action act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.String, System.Int32)");
        }

        [Test]
        public void Invokes_with_2_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Invokes((int i, string s) => { });

            // Assert
            Action act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.String)");
        }

        [Test]
        public void Invokes_with_3_arguments_should_use_invokes_with_action_having_3_arguments()
        {
            // Arrange
            const int FirstArgument = 5;
            const int SecondArgument = 8;
            const int ThirdArgument = 13;

            bool actionIsInvoked = false;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int? thirdCollectedArgument = null;

            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k) =>
                             {
                                 actionIsInvoked = true;
                                 firstCollectedArgument = i;
                                 secondCollectedArgument = j;
                                 thirdCollectedArgument = k;
                             });

            // Assert
            fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument);

            actionIsInvoked.Should().BeTrue();
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
        }

        [Test]
        public void Invokes_with_3_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "first argument";
            const string SecondArgument = "second argument";
            const string ThirdArgument = "third argument";

            bool actionIsInvoked = false;
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;

            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfThree(A<string>._, A<string>._, A<string>._))
                .Invokes((string s, string t, string u) =>
                            {
                                actionIsInvoked = true;
                                firstCollectedArgument = s;
                                secondCollectedArgument = t;
                                thirdCollectedArgument = u;
                            });

            // Assert
            fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument);

            actionIsInvoked.Should().BeTrue();
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
        }

        [Test]
        public void Invokes_with_3_arguments_should_support_return_value()
        {
            // Arrange
            const int ReturnValue = 0;

            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k) => { })
                .Returns(ReturnValue);

            // Assert
            var result = fake.RequestOfThree(5, 8, 13);

            result.Should().Be(ReturnValue);
        }

        [Test]
        public void Invokes_with_3_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Invokes((int i, int j, int k) => { });

            // Assert
            Action act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_3_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            
            // Act
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((string s, int i, int j) => { });

            // Assert
            Action act = () => fake.ActionOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_3_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            
            // Act
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((int i, string s, int j) => { });

            // Assert
            Action act = () => fake.ActionOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void Invokes_with_3_arguments_should_throw_exception_when_third_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            
            // Act
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, string s) => { });
            
            // Assert
            Action act = () => fake.ActionOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.String)");
        }

        [Test]
        public void Invokes_with_4_arguments_should_use_invokes_with_action_having_4_arguments()
        {
            // Arrange
            const int FirstArgument = 5;
            const int SecondArgument = 8;
            const int ThirdArgument = 13;
            const int FourthArgument = 21;

            bool actionIsInvoked = false;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int? thirdCollectedArgument = null;
            int? fourthCollectedArgument = null;

            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k, int l) =>
                             {
                                 actionIsInvoked = true;
                                 firstCollectedArgument = i;
                                 secondCollectedArgument = j;
                                 thirdCollectedArgument = k;
                                 fourthCollectedArgument = l;
                             });

            // Assert
            fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            actionIsInvoked.Should().BeTrue();
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
            fourthCollectedArgument.Should().Be(FourthArgument);
        }

        [Test]
        public void Invokes_with_4_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "first argument";
            const string SecondArgument = "second argument";
            const string ThirdArgument = "third argument";
            const string FourthArgument = "fourth argument";

            bool actionIsInvoked = false;
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;
            string fourthCollectedArgument = null;

            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfFour(A<string>._, A<string>._, A<string>._, A<string>._))
                .Invokes((string s, string t, string u, string v) =>
                {
                    actionIsInvoked = true;
                    firstCollectedArgument = s;
                    secondCollectedArgument = t;
                    thirdCollectedArgument = u;
                    fourthCollectedArgument = v;
                });

            // Assert
            fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            actionIsInvoked.Should().BeTrue();
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
            fourthCollectedArgument.Should().Be(FourthArgument);
        }

        [Test]
        public void Invokes_with_4_arguments_should_support_return_value()
        {
            // Arrange
            const int ReturnValue = 0;

            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k, int l) => { })
                .Returns(ReturnValue);

            // Assert
            var result = fake.RequestOfFour(5, 8, 13, 21);

            result.Should().Be(ReturnValue);
        }

        [Test]
        public void Invokes_with_4_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k, int l) => { });

            // Assert
            Action act = () => fake.ActionOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_4_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((string s, int i, int j, int k) => { });

            // Assert
            Action act = () => fake.ActionOfFour(5, 8, 13, 20);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_4_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Acct
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((int i, string s, int j, int k) => { });

            // Assert
            Action act = () => fake.ActionOfFour(5, 8, 13, 20);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_4_arguments_should_throw_exception_when_third_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, string s, int k) => { });

            // Assert
            Action act = () => fake.ActionOfFour(5, 8, 13, 20);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void Invokes_with_4_arguments_should_throw_exception_when_fourth_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();

            // Act
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k, string s) => { });

            // Assert
            Action act = () => fake.ActionOfFour(5, 8, 13, 20);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.String)");
        }

        private static void AssertThatSignatureMismatchExceptionIsThrown(Action act, string fakeSignature, string invokesSignature)
        {
            var expectedMessage = "The faked method has the signature " + fakeSignature + ", but invokes was used with " + invokesSignature + ".";
            
            var exception = Record.Exception(act);

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                .WithMessage(expectedMessage);
        }
    }
}