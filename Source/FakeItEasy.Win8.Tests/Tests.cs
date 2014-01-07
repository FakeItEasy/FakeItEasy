namespace FakeItEasy.Tests
{
    using FluentAssertions;
    using NUnit.Framework;

    public static class Tests
    {
        [Test]
        public static void CanUseFakeItEasy()
        {
            var fake = A.Fake<IDummyDefinition>();
            A.CallTo(() => fake.CreateDummy()).Returns(new object());
            fake.CreateDummy().Should().NotBeNull();
        }
    }
}
