namespace FakeItEasy.Tests.Core
{
    using System;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class WrappedObjectRuleTests
    {
        public interface ITypeWithReferenceArguments
        {
            void MethodWithReferenceArgument(ref int argument);
        }

        public interface ITypeWithOutputArguments
        {
            void MethodWithOutputArgument(out int argument);
        }

        [Fact]
        public void IsApplicableTo_should_return_true()
        {
            var rule = this.CreateRule();

            rule.IsApplicableTo(A.Fake<IFakeObjectCall>()).Should().BeTrue();
        }

        [Fact]
        public void NumberOfTimesToCall_should_be_null()
        {
            var rule = this.CreateRule();

            rule.NumberOfTimesToCall.Should().Be(null);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(8)]
        public void Apply_should_set_return_value_from_wrapped_object(int returnValue)
        {
            var wrapped = A.Fake<IFoo>();
            A.CallTo(() => wrapped.Baz()).Returns(returnValue);

            var call = FakeCall.Create<IFoo>("Baz", new Type[] { }, new object[] { });

            var rule = this.CreateRule(wrapped);
            rule.Apply(call);

            call.ReturnValue.Should().Be(returnValue);
        }

        [Fact]
        public void Apply_should_use_arguments_from_call_when_calling_wrapped_object()
        {
            var wrapped = A.Fake<IFoo>();

            var call = FakeCall.Create<IFoo>("Bar", new[] { typeof(object), typeof(object) }, new object[] { "foo", "bar" });

            var rule = this.CreateRule(wrapped);
            rule.Apply(call);

            A.CallTo(() => wrapped.Bar("foo", "bar")).MustHaveHappened();
        }

        [Fact]
        public void Apply_should_assign_reference_arguments()
        {
            // Arrange
            var wrapped = new TypeWithReferenceArguments(10);
            var call = FakeCall.Create<ITypeWithReferenceArguments>("MethodWithReferenceArgument", new[] { Type.GetType("System.Int32&") }, new object[] { 0 });
            var rule = this.CreateRule(wrapped);

            // Act
            rule.Apply(call);

            // Assert
            call.Arguments[0].Should().Be(10);
        }

        [Fact]
        public void Apply_should_assign_out_arguments()
        {
            // Arrange
            var wrapped = new TypeWithOutputArguments(10);
            var call = FakeCall.Create<ITypeWithOutputArguments>("MethodWithOutputArgument", new[] { Type.GetType("System.Int32&") }, new object[] { 0 });
            var rule = this.CreateRule(wrapped);

            // Act
            rule.Apply(call);

            // Assert
            call.Arguments[0].Should().Be(10);
        }

        private WrappedObjectRule CreateRule()
        {
            return new WrappedObjectRule(new object());
        }

        private WrappedObjectRule CreateRule(object wrapped)
        {
            return new WrappedObjectRule(wrapped);
        }

        private class TypeWithReferenceArguments
            : ITypeWithReferenceArguments
        {
            private readonly int referenceArgumentThatWillBeApplied;

            public TypeWithReferenceArguments(int referenceArgumentThatWillBeApplied)
            {
                this.referenceArgumentThatWillBeApplied = referenceArgumentThatWillBeApplied;
            }

            public void MethodWithReferenceArgument(ref int argument)
            {
                argument = this.referenceArgumentThatWillBeApplied;
            }
        }

        private class TypeWithOutputArguments
            : ITypeWithOutputArguments
        {
            private readonly int outArgumentThatWillBeApplied;

            public TypeWithOutputArguments(int outArgumentThatWillBeApplied)
            {
                this.outArgumentThatWillBeApplied = outArgumentThatWillBeApplied;
            }

            public void MethodWithOutputArgument(out int argument)
            {
                argument = this.outArgumentThatWillBeApplied;
            }
        }
    }
}
