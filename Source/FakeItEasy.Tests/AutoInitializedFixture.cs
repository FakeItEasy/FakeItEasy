namespace FakeItEasy.Tests
{
    using NUnit.Framework;

    internal class AutoInitializedFixture
    {
        [SetUp]
        public void InitializeFixture()
        {
            Fake.InitializeFixture(this);
        }
    }
}