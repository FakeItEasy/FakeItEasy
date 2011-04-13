namespace FakeItEasy.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class OutputWriterTests
    {
        [Test]
        public void Should_append_line_break_when_calling_write_line()
        {
            // Arrange
            var writer = A.Fake<IOutputWriter>();
            
            // Act
            writer.WriteLine();

            // Assert
            FakeExtensions.MustHaveHappened(A.CallTo(() => writer.Write(Environment.NewLine)));
        }

        [Test]
        public void Should_return_same_instance_when_calling_write_line()
        {
            // Arrange
            var writer = A.Dummy<IOutputWriter>();

            // Act
            var result = writer.WriteLine();

            // Assert
            Assert.That(result, Is.SameAs(writer));
        }
    }
}