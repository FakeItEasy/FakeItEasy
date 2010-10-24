namespace FakeItEasy.IntegrationTests
{
    using NUnit.Framework;
    using System;
    using Core;

    [TestFixture]
    public class FakingClassesTests
    {
        [Test]
        public void Should_be_able_to_get_a_fake_value_of_uri_type()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                A.Fake<Uri>();
            }
        }
    }
}
