namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Reflection;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    using static FakeItEasy.Tests.TestHelpers.ExpressionHelper;

    public class FakeCallEqualityComparerTests
    {
        private static readonly MethodInfo ToStringMethod = GetMethodInfo<object>(x => x.ToString());
        private static readonly MethodInfo EqualsMethod = GetMethodInfo<object>(x => x.Equals(new object()));

        private readonly FakeCallEqualityComparer comparer;
        private readonly IFakeObjectCall firstCall;
        private readonly IFakeObjectCall secondCall;

        public FakeCallEqualityComparerTests()
        {
            this.firstCall = CreateFakedFakeObjectCall();
            this.secondCall = MakeEqualCopy(this.firstCall);

            this.comparer = new FakeCallEqualityComparer();
        }

        [Fact]
        public void Should_return_true_for_call_where_method_arguments_and_fake_are_the_same()
        {
            // Arrange

            // Act

            // Assert
            this.comparer.Equals(this.firstCall, this.secondCall).Should().BeTrue();
        }

        [Fact]
        public void Should_return_false_when_method_differs()
        {
            // Arrang
            A.CallTo(() => this.firstCall.Method).Returns(EqualsMethod);
            A.CallTo(() => this.secondCall.Method).Returns(ToStringMethod);

            // Act
            var result = this.comparer.Equals(this.firstCall, this.secondCall);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_return_false_when_argument_differs()
        {
            // Arrang
            A.CallTo(() => this.firstCall.Arguments).Returns(new ArgumentCollection(new[] { new object() }, EqualsMethod));

            // Act
            var result = this.comparer.Equals(this.firstCall, this.secondCall);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_return_false_when_faked_object_differs()
        {
            // Arrang
            A.CallTo(() => this.firstCall.FakedObject).Returns(new object());

            // Act
            var result = this.comparer.Equals(this.firstCall, this.secondCall);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_return_same_hash_code_for_equal_calls()
        {
            // Arrange

            // Act

            // Assert
            this.comparer.GetHashCode(this.firstCall).Should().Be(this.comparer.GetHashCode(this.secondCall));
        }

        [Fact]
        public void Should_not_fail_when_getting_hash_code_where_arguments_contains_null()
        {
            // Arrange
            A.CallTo(() => this.firstCall.Arguments).Returns(new ArgumentCollection(new object?[] { null }, EqualsMethod));

            // Act
            var exception = Record.Exception(() => this.comparer.GetHashCode(this.firstCall));

            // Assert
            exception.Should().BeNull();
        }

        [Fact]
        public void Should_not_fail_getting_hash_code_when_fake_is_strict()
        {
            // arrange
            var call = A.Fake<IFakeObjectCall>();
            A.CallTo(() => call.FakedObject).Returns(A.Fake<IFoo>(o => o.Strict()));
            var sut = new FakeCallEqualityComparer();

            // act
            var exception = Record.Exception(() => sut.GetHashCode(call));

            // assert
            exception.Should().BeNull();
        }

        private static IFakeObjectCall CreateFakedFakeObjectCall()
        {
            var call = A.Fake<IFakeObjectCall>();

            A.CallTo(() => call.Method).Returns(ToStringMethod);
            A.CallTo(() => call.Arguments).Returns(new ArgumentCollection(Array.Empty<object>(), A.Fake<MethodInfo>()));

            return call;
        }

        private static IFakeObjectCall MakeEqualCopy(IFakeObjectCall call)
        {
            var copy = A.Fake<IFakeObjectCall>();

            A.CallTo(() => copy.Method).Returns(call.Method);
            A.CallTo(() => copy.Arguments).Returns(call.Arguments);
            A.CallTo(() => copy.FakedObject).Returns(call.FakedObject);

            return copy;
        }
    }
}
