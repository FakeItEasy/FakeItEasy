namespace FakeItEasy.Tests
{
    using FluentAssertions;
    using NUnit.Framework;

    public static class Tests
    {
        [Test]
        public static void CanUseFakeItEasy()
        {
            var fake = A.Fake<IDummyFactory>();
            A.CallTo(() => fake.Create(typeof(object))).Returns(new object());
            fake.Create(typeof(object)).Should().NotBeNull();
        }
    }
}
