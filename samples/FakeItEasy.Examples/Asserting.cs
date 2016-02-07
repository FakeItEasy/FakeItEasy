namespace FakeItEasy.Examples
{
    using FakeItEasy.Examples.ExampleObjects;

    public class Asserting
    {
        public void Asserting_on_calls()
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
    }
}
