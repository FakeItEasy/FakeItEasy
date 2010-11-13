namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class FakeCallEqualityComparerTests
    {
        private static readonly MethodInfo ToStringMethod = typeof(object).GetMethod("ToString", new Type[] { });
        private static readonly MethodInfo EqualsMethod = typeof(object).GetMethod("Equals", new[] { typeof(object) });

        private FakeCallEqualityComparer comparer;
        private IFakeObjectCall firstCall;
        private IFakeObjectCall secondCall;
        
        [SetUp]
        public void SetUp()
        {
            this.firstCall = CreateFakedFakeObjectCall();
            this.secondCall = MakeEqualCopy(this.firstCall);

            this.comparer = new FakeCallEqualityComparer();
        }

        [Test]
        public void Should_return_true_for_call_where_method_arguments_and_fake_are_the_same()
        {
            // Arrange

            // Act
            
            // Assert
            Assert.That(this.comparer.Equals(this.firstCall, this.secondCall), Is.True);
        }

        [Test]
        public void Should_return_false_when_method_differs()
        {
            // Arrang
            A.CallTo(() => this.firstCall.Method).Returns(EqualsMethod);
            A.CallTo(() => this.secondCall.Method).Returns(ToStringMethod);

            // Act
            var result = this.comparer.Equals(this.firstCall, this.secondCall);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_return_false_when_argument_differs()
        {
            // Arrang
            A.CallTo(() => this.firstCall.Arguments).Returns(new ArgumentCollection(new object[] { new object() }, EqualsMethod));
            
            // Act
            var result = this.comparer.Equals(this.firstCall, this.secondCall);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_return_false_when_faked_object_differs()
        {
            // Arrang
            A.CallTo(() => this.firstCall.FakedObject).Returns(new object());

            // Act
            var result = this.comparer.Equals(this.firstCall, this.secondCall);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_return_same_hash_code_for_equal_calls()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(this.comparer.GetHashCode(this.firstCall), 
                Is.EqualTo(this.comparer.GetHashCode(this.secondCall)));
        }

        [Test]
        public void Should_not_fail_when_getting_hash_code_where_arguments_contains_null()
        {
            // Arrange
            A.CallTo(() => this.firstCall.Arguments).Returns(new ArgumentCollection(new object[] {null}, EqualsMethod));

            // Act

            // Assert
            Assert.DoesNotThrow(() => this.comparer.GetHashCode(this.firstCall));
        }

        private static IFakeObjectCall CreateFakedFakeObjectCall()
        {
            var call = A.Fake<IFakeObjectCall>();

            A.CallTo(() => call.Method).Returns(ToStringMethod);
            A.CallTo(() => call.Arguments).Returns(new ArgumentCollection(new object[] { }, new string[] { }));

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