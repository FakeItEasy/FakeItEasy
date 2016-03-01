namespace FakeItEasy
{
    extern alias FakeItEasy;

    using System;
    using System.Collections.Generic;
    using FakeItEasy::System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class ZipTests
    {
        [Test]
        public void Should_zip_correctly()
        {
            // Arrange
            var letters = new[] { "a", "b", "c" };
            var numbers = new[] { 1, 2 };

            // Act
            var zipped = new List<string>(letters.Zip(numbers, (l, n) => l + n));

            // Assert
            Assert.That(zipped, Is.EqualTo(new[] { "a1", "b2" }));
        }
    }
}
