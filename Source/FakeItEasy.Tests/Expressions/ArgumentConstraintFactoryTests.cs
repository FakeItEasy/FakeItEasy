namespace FakeItEasy.Tests.Expressions
{
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentConstraintFactoryTests
    {
        private ArgumentConstraintFactory CreateFactory()
        {
            return new ArgumentConstraintFactory();
        }

        [Test]
        public void GetArgumentConstraint_should_get_constraint_produced_by_method()
        {
            // Arrange
            var constraint = A.Fake<ArgumentConstraint<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(GetConstraint(constraint)), 0);

            // Act
            var factory = this.CreateFactory();
            var producedConstraint = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedConstraint, Is.SameAs(constraint));
        }

        [Test]
        public void GetArgumentConstraint_should_get_constraint_passed_in_directly()
        {
            // Arrange
            var constraint = A.Fake<ArgumentConstraint<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(constraint), 0);
            
            // Act
            var factory = this.CreateFactory();
            var producedConstraint = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedConstraint, Is.SameAs(constraint));
        }

        [Test]
        public void GetArgumentConstraint_should_get_constraint_when_Argument_property_has_been_called_on_it()
        {
            // Arrange
            var constraint = A.Fake<ArgumentConstraint<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(constraint.Argument), 0);

            // Act
            var factory = this.CreateFactory();
            var producedConstraint = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedConstraint, Is.SameAs(constraint));
        }

        [Test]
        public void GetArgumentConstraint_should_get_constraint_when_parameter_is_of_type_object()
        {
            // Arrange
            var constraint = A.Fake<ArgumentConstraint<object>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Bar(GetConstraint(constraint)), 0);

            // Act
            var factory = this.CreateFactory();
            var producedConstraint = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedConstraint, Is.SameAs(constraint));
        }

        private static ArgumentConstraint<T> GetConstraint<T>(ArgumentConstraint<T> constraint)
        {
            return constraint;
        }

        public interface ITestInterface
        {
            void Foo(int number);
            void Bar(object value);
        }
    }
}
