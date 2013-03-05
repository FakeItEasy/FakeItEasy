namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class ThrowsExtensionsTests
    {
        public interface IInterface
        {
            void ActionOfOne(int i);

            void ActionOfOne(string s);

            void ActionOfTwo(int i, int j);

            void ActionOfTwo(string s, string t);

            void ActionOfThree(int i, int j, int k);

            void ActionOfThree(string s, string t, string u);

            void ActionOfFour(int i, int j, int k, int l);

            void ActionOfFour(string s, string t, string u, string v);
        }

        [Test]
        public void Throws_with_1_argument_should_throw_exception_and_provide_argument_for_consumption()
        {
            const int Argument = 2;
            int? collectedArgument = null;
            var exceptionToThrow = new Exception();

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfOne(Argument)).Throws((int i) =>
            {
                collectedArgument = i;
                return exceptionToThrow;
            });

            TestDelegate act = () => fake.ActionOfOne(Argument);

            Assert.That(act, Throws.Exception.EqualTo(exceptionToThrow));
            Assert.That(collectedArgument, Is.EqualTo(Argument));
        }

        [Test]
        public void Throws_with_1_argument_should_support_overloads()
        {
            const string Argument = "Argument";
            string collectedArgument = null;
            var exceptionToThrow = new Exception();

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfOne(Argument)).Throws((string s) =>
            {
                collectedArgument = s;
                return exceptionToThrow;
            });

            TestDelegate act = () => fake.ActionOfOne(Argument);

            Assert.That(act, Throws.Exception.EqualTo(exceptionToThrow));
            Assert.That(collectedArgument, Is.EqualTo(Argument));
        }

        [Test]
        public void Throws_with_1_argument_should_throw_fake_configuration_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Throws((int i) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32)");
        }

        [Test]
        public void Throws_with_1_argument_should_throw_fake_configuration_exception_when_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Throws((string s) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfOne(5);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.String)");
        }

        [Test]
        public void Throws_with_2_arguments_should_throw_exception_and_provide_arguments_for_consumption()
        {
            const int FirstArgument = 2;
            const int SecondArgument = 5;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            var exceptionToThrow = new Exception();

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(FirstArgument, SecondArgument)).Throws((int i, int j) =>
            {
                firstCollectedArgument = i;
                secondCollectedArgument = j;
                return exceptionToThrow;
            });

            TestDelegate act = () => fake.ActionOfTwo(FirstArgument, SecondArgument);

            Assert.That(act, Throws.Exception.EqualTo(exceptionToThrow));
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
        }

        [Test]
        public void Throws_with_2_arguments_should_support_overloads()
        {
            const string FirstArgument = "First Argument";
            const string SecondArgument = "Second Argument";
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            var exceptionToThrow = new Exception();

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(FirstArgument, SecondArgument)).Throws((string s, string t) =>
            {
                firstCollectedArgument = s;
                secondCollectedArgument = t;
                return exceptionToThrow;
            });

            TestDelegate act = () => fake.ActionOfTwo(FirstArgument, SecondArgument);

            Assert.That(act, Throws.Exception.EqualTo(exceptionToThrow));
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
        }

        [Test]
        public void Throws_with_2_arguments_should_throw_fake_configuration_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Throws((int i, int j) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfOne(5);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_2_arguments_should_throw_fake_configuration_exception_when_first_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Throws((string s, int i) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.String, System.Int32)");
        }

        [Test]
        public void Throws_with_2_arguments_should_throw_fake_configuration_exception_when_second_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Throws((int i, string s) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.String)");
        }

        [Test]
        public void Throws_with_3_arguments_should_throw_exception_and_provide_arguments_for_consumption()
        {
            const int FirstArgument = 2;
            const int SecondArgument = 5;
            const int ThirdArgument = 8;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int? thirdCollectedArgument = null;
            var exceptionToThrow = new Exception();

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument)).Throws((int i, int j, int k) =>
            {
                firstCollectedArgument = i;
                secondCollectedArgument = j;
                thirdCollectedArgument = k;
                return exceptionToThrow;
            });

            TestDelegate act = () => fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument);

            Assert.That(act, Throws.Exception.EqualTo(exceptionToThrow));
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
        }

        [Test]
        public void Throws_with_3_arguments_should_support_overloads()
        {
            const string FirstArgument = "First Argument";
            const string SecondArgument = "Second Argument";
            const string ThirdArgument = "Third Argument";
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;
            var exceptionToThrow = new Exception();

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument)).Throws((string s, string t, string u) =>
            {
                firstCollectedArgument = s;
                secondCollectedArgument = t;
                thirdCollectedArgument = u;
                return exceptionToThrow;
            });

            TestDelegate act = () => fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument);

            Assert.That(act, Throws.Exception.EqualTo(exceptionToThrow));
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
        }

        [Test]
        public void Throws_with_3_arguments_should_throw_fake_configuration_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Throws((int i, int j, int k) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_3_arguments_should_throw_fake_configuration_exception_when_first_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Throws((string s, int i, int j) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfThree(2, 5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_3_arguments_should_throw_fake_configuration_exception_when_second_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Throws((int i, string s, int j) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfThree(2, 5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void Throws_with_3_arguments_should_throw_fake_configuration_exception_when_third_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Throws((int i, int j, string s) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfThree(2, 5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.String)");
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_exception_and_provide_arguments_for_consumption()
        {
            const int FirstArgument = 2;
            const int SecondArgument = 5;
            const int ThirdArgument = 8;
            const int FourthArgument = 13;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int? thirdCollectedArgument = null;
            int? fourthCollectedArgument = null;
            var exceptionToThrow = new Exception();

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument))
                .Throws((int i, int j, int k, int l) =>
            {
                firstCollectedArgument = i;
                secondCollectedArgument = j;
                thirdCollectedArgument = k;
                fourthCollectedArgument = l;
                return exceptionToThrow;
            });

            TestDelegate act = () => fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            Assert.That(act, Throws.Exception.EqualTo(exceptionToThrow));
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
            Assert.That(fourthCollectedArgument, Is.EqualTo(FourthArgument));
        }

        [Test]
        public void Throws_with_4_arguments_should_support_overloads()
        {
            const string FirstArgument = "First Argument";
            const string SecondArgument = "Second Argument";
            const string ThirdArgument = "Third Argument";
            const string FourthArgument = "Fourth Argument";
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;
            string fourthCollectedArgument = null;
            var exceptionToThrow = new Exception();

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument))
                .Throws((string s, string t, string u, string v) =>
            {
                firstCollectedArgument = s;
                secondCollectedArgument = t;
                thirdCollectedArgument = u;
                fourthCollectedArgument = v;
                return exceptionToThrow;
            });

            TestDelegate act = () => fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            Assert.That(act, Throws.Exception.EqualTo(exceptionToThrow));
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
            Assert.That(fourthCollectedArgument, Is.EqualTo(FourthArgument));
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_fake_configuration_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Throws((int i, int j, int k, int l) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_fake_configuration_exception_when_first_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Throws((string s, int i, int j, int l) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfFour(2, 5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_fake_configuration_exception_when_second_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Throws((int i, string s, int j, int l) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfFour(2, 5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32, System.Int32)");
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_fake_configuration_exception_when_third_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Throws((int i, int j, string s, int l) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfFour(2, 5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void Throws_with_4_arguments_should_throw_fake_configuration_exception_when_fourth_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Throws((int i, int j, int l, string s) => { throw new InvalidOperationException("throws action should not be executed"); });
            TestDelegate act = () => fake.ActionOfFour(2, 5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.String)");
        }

        private static void AssertThatSignatureMismatchExceptionIsThrown(TestDelegate act, string fakeSignature, string throwsSignature)
        {
            var expectedMessage = "The faked method has the signature " + fakeSignature + ", but throws was used with " + throwsSignature + ".";
            Assert.That(act, Throws.TypeOf<FakeConfigurationException>().With.Message.EqualTo(expectedMessage));
        }
    }
}