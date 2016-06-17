namespace FakeItEasy.Tests.SelfInitializedFakes
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.SelfInitializedFakes;
    using FluentAssertions;
    using Xunit;

    public class CallDataTests
    {
        private MethodInfo DummyMethodInfo => typeof(IFoo).GetMethod("Bar", new Type[0]);

        [Fact]
        public void CallData_should_be_serializable()
        {
            // Arrange
            var data = new CallData(this.DummyMethodInfo, Enumerable.Empty<object>(), new object());

            // Act

            // Assert
            data.Should().BeBinarySerializable();
        }

        [Fact]
        public void CallData_should_be_serializable_when_output_arguments_is_provided_by_linq_query()
        {
            // Arrange
            var outputArguments =
                (from number in Enumerable.Range(1, 10)
                 select number).Cast<object>();

            var data = new CallData(this.DummyMethodInfo, outputArguments, null);

            // Act

            // Assert
            data.Should().BeBinarySerializable();
        }
    }
}
