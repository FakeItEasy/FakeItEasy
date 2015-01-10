namespace FakeItEasy.Win8.Tests
{
    using FluentAssertions;
    using NUnit.Framework;

    public static class Tests
    {
        [Test]
        public static void CanUseFakeItEasy()
        {
            var fake = A.Fake<IDummyDefinition>();
            A.CallTo(() => fake.CreateDummyOfType(typeof(object))).Returns(new object());
            fake.CreateDummyOfType(typeof(object)).Should().NotBeNull();
        }
    }
}
