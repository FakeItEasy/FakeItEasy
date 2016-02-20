namespace FakeItEasy.IntegrationTests
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class FakingClassesTests
    {
        [Test]
        public void Should_be_able_to_get_a_fake_value_of_uri_type()
        {
            // Arrange

            // Act
            var fake = A.Fake<Uri>();

            // Assert
            Fake.GetFakeManager(fake).Should().NotBeNull("because we should be able to create a fake Uri");
        }

        [Test]
        public void Should_be_able_to_use_a_fake_after_binary_deserializing_it()
        {
            // Arrange
            var person = A.Fake<Person>();

            // Act
            var deserializedPerson = BinarySerializationHelper.SerializeAndDeserialize(person);

            // Assert
            deserializedPerson.Name.Should().Be(string.Empty, "because the default behavior should work");
        }

        [Test]
        public void Should_be_able_to_change_the_configuration_of_a_fake_after_binary_deserializing_it()
        {
            // Arrange
            var person = A.Fake<Person>();

            // Act
            var deserializedPerson = BinarySerializationHelper.SerializeAndDeserialize(person);

            // Assert
            A.CallTo(() => deserializedPerson.Name).Returns("Eric Cartman");
            deserializedPerson.Name.Should().Be("Eric Cartman");
        }

        [Serializable]
        public class Person
        {
            public virtual string Name { get; set; }
        }
    }
}
