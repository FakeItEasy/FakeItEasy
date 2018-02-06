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

        // The following are examples using the old Repeated class, which is being phased out, and
        // will be deprecated in version 5.0.0 and removed in 6.0.0.
        // If it's possible at all, prefer the versions above.
        public void Asserting_on_calls_using_legacy_repeated_class()
        {
            var factory = A.Fake<IWidgetFactory>();

            // This would throw an exception since no call has been made.
            A.CallTo(() => factory.Create()).MustHaveHappened();
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.AtLeast.Twice);
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.AtLeast.Times(10));

            // This on the other hand would not throw.
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Never);

            // The number of times the call has been made can be restricted so that it must have happened
            // exactly the number of times specified
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.Exactly.Times(4));

            // Then number of times can be specified so that it must not have happened more
            // than the specified number of times.
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.NoMoreThan.Twice);
            A.CallTo(() => factory.Create()).MustHaveHappened(Repeated.NoMoreThan.Times(5));
        }

        private static bool IsOdd(int i)
        {
            return i % 2 == 1;
        }
    }
}
