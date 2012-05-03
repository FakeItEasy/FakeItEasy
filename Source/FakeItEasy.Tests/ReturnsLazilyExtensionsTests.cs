﻿namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class ReturnsLazilyExtensionsTests
    {
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

        private static void AssertThatSignatureMismatchExceptionIsThrown(TestDelegate act, string fakeSignature, string invokesSignature)
        {
            Assert.That(
                act,
                Throws.TypeOf<FakeConfigurationException>()
                    .With.Message.EqualTo("The faked method has the signature " + fakeSignature + ", but invokes was used with " + invokesSignature + "."));
        }
    }

    public interface IInterface
    {
        int RequestOfOne(int i);
        string RequestOfOne(string s);
        int RequestOfTwo(int i, int j);
        string RequestOfTwo(string s, string t);
        int RequestOfThree(int i, int j, int k);
        string RequestOfThree(string s, string t, string u);
        int RequestOfFour(int i, int j, int k, int l);
        string RequestOfFour(string s, string t, string u, string v);
    }
}