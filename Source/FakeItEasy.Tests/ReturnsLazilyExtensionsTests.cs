namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class ReturnsLazilyExtensionsTests
    {
        public interface IInterface
        {
            int RequestOfOne(int number);

            string RequestOfOne(string text);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Required for testing.")]
            string RequestOfOneWithOutput(out string text);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Required for testing.")]
            string RequestOfOneWithReference(ref string text);

            int RequestOfTwo(int number1, int number2);

            string RequestOfTwo(string text1, string text2);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Required for testing.")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Required for testing.")]
            string RequestOfTwoWithOutputAndReference(out string text1, ref string text2);

            int RequestOfThree(int number1, int number2, int number3);

            string RequestOfThree(string text1, string text2, string text3);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Required for testing.")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Required for testing.")]
            string RequestOfThreeWithOutputAndReference(out string text1, ref string text2, string text3);

            int RequestOfFour(int number1, int number2, int number3, int number4);

            string RequestOfFour(string text1, string text2, string text3, string text4);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Required for testing.")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Required for testing.")]
            string RequestOfFourWithOutputAndReference(string text1, string text2, ref string text3, out string text4);
        }

        [Test]
        public void ReturnsLazily_with_1_argument_should_use_returns_lazily_with_action_having_1_argument()
        {
            const int Argument = 2;
            const int ReturnValue = 5;
            int? collectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(Argument)).ReturnsLazily((int i) =>
                {
                    collectedArgument = i;
                    return ReturnValue;
                });

            var result = fake.RequestOfOne(Argument);

            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(collectedArgument, Is.EqualTo(Argument));
        }

        [Test]
        public void ReturnsLazily_with_1_argument_should_support_overloads()
        {
            const string Argument = "argument";
            const string ReturnValue = "Result";
            string collectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(Argument)).ReturnsLazily((string s) =>
                {
                    collectedArgument = s;
                    return ReturnValue;
                });
            var result = fake.RequestOfOne(Argument);

            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(collectedArgument, Is.EqualTo(Argument));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(collectedArgument, Is.EqualTo(argument));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(collectedArgument, Is.EqualTo(argument));
        }

        [Test]
        public void ReturnsLazily_with_1_argument_should_throw_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .ReturnsLazily((int i) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_1_argument_should_throw_exception_when_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(A<int>._))
                .ReturnsLazily((string s) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfOne(5);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.String)");
        }

        [Test]
        public void ReturnsLazily_with_2_arguments_should_use_returns_lazily_with_action_having_2_arguments()
        {
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
            var result = fake.RequestOfTwo(FirstArgument, SecondArgument);

            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(firstCollectedArgument, Is.Not.Null.And.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.Not.Null.And.EqualTo(SecondArgument));
        }

        [Test]
        public void ReturnsLazily_with_2_arguments_should_support_overloads()
        {
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
            var result = fake.RequestOfTwo(FirstArgument, SecondArgument);

            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(firstCollectedArgument, Is.EqualTo(firstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(secondArgument));
        }

        [Test]
        public void ReturnsLazily_with_2_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(A<int>._))
                .ReturnsLazily((int i, int j) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfOne(5);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.Int32, System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_2_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .ReturnsLazily((string s, int i) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.String, System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_2_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .ReturnsLazily((int i, string s) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.String)");
        }

        [Test]
        public void ReturnsLazily_with_3_arguments_should_use_returns_lazily_with_action_having_3_arguments()
        {
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
            var result = fake.RequestOfThree(FirstArgument, SecondArgument, ThirdArgument);

            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(firstCollectedArgument, Is.Not.Null.And.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.Not.Null.And.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.Not.Null.And.EqualTo(ThirdArgument));
        }

        [Test]
        public void ReturnsLazily_with_3_arguments_should_support_overloads()
        {
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
            var result = fake.RequestOfThree(FirstArgument, SecondArgument, ThirdArgument);

            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(firstCollectedArgument, Is.EqualTo(firstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(secondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
        }

        [Test]
        public void ReturnsLazily_with_3_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .ReturnsLazily((int i, int j, int k) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_3_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((string s, int i, int j) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_3_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, string s, int j) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_3_arguments_should_throw_exception_when_third_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, string s, int j) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_4_arguments_should_use_returns_lazily_with_action_having_4_arguments()
        {
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
            var result = fake.RequestOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(firstCollectedArgument, Is.Not.Null.And.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.Not.Null.And.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.Not.Null.And.EqualTo(ThirdArgument));
            Assert.That(fourthCollectedArgument, Is.Not.Null.And.EqualTo(FourthArgument));
        }

        [Test]
        public void ReturnsLazily_with_4_arguments_should_support_overloads()
        {
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
            var result = fake.RequestOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
            Assert.That(fourthCollectedArgument, Is.EqualTo(FourthArgument));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo(ReturnValue));
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(thirdArgument));
            Assert.That(fourthCollectedArgument, Is.EqualTo(fourthArgument));
        }

        [Test]
        public void ReturnsLazily_with_4_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, int j, int k, int l) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_4_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((string s, int i, int j, int k) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfFour(5, 8, 13, 21);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_4_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, string s, int j, int k) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfFour(5, 8, 13, 21);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32, System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_4_arguments_should_throw_exception_when_third_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, int j, string s, int k) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfFour(5, 8, 13, 21);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void ReturnsLazily_with_4_arguments_should_throw_exception_when_fourth_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .ReturnsLazily((int i, int j, int k, string s) => { throw new InvalidOperationException("returns lazily action should not be executed"); });
            TestDelegate act = () => fake.RequestOfFour(5, 8, 13, 21);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.String)");
        }

        private static void AssertThatSignatureMismatchExceptionIsThrown(TestDelegate act, string fakeSignature, string returnsLazilySignature)
        {
            var expectedMessage = "The faked method has the signature " + fakeSignature + ", but returns lazily was used with " + returnsLazilySignature + ".";
            Assert.That(act, Throws.TypeOf<FakeConfigurationException>().With.Message.EqualTo(expectedMessage));
        }
    }
}