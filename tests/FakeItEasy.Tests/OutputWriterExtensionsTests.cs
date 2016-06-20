namespace FakeItEasy.Tests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class OutputWriterExtensionsTests
    {
        [Fact]
        public void Should_append_line_break_when_calling_write_line()
        {
            // Arrange
            var writer = A.Fake<IOutputWriter>();

            // Act
            writer.WriteLine();

            // Assert
            A.CallTo(() => writer.Write(Environment.NewLine)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_same_instance_when_calling_write_line()
        {
            // Arrange
            var writer = A.Dummy<IOutputWriter>();

            // Act
            var result = writer.WriteLine();

            // Assert
            result.Should().BeSameAs(writer);
        }
    }
}
