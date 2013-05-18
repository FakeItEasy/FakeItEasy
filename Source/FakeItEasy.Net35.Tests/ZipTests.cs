namespace FakeItEasy
{
    using System;
    using System.Linq;
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
            var zipped = letters.Zip(numbers, (f, s) => new { Letter = f, Number = s }).ToList();

            // Assert
            Assert.That(zipped, Has.Count.EqualTo(2));
            Assert.That(zipped[0], Has.Property("Letter").EqualTo("a").And.Property("Number").EqualTo(1));
            Assert.That(zipped[1], Has.Property("Letter").EqualTo("b").And.Property("Number").EqualTo(2));
        }
    }
}