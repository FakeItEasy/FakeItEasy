namespace FakeItEasy.Examples
{
    using FakeItEasy.Examples.ExampleObjects;

    public class Asserting
    {
        public void Asserting_on_calls()
        {
            var factory = A.Fake<IWidgetFactory>();

            // These would throw an exception since no call has been made.
            A.CallTo(() => factory.Create()).MustHaveHappened();
            A.CallTo(() => factory.Create()).MustHaveHappenedOnceOrMore();
            A.CallTo(() => factory.Create()).MustHaveHappenedTwiceOrMore();
            A.CallTo(() => factory.Create()).MustHaveHappened(10, Times.OrMore);

            // On the other hand, these would not throw.
            A.CallTo(() => factory.Create()).MustNotHaveHappened();
            A.CallTo(() => factory.Create()).MustHaveHappenedOnceOrLess();
            A.CallTo(() => factory.Create()).MustHaveHappenedTwiceOrLess();
            A.CallTo(() => factory.Create()).MustHaveHappened(10, Times.OrLess);

            // The number of times the call has been made can be restricted so that it must have happened
            // exactly the number of times specified.
            A.CallTo(() => factory.Create()).MustHaveHappenedOnceExactly();
            A.CallTo(() => factory.Create()).MustHaveHappenedTwiceExactly();
            A.CallTo(() => factory.Create()).MustHaveHappened(4, Times.Exactly);

            // The number of times the call has been made can also be subjected to more complex requirements.
            A.CallTo(() => factory.Create()).MustHaveHappenedANumberOfTimesMatching(n => n == 1 || n == 17);
            A.CallTo(() => factory.Create()).MustHaveHappenedANumberOfTimesMatching(n => IsOdd(n));
        }

        private static bool IsOdd(int i)
        {
            return i % 2 == 1;
        }
    }
}
