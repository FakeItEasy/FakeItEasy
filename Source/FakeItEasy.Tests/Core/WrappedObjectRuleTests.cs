namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class WrappedObjectRuleTests
    {
        public interface ITypeWithOutputAndRefArguments
        {
            [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Required for testing.")]
            void MethodWithReferenceArgument(ref int argument);

            [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Required for testing.")]
            void MethodWithOutputArgument(out int argument);
        }

        [Test]
        public void IsApplicableTo_should_return_true()
        {
            var rule = this.CreateRule();

            Assert.That(rule.IsApplicableTo(A.Fake<IFakeObjectCall>()));
        }

        [Test]
        public void NumberOfTimesToCall_should_be_null()
        {
            var rule = this.CreateRule();

            Assert.That(rule.NumberOfTimesToCall, Is.Null);
        }

        [Test]
        public void Apply_should_set_return_value_from_wrapped_object([Values(0, 1, 2, 3, 5, 8)] int returnValue)
        {
            var wrapped = A.Fake<IFoo>();
            A.CallTo(() => wrapped.Baz()).Returns(returnValue);

            var call = FakeCall.Create<IFoo>("Baz", new Type[] { }, new object[] { });

            var rule = this.CreateRule(wrapped);
            rule.Apply(call);

            Assert.That(call.ReturnValue, Is.EqualTo(returnValue));
        }

        [Test]
        public void Apply_should_use_arguments_from_call_when_calling_wrapped_object()
        {
            var wrapped = A.Fake<IFoo>();

            var call = FakeCall.Create<IFoo>("Bar", new Type[] { typeof(object), typeof(object) }, new object[] { "foo", "bar" });

            var rule = this.CreateRule(wrapped);
            rule.Apply(call);

            A.CallTo(() => wrapped.Bar("foo", "bar")).MustHaveHappened();
        }

        [Test]
        public void Apply_should_assign_reference_arguments()
        {
            // Arrange
            var wrapped = new TypeThatImplementsInterfaceWithOutputAndRefArguments()
            {
                ReferenceArgumentThatWillBeApplied = 10
            };

            var call = FakeCall.Create<ITypeWithOutputAndRefArguments>("MethodWithReferenceArgument", new[] { Type.GetType("System.Int32&") }, new object[] { 0 });
            var rule = this.CreateRule(wrapped);

            // Act
            rule.Apply(call);

            // Assert
            Assert.That(call.Arguments[0], Is.EqualTo(10));
        }

        [Test]
        public void Apply_should_assign_out_arguments()
        {
            // Arrange
            var wrapped = new TypeThatImplementsInterfaceWithOutputAndRefArguments()
            {
                OutArgumentThatWillBeApplied = 10
            };

            var call = FakeCall.Create<ITypeWithOutputAndRefArguments>("MethodWithOutputArgument", new[] { Type.GetType("System.Int32&") }, new object[] { 0 });
            var rule = this.CreateRule(wrapped);

            // Act
            rule.Apply(call);

            // Assert
            Assert.That(call.Arguments[0], Is.EqualTo(10));
        }

        private WrappedObjectRule CreateRule()
        {
            return new WrappedObjectRule(new object());
        }

        private WrappedObjectRule CreateRule(object wrapped)
        {
            return new WrappedObjectRule(wrapped);
        }

        public class TypeThatImplementsInterfaceWithOutputAndRefArguments
            : ITypeWithOutputAndRefArguments
        {
            public int ReferenceArgumentThatWillBeApplied { get; set; }

            public int OutArgumentThatWillBeApplied { get; set; }

            public void MethodWithReferenceArgument(ref int argument)
            {
                argument = this.ReferenceArgumentThatWillBeApplied;
            }

            public void MethodWithOutputArgument(out int argument)
            {
                argument = this.OutArgumentThatWillBeApplied;
            }
        }
    }
}
