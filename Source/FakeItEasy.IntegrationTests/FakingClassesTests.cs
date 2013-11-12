namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Core;
    using NUnit.Framework;

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

        [Test]
        public void Should_be_able_to_serialize_a_fake()
        {
            // arrange
            var person = A.Fake<Person>();

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                // act
                formatter.Serialize(stream, person);
            }
        }

        [Serializable]
        public class Person
        {
        }
    }
}