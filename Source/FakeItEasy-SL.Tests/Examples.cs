namespace FakeItEasy_SL.Tests
{
    using FakeItEasy;

    public class Examples
    {
        public void Test_that_nothing_blows_up_thats_all()
        {
            var foo = A.Fake<System.IDisposable>();

            foo.Dispose();

            A.CallTo(() => foo.Dispose()).MustHaveHappened();
        }
    }
}
