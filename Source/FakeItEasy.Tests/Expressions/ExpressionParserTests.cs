namespace FakeItEasy.Tests.Expressions
{
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;
    using System;
    using System.Linq.Expressions;

    [TestFixture]
    public class ExpressionParserTests
    {
        private ExpressionParser CreateManager()
        {
            return new ExpressionParser();
        }

        [Test]
        public void GetFakeObjectCallIsMadeOn_should_be_null_guarded()
        {
            // Arrange
            var manager = this.CreateManager();

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                manager.GetFakeObjectCallIsMadeOn(this.CreateDummyExpression()));
        }

        [Test]
        public void GetFakeObjectCallIsMadeOn_should_return_fake_object()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var fake = Fake.GetFakeManager(foo);

            Expression<Action> callSpecification = () => foo.Bar();

            var manager = this.CreateManager();

            // Act
            var retrievedFake = manager.GetFakeObjectCallIsMadeOn(callSpecification);

            // Assert
            Assert.That(retrievedFake, Is.SameAs(fake));
        }

        [Test]
        public void GetFakeObjectCallIsMadeOn_should_return_fake_object_when_member_accessed_is_property()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var fake = Fake.GetFakeManager(foo);

            Expression<Func<int>> callSpecification = () => foo.SomeProperty;

            var manager = this.CreateManager();

            // Act
            var retrievedFake = manager.GetFakeObjectCallIsMadeOn(callSpecification);

            // Assert
            Assert.That(retrievedFake, Is.SameAs(fake));
        }

        [Test]
        public void GetFakeObjectCallIsMadeOn_should_thrown_when_object_call_is_made_on_is_not_faked()
        {
             //Arrange
            var notFaked = new TypeWithPublicField();

            Expression<Func<int>> callSpecification = () => notFaked.Foo;

            var manager = this.CreateManager();

            // Act

            // Assert
            Assert.Throws<ArgumentException>(() =>
                manager.GetFakeObjectCallIsMadeOn(callSpecification));
        }

        [Test]
        public void GetFakeObjectCallIsMadeOn_should_fail_when_call_is_not_on_an_instance()
        {
            // Arrange
            Expression<Func<DateTime>> callSpecification = () => DateTime.Now;

            var manager = this.CreateManager();

            // Act

            // Assert
            var thrown = Assert.Throws<ArgumentException>(() =>
                manager.GetFakeObjectCallIsMadeOn(callSpecification));

            Assert.That(thrown.Message, Is.EqualTo("The specified call is not made on a fake object."));
        }


        public class TypeWithPublicField
        {
            public int Foo = 1;
        }

        private System.Linq.Expressions.LambdaExpression CreateDummyExpression()
        {
            return ExpressionHelper.CreateExpression<IFoo>(x => x.Bar());
        }

    }
}
