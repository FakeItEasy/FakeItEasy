namespace FakeItEasy.Tests.Core
{
    using FakeItEasy.Core;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class ConcurrentListTests
    {
        [Test]
        public void Should_allow_changes_during_enumeration()
        {
            var list = new ConcurrentList<string>() { "a", "b", "c" };
            Assert.DoesNotThrow(() =>
            {
                foreach (var element in list)
                {
                    list.Add(element);
                }
            });
        }

        [Test]
        public void Should_be_serializable()
        {
            // Arrange
            var initial = new ConcurrentList<string> { "a", "b", "c" };

            // Act
            var deserialized = BinarySerializationHelper.SerializeAndDeserialize(initial);

            // Assert
            Assert.AreEqual(3, deserialized.Count);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(initial[i], deserialized[i]);
            }
        }
    }
}
