namespace FakeItEasy.Tests
{
    using FakeItEasy.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class InvokesExtensionsTests
    {
        [Test]
        public void Invokes_should_support_omitting_arguments_when_they_are_not_used()
        {
            bool actionIsInvoked = false;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Invokes(() => actionIsInvoked = true);
            fake.ActionOfOne(5);

            Assert.That(actionIsInvoked, Is.True);
        }

        [Test]
        public void Invokes_with_no_argument_should_use_invokes_with_action_having_no_arguments()
        {
            bool actionIsInvoked = false;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.Action())
                .Invokes(() => actionIsInvoked = true);
            fake.Action();

            Assert.That(actionIsInvoked, Is.True);
        }

        [Test]
        public void Invokes_with_no_argument_should_support_return_value()
        {
            const int ReturnValue = 0;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.Request())
                .Invokes(() => { })
                .Returns(ReturnValue);
            var result = fake.Request();

            Assert.That(result, Is.EqualTo(ReturnValue));
        }

        [Test]
        public void Invokes_with_1_argument_should_use_invokes_with_action_having_1_argument()
        {
            const int Argument = 5;
            bool actionIsInvoked = false;
            int? collectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Invokes((int i) =>
                             {
                                 actionIsInvoked = true;
                                 collectedArgument = i;
                             });
            fake.ActionOfOne(Argument);

            Assert.That(actionIsInvoked, Is.True);
            Assert.That(collectedArgument, Is.EqualTo(Argument));
        }

        [Test]
        public void Invokes_with_1_argument_should_support_overloads()
        {
            const string Argument = "argument";
            bool actionIsInvoked = false;
            string collectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfOne(A<string>._))
                .Invokes((string s) =>
                            {
                                actionIsInvoked = true;
                                collectedArgument = s;
                            });
            fake.ActionOfOne(Argument);

            Assert.That(actionIsInvoked, Is.True);
            Assert.That(collectedArgument, Is.EqualTo(Argument));
        }

        [Test]
        public void Invokes_with_1_argument_should_support_return_value()
        {
            const int ReturnValue = 0;
            const int Argument = 5;
            int? collectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfOne(A<int>._))
                .Invokes((int i) => { })
                .Returns(ReturnValue);
            var result = fake.RequestOfOne(Argument);

            Assert.That(result, Is.EqualTo(ReturnValue));
        }

        [Test]
        public void Invokes_with_1_argument_should_throw_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Invokes((int i) => { });
            TestDelegate act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32)");
        }

        [Test]
        public void Invokes_with_1_argument_should_throw_exception_when_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Invokes((string s) => { });
            TestDelegate act = () => fake.ActionOfOne(5);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.String)");
        }

        [Test]
        public void Invokes_with_2_arguments_should_use_invokes_with_action_having_2_arguments()
        {
            const int FirstArgument = 5;
            const int SecondArgument = 8;

            bool actionIsInvoked = false;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Invokes((int i, int j) =>
                             {
                                 actionIsInvoked = true;
                                 firstCollectedArgument = i;
                                 secondCollectedArgument = j;
                             });
            fake.ActionOfTwo(FirstArgument, SecondArgument);

            Assert.That(actionIsInvoked, Is.True);
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
        }

        [Test]
        public void Invokes_with_2_arguments_should_support_overloads()
        {
            const string FirstArgument = "first argument";
            const string SecondArgument = "second argument";

            bool actionIsInvoked = false;
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(A<string>._, A<string>._))
                .Invokes((string s, string t) =>
                            {
                                actionIsInvoked = true;
                                firstCollectedArgument = s;
                                secondCollectedArgument = t;
                            });
            fake.ActionOfTwo(FirstArgument, SecondArgument);

            Assert.That(actionIsInvoked, Is.True);
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
        }

        [Test]
        public void Invokes_with_2_arguments_should_support_return_value()
        {
            const int ReturnValue = 0;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfTwo(A<int>._, A<int>._))
                .Invokes((int i, int j) => { })
                .Returns(ReturnValue);
            var result = fake.RequestOfTwo(5, 8);

            Assert.That(result, Is.EqualTo(ReturnValue));
        }

        [Test]
        public void Invokes_with_2_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfOne(A<int>._))
                .Invokes((int i, int j) => { });
            TestDelegate act = () => fake.ActionOfOne(5);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32)", "(System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_2_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Invokes((string s, int i) => { });
            TestDelegate act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.String, System.Int32)");
        }

        [Test]
        public void Invokes_with_2_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Invokes((int i, string s) => { });
            TestDelegate act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.String)");
        }

        [Test]
        public void Invokes_with_3_arguments_should_use_invokes_with_action_having_3_arguments()
        {
            const int FirstArgument = 5;
            const int SecondArgument = 8;
            const int ThirdArgument = 13;

            bool actionIsInvoked = false;
            int? firstCollectedArgument = null;
            int? secondCollectedArgument = null;
            int? thirdCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k) =>
                             {
                                 actionIsInvoked = true;
                                 firstCollectedArgument = i;
                                 secondCollectedArgument = j;
                                 thirdCollectedArgument = k;
                             });
            fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument);

            Assert.That(actionIsInvoked, Is.True);
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
        }

        [Test]
        public void Invokes_with_3_arguments_should_support_overloads()
        {
            const string FirstArgument = "first argument";
            const string SecondArgument = "second argument";
            const string ThirdArgument = "third argument";

            bool actionIsInvoked = false;
            string firstCollectedArgument = null;
            string secondCollectedArgument = null;
            string thirdCollectedArgument = null;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(A<string>._, A<string>._, A<string>._))
                .Invokes((string s, string t, string u) =>
                            {
                                actionIsInvoked = true;
                                firstCollectedArgument = s;
                                secondCollectedArgument = t;
                                thirdCollectedArgument = u;
                            });
            fake.ActionOfThree(FirstArgument, SecondArgument, ThirdArgument);

            Assert.That(actionIsInvoked, Is.True);
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
        }

        [Test]
        public void Invokes_with_3_arguments_should_support_return_value()
        {
            const int ReturnValue = 0;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k) => { })
                .Returns(ReturnValue);
            var result = fake.RequestOfThree(5, 8, 13);

            Assert.That(result, Is.EqualTo(ReturnValue));
        }

        [Test]
        public void Invokes_with_3_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfTwo(A<int>._, A<int>._))
                .Invokes((int i, int j, int k) => { });
            TestDelegate act = () => fake.ActionOfTwo(5, 8);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_3_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((string s, int i, int j) => { });
            TestDelegate act = () => fake.ActionOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_3_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((int i, string s, int j) => { });
            TestDelegate act = () => fake.ActionOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void Invokes_with_3_arguments_should_throw_exception_when_third_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, string s) => { });
            TestDelegate act = () => fake.ActionOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.String)");
        }

        [Test]
        public void Invokes_with_4_arguments_should_use_invokes_with_action_having_4_arguments()
        {
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
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k, int l) =>
                             {
                                 actionIsInvoked = true;
                                 firstCollectedArgument = i;
                                 secondCollectedArgument = j;
                                 thirdCollectedArgument = k;
                                 fourthCollectedArgument = l;
                             });
            fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            Assert.That(actionIsInvoked, Is.True);
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
            Assert.That(fourthCollectedArgument, Is.EqualTo(FourthArgument));
        }

        [Test]
        public void Invokes_with_4_arguments_should_support_overloads()
        {
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
            A.CallTo(() => fake.ActionOfFour(A<string>._, A<string>._, A<string>._, A<string>._))
                .Invokes((string s, string t, string u, string v) =>
                {
                    actionIsInvoked = true;
                    firstCollectedArgument = s;
                    secondCollectedArgument = t;
                    thirdCollectedArgument = u;
                    fourthCollectedArgument = v;
                });
            fake.ActionOfFour(FirstArgument, SecondArgument, ThirdArgument, FourthArgument);

            Assert.That(actionIsInvoked, Is.True);
            Assert.That(firstCollectedArgument, Is.EqualTo(FirstArgument));
            Assert.That(secondCollectedArgument, Is.EqualTo(SecondArgument));
            Assert.That(thirdCollectedArgument, Is.EqualTo(ThirdArgument));
            Assert.That(fourthCollectedArgument, Is.EqualTo(FourthArgument));
        }

        [Test]
        public void Invokes_with_4_arguments_should_support_return_value()
        {
            const int ReturnValue = 0;

            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.RequestOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k, int l) => { })
                .Returns(ReturnValue);
            var result = fake.RequestOfFour(5, 8, 13, 21);

            Assert.That(result, Is.EqualTo(ReturnValue));
        }

        [Test]
        public void Invokes_with_4_arguments_should_throw_exception_when_argument_count_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfThree(A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k, int l) => { });
            TestDelegate act = () => fake.ActionOfThree(5, 8, 13);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_4_arguments_should_throw_exception_when_first_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((string s, int i, int j, int k) => { });
            TestDelegate act = () => fake.ActionOfFour(5, 8, 13, 20);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.String, System.Int32, System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_4_arguments_should_throw_exception_when_second_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((int i, string s, int j, int k) => { });
            TestDelegate act = () => fake.ActionOfFour(5, 8, 13, 20);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.String, System.Int32, System.Int32)");
        }

        [Test]
        public void Invokes_with_4_arguments_should_throw_exception_when_third_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, string s, int k) => { });
            TestDelegate act = () => fake.ActionOfFour(5, 8, 13, 20);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.String, System.Int32)");
        }

        [Test]
        public void Invokes_with_4_arguments_should_throw_exception_when_fourth_argument_type_does_not_match()
        {
            var fake = A.Fake<IInterface>();
            A.CallTo(() => fake.ActionOfFour(A<int>._, A<int>._, A<int>._, A<int>._))
                .Invokes((int i, int j, int k, string s) => { });
            TestDelegate act = () => fake.ActionOfFour(5, 8, 13, 20);

            AssertThatSignatureMismatchExceptionIsThrown(act, "(System.Int32, System.Int32, System.Int32, System.Int32)", "(System.Int32, System.Int32, System.Int32, System.String)");
        }

        private static void AssertThatSignatureMismatchExceptionIsThrown(TestDelegate act, string fakeSignature, string invokesSignature)
        {
            Assert.That(
                act,
                Throws.TypeOf<FakeConfigurationException>()
                    .With.Message.EqualTo("The faked method has the signature " + fakeSignature + ", but invokes was used with " + invokesSignature + "."));
        }

        public interface IInterface
        {
            void Action();
            void ActionOfOne(int i);
            void ActionOfOne(string s);
            void ActionOfTwo(int i, int j);
            void ActionOfTwo(string s, string t);
            void ActionOfThree(int i, int j, int k);
            void ActionOfThree(string s, string t, string u);
            void ActionOfFour(int i, int j, int k, int l);
            void ActionOfFour(string s, string t, string u, string v);
            int Request();
            int RequestOfOne(int i);
            int RequestOfTwo(int i, int j);
            int RequestOfThree(int i, int j, int k);
            int RequestOfFour(int i, int j, int k, int l);
        }
    }
}