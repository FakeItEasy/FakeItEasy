namespace FakeItEasy.Tests
{
    public class AutoInitializedFixture
    {
        public AutoInitializedFixture()
        {
            Fake.InitializeFixture(this);
        }
    }
}
