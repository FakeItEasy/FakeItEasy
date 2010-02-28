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
        public void GetArgumentValidator_should_get_validator_produced_by_method()
        {
            // Arrange
            var validator = A.Fake<ArgumentConstraint<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(GetValidator(validator)), 0);

            // Act
            var factory = this.CreateFactory();
            var producedValidator = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedValidator, Is.SameAs(validator));
        }

        [Test]
        public void GetArgumentValidator_should_get_validator_passed_in_directly()
        {
            // Arrange
            var validator = A.Fake<ArgumentConstraint<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(validator), 0);
            
            // Act
            var factory = this.CreateFactory();
            var producedValidator = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedValidator, Is.SameAs(validator));
        }

        [Test]
        public void GetArgumentValidator_should_get_validator_when_Argument_property_has_been_called_on_it()
        {
            // Arrange
            var validator = A.Fake<ArgumentConstraint<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(validator.Argument), 0);

            // Act
            var factory = this.CreateFactory();
            var producedValidator = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedValidator, Is.SameAs(validator));
        }

        [Test]
        public void GetArgumentValidator_should_get_validator_when_parameter_is_of_type_object()
        {
            // Arrange
            var validator = A.Fake<ArgumentConstraint<object>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Bar(GetValidator(validator)), 0);

            // Act
            var factory = this.CreateFactory();
            var producedValidator = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedValidator, Is.SameAs(validator));
        }

        private static ArgumentConstraint<T> GetValidator<T>(ArgumentConstraint<T> validator)
        {
            return validator;
        }

        public interface ITestInterface
        {
            void Foo(int number);
            void Bar(object value);
        }
    }
}
