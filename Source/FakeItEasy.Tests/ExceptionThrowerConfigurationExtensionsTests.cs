namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class ExceptionThrowerConfigurationExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        public interface IInterface
        {
            void ActionOfOne(int number);

            void ActionOfOne(string text);

            void ActionOfTwo(int number1, int number2);

            void ActionOfTwo(string text1, string text2);

            void ActionOfThree(int number1, int number2, int number3);

            void ActionOfThree(string text1, string text2, string text3);

            void ActionOfFour(int number1, int number2, int number3, int number4);

            void ActionOfFour(string text1, string text2, string text3, string text4);
        }

        [Test]
        public void Throws_with_1_argument_should_throw_exception_and_provide_argument_for_consumption()
        {
            // Arrange
            const int Argument = 2;
            int? collectedArgument = null;
            var exceptionToThrow = new InvalidOperationException();

            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfOne(Argument);

            // Act
            A.CallTo(() => fake.ActionOfOne(Argument)).Throws((int i) =>
            {
                collectedArgument = i;
                return exceptionToThrow;
            });

            // Assert
            var thrownException = Record.Exception(act);
            thrownException.Should().Be(exceptionToThrow);
            collectedArgument.Should().Be(Argument);
        }

        [Test]
        public void Throws_with_1_argument_should_support_overloads()
        {
            // Arrange
            const string Argument = "Argument";
            string collectedArgument = null;
            var exceptionToThrow = new InvalidOperationException();

            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfOne(Argument);

            // Act
            A.CallTo(() => fake.ActionOfOne(Argument)).Throws((string s) =>
            {
                collectedArgument = s;
                return exceptionToThrow;
            });

            // Assert
            var thrownException = Record.Exception(act);
            thrownException.Should().Be(exceptionToThrow);
            collectedArgument.Should().Be(Argument);
        }

        [Test]
        public void Throws_with_1_argument_should_throw_fake_configuration_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfTwo(5, 8);

            // Act
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Throws((int i) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32)");
        }

        [Test]
        public void Throws_with_1_argument_should_throw_fake_configuration_exception_when_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfOne(5);

            // Act
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Throws((string s) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.String)");
        }

        [Test]
        public void Throws_with_2_arguments_should_throw_exception_and_provide_arguments_for_consumption()
        {
            // Arrange
            const int FirstArgument = 2;
            const int SecondArgument = 5;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            var exceptionToThrow = new InvalidOperationException();

            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfTwo(FirstArgument, SecondArgument);

            // Act
            A.CallTo(() => fake.ActionOfTwo(FirstArgument, SecondArgument)).Throws((int i, int j) =>
            {
                firstCollectedArgument = i;
                secondCollectedArgument = j;
                return exceptionToThrow;
            });

            // Assert
            var thrownException = Record.Exception(act);
            thrownException.Should().Be(exceptionToThrow);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
        }

        [Test]
        public void Throws_with_2_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "First Argument";
            const string SecondArgument = "Second Argument";
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            var exceptionToThrow = new InvalidOperationException();

            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfTwo(FirstArgument, SecondArgument);

            // Act
            A.CallTo(() => fake.ActionOfTwo(FirstArgument, SecondArgument)).Throws((string s, string t) =>
            {
                firstCollectedArgument = s;
                secondCollectedArgument = t;
                return exceptionToThrow;
            });

            // Assert
            var thrownException = Record.Exception(act);
            thrownException.Should().Be(exceptionToThrow);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
        }

        [Test]
        public void Throws_with_2_arguments_should_throw_fake_configuration_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfOne(5);
            
            // Act
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Throws((int i, int j) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_2_arguments_should_throw_fake_configuration_exception_when_first_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfTwo(5, 8);

            // Act
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Throws((string s, int i) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.String, System.Int32)");
        }

        [Test]
        public void Throws_with_2_arguments_should_throw_fake_configuration_exception_when_second_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfTwo(5, 8);

            // Act
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Throws((int i, string s) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.String)");
        }

        [Test]
        public void Throws_with_3_arguments_should_throw_exception_and_provide_arguments_for_consumption()
        {
            // Arrange
            const int FirstArgument = 2;
            const int SecondArgument = 5;
            const int ThirdArgument = 8;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int? thirdCollectedArgument = null;
            var exceptionToThrow = new InvalidOperationException();

            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument);

            // Act
            A.CallTo(() => fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument)).Throws((int i, int j, int k) =>
            {
                firstCollectedArgument = i;
                secondCollectedArgument = j;
                thirdCollectedArgument = k;
                return exceptionToThrow;
            });

            // Assert
            var thrownException = Record.Exception(act);
            thrownException.Should().Be(exceptionToThrow);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
        }

        [Test]
        public void Throws_with_3_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "First Argument";
            const string SecondArgument = "Second Argument";
            const string ThirdArgument = "Third Argument";
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;
            var exceptionToThrow = new InvalidOperationException();

            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument);

            // Act
            A.CallTo(() => fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument)).Throws((string s, string t, string u) =>
            {
                firstCollectedArgument = s;
                secondCollectedArgument = t;
                thirdCollectedArgument = u;
                return exceptionToThrow;
            });

            // Assert
            var thrownException = Record.Exception(act);
            thrownException.Should().Be(exceptionToThrow);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
        }

        [Test]
        public void Throws_with_3_arguments_should_throw_fake_configuration_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfTwo(5, 8);

            // Act
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Throws((int i, int j, int k) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_3_arguments_should_throw_fake_configuration_exception_when_first_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfThree(2, 5, 8);

            // Act
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Throws((string s, int i, int j) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_3_arguments_should_throw_fake_configuration_exception_when_second_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfThree(2, 5, 8);

            // Act
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Throws((int i, string s, int j) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void Throws_with_3_arguments_should_throw_fake_configuration_exception_when_third_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfThree(2, 5, 8);

            // Act
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Throws((int i, int j, string s) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.String)");
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_exception_and_provide_arguments_for_consumption()
        {
            // Arrange
            const int FirstArgument = 2;
            const int SecondArgument = 5;
            const int ThirdArgument = 8;
            const int FourthArgument = 13;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int? thirdCollectedArgument = null;
            int? fourthCollectedArgument = null;
            var exceptionToThrow = new InvalidOperationException();

            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            // Act            
            A.CallTo(() => fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument))
                .Throws((int i, int j, int k, int l) =>
            {
                firstCollectedArgument = i;
                secondCollectedArgument = j;
                thirdCollectedArgument = k;
                fourthCollectedArgument = l;
                return exceptionToThrow;
            });

            // Assert
            var thrownException = Record.Exception(act);
            thrownException.Should().Be(exceptionToThrow);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
            fourthCollectedArgument.Should().Be(FourthArgument);
        }

        [Test]
        public void Throws_with_4_arguments_should_support_overloads()
        {
            // Arrange
            const string FirstArgument = "First Argument";
            const string SecondArgument = "Second Argument";
            const string ThirdArgument = "Third Argument";
            const string FourthArgument = "Fourth Argument";
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;
            string fourthCollectedArgument = null;
            var exceptionToThrow = new InvalidOperationException();

            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            // Act
            A.CallTo(() => fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument))
                .Throws((string s, string t, string u, string v) =>
            {
                firstCollectedArgument = s;
                secondCollectedArgument = t;
                thirdCollectedArgument = u;
                fourthCollectedArgument = v;
                return exceptionToThrow;
            });

            // Assert
            var thrownException = Record.Exception(act);
            thrownException.Should().Be(exceptionToThrow);
            firstCollectedArgument.Should().Be(FirstArgument);
            secondCollectedArgument.Should().Be(SecondArgument);
            thirdCollectedArgument.Should().Be(ThirdArgument);
            fourthCollectedArgument.Should().Be(FourthArgument);
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_fake_configuration_exception_when_argument_count_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfThree(5, 8, 13);

            // Act
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Throws((int i, int j, int k, int l) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_fake_configuration_exception_when_first_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfFour(2, 5, 8, 13);

            // Act
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Throws((string s, int i, int j, int l) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_fake_configuration_exception_when_second_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfFour(2, 5, 8, 13);

            // Act
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Throws((int i, string s, int j, int l) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_fake_configuration_exception_when_third_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfFour(2, 5, 8, 13);

            // Act
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Throws((int i, int j, string s, int l) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_fake_configuration_exception_when_fourth_argument_type_does_not_match()
        {
            // Arrange
            var fake = A.Fake<IInterface>();
            Action act = () => fake.ActionOfFour(2, 5, 8, 13);
            
            // Act
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Throws((int i, int j, int l, string s) => { throw new InvalidOperationException("throws action should not be executed"); });

            // Assert
            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.String)");
        }

        [Test]
        public void Should_configure_fake_to_throw_the_specified_exception()
        {
            // Arrange
            var ex = A.Dummy<Exception>();
            var config = A.Fake<IExceptionThrowerConfiguration>();

            // Act
            config.Throws(ex);

            // Assert
            A.CallTo(() => config.Throws(A<Func<IFakeObjectCall, Exception>>.That.Returns(ex))).MustHaveHappened();
        }

        [Test]
        public void Should_configure_fake_to_throw_the_specified_exception_type()
        {
            // Arrange
            var config = A.Fake<IExceptionThrowerConfiguration>();

            // Act
            config.Throws<InvalidOperationException>();

            // Assert
            A.CallTo(() => config.Throws(FuncThatReturnsExceptionOfType<InvalidOperationException>())).MustHaveHappened();
        }

        [Test]
        public void Should_configure_fake_to_throw_exceptions_returned_by_the_factory()
        {
            // Arrange
            var exception = A.Dummy<Exception>();
            var factory = new Func<Exception>(() => exception);
            var config = A.Fake<IExceptionThrowerConfiguration>();

            // Act
            config.Throws(factory);

            // Assert
            A.CallTo(() => config.Throws(A<Func<IFakeObjectCall, Exception>>.That.Returns(exception))).MustHaveHappened();
        }

        private static Func<IFakeObjectCall, Exception> FuncThatReturnsExceptionOfType<T>()
        {
            return A<Func<IFakeObjectCall, Exception>>.That.NullCheckedMatches(
                x =>
                {
                    var result = x.Invoke(null);

                    if (result == null)
                    {
                        return false;
                    }

                    return typeof(T).IsAssignableFrom(result.GetType());
                },
                x => x.Write("function that returns exception of type ").WriteArgumentValue(typeof(T)));
        }
        
        private static void AssertThatSignatureMismatchExceptionIsThrown(Action act, string fakeSignature, string throwsSignature)
        {
            var expectedMessage = "The faked method has the signature " + fakeSignature + ", but throws was used with " + throwsSignature + ".";

            act.ShouldThrow<FakeConfigurationException>().WithMessage(expectedMessage);
        }
    }
}