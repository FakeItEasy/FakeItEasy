namespace FakeItEasy.Recipes.CSharp;

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

public class CapturingArguments
{
    //// --8<-- [start:IListLogger]
    public interface IListLogger
    {
        void Log(string message, IEnumerable<int> list);
    }
    //// --8<-- [end:IListLogger]

    [Fact]
    public void OperationsShouldLog()
    {
        //// --8<-- [start:SimpleCapture]
        // Arrange
        var capturedMessage = A.Captured<string>();

        var logger = A.Fake<IListLogger>();
        A.CallTo(() => logger.Log(
                capturedMessage._,
                An<IEnumerable<int>>._))
            .DoesNothing();

        var calculator = new Calculator(logger);

        // Act
        calculator.Add([1, 2, 3, 4]);
        calculator.Square(7);

        // Assert
        capturedMessage.Values.Should().Equal(
            "about to add",
            "about to square");

        // Or if you only care about the last value:
        capturedMessage.GetLastValue().Should().Be(
            "about to square");
        //// --8<-- [end:SimpleCapture]
    }

    [Fact]
    public void CaptureInputThatContainsThree()
    {
        //// --8<-- [start:ConstrainedCapture]
        // Arrange
        var capturedMessage = A.Captured<string>();

        var logger = A.Fake<IListLogger>();
        A.CallTo(() => logger.Log(
                capturedMessage.That.Contains("add"),
                An<IEnumerable<int>>._))
            .DoesNothing();

        var calculator = new Calculator(logger);

        // Act
        calculator.Add([1, 2, 3, 4]);
        calculator.Square(7);
        calculator.Add([8, 9]);

        // Assert
        capturedMessage.Values.Should().Equal(
            "about to add",
            "about to add");
        //// --8<-- [end:ConstrainedCapture]
    }

    [Fact]
    public void NaivelyCaptureMutatedList()
    {
        var test = () =>
        {
            //// --8<-- [start:NaivelyCaptureMutatedList]
            // Arrange
            var capturedOperands = A.Captured<IEnumerable<int>>();

            var logger = A.Fake<IListLogger>();
            A.CallTo(() => logger.Log(
                    "about to add",
                    capturedOperands._))
                .DoesNothing();

            var calculator = new Calculator(logger);

            // Act
            List<int> operands = [1, 2, 3, 4];
            calculator.Add(operands); // capturedOperands captures operands
            operands.RemoveAt(0);
            calculator.Add(operands); // captures operands again - same instance

            // Assert
            // passes:
            capturedOperands.Values[1].Should().Equal(2, 3, 4);

            // fails - operands contains only 2, 3, and 4:
            capturedOperands.Values[0].Should().Equal(1, 2, 3, 4);
            //// --8<-- [end:NaivelyCaptureMutatedList]
        };

        test.Should().ThrowExactly<AssertionFailedException>()
            .WithMessage("Expected * to be equal to {1, 2, 3, 4}, but {2, 3, 4} contains 1 item(s) less.");
    }

    [Fact]
    public void CaptureCopiedMutatedList()
    {
        //// --8<-- [start:CaptureCopiedMutatedList]
        // Arrange
        var capturedOperands =
            A.Captured<IEnumerable<int>>().FrozenBy(l => l.ToList());

        var logger = A.Fake<IListLogger>();
        A.CallTo(() => logger.Log(
                "about to add",
                capturedOperands._))
            .DoesNothing();

        var calculator = new Calculator(logger);

        // Act
        List<int> operands = [1, 2, 3, 4];
        calculator.Add(operands); // capturedOperands captures copy of operands
        operands.RemoveAt(0);
        calculator.Add(operands); // capturedOperands captures copy of operands

        // Assert
        // both pass:
        capturedOperands.Values[1].Should().Equal(2, 3, 4);
        capturedOperands.Values[0].Should().Equal(1, 2, 3, 4);
        //// --8<-- [end:CaptureCopiedMutatedList]
    }

    [Fact]
    public void CaptureCopiedMutatedListToNewType()
    {
        //// --8<-- [start:CaptureCopiedMutatedListToNewType]
        // Arrange
        var capturedOperands =
            A.Captured<IEnumerable<int>>().FrozenBy(l => string.Join(" + ", l));

        var logger = A.Fake<IListLogger>();
        A.CallTo(() => logger.Log(
                "about to add",
                capturedOperands._))
            .DoesNothing();

        var calculator = new Calculator(logger);

        // Act
        List<int> operands = [1, 2, 3, 4];
        calculator.Add(operands); // capturedOperands captures transformed operands
        operands.RemoveAt(0);
        calculator.Add(operands); // capturedOperands captures transformed operands

        // Assert
        capturedOperands.Values[1].Should().Be("2 + 3 + 4");
        capturedOperands.Values[0].Should().Be("1 + 2 + 3 + 4");
        //// --8<-- [end:CaptureCopiedMutatedListToNewType]
    }

    //// --8<-- [start:Calculator]
    internal sealed class Calculator(IListLogger logger)
    {
        public int Add(IList<int> operands)
        {
            logger.Log("about to add", operands);
            return operands.Sum();
        }

        public int Square(int input)
        {
            logger.Log("about to square", [input]);
            return input * input;
        }
    }
    //// --8<-- [end:Calculator]
}
