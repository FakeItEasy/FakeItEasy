namespace FakeItEasy.Examples
{
    public class Asserting
    {
        public void Asserting_on_calls()
        {
            var factory = A.Fake<IWidgetFactory>();

            // This would throw an exception since no call has been made.
            A.CallTo(() => factory.Create()).Assert(Happened.Once);
            A.CallTo(() => factory.Create()).Assert(Happened.Twice);
            A.CallTo(() => factory.Create()).Assert(Happened.Times(10));

            // This on the other hand would not throw.
            A.CallTo(() => factory.Create()).Assert(Happened.Never);

            // The number of times the call has been made can be restricted so that it must have happened
            // exactly the number of times specified
            A.CallTo(() => factory.Create()).Assert(Happened.Once.Exactly);
            A.CallTo(() => factory.Create()).Assert(Happened.Times(4).Exactly);

            // Then number of times can be specified so that it must not have happened more
            // than the specified number of times.
            A.CallTo(() => factory.Create()).Assert(Happened.Twice.OrLess);
            A.CallTo(() => factory.Create()).Assert(Happened.Times(5).OrLess);
        }
    }
}