namespace FakeItEasy.Tests.Expressions
{
    using NUnit.Framework;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.Expressions.ArgumentConstraints;
    using System;
    using FakeItEasy.Expressions.ArgumentConstraints;

    [TestFixture]
    public class ArgumentConstraintTests
        : ArgumentConstraintTestBase<int>
    {
        private ArgumentConstraintScope<int> scope;

        protected override void OnSetUp()
        {
            base.OnSetUp();

            this.scope = A.Fake<ArgumentConstraintScope<int>>();
            A.CallTo(() => this.scope.IsValid(A<int>.Ignored)).Returns(true);
            A.CallTo(() => this.scope.ResultOfChildConstraintIsValid(true)).Returns(true);
            A.CallTo(() => this.scope.ResultOfChildConstraintIsValid(false)).Returns(false);
        }

        private TestableConstraint CreateValidator()
        {
            return new TestableConstraint(this.scope);
        }

        [Test]
        public void Create_should_return_validator_that_calls_predicate_when_IsValid_is_called()
        {
            // Arrange
            string argumentForPredicate = null;

            Func<string, bool> predicate = x =>
                {
                    argumentForPredicate = x;
                    return true;
                };

            var validations = A.Fake<ArgumentConstraintScope<string>>();
            A.CallTo(() => validations.IsValid(A<string>.Ignored)).Returns(true);
            
            // Act
            var validator = ArgumentConstraint.Create(validations, predicate, "foo");
            validator.IsValid("argument");

            // Assert
            Assert.That(argumentForPredicate, Is.EqualTo("argument"));
        }

        [Test]
        public void Create_should_return_validator_that_delegates_response_from_predicate(
            [Values(true, false)] bool predicateResponse)
        {
            // Arrange
            Func<int, bool> predicate = x => predicateResponse;

            // Act
            var validator = ArgumentConstraint.Create(this.scope, predicate, "foo");
            var isValid = validator.IsValid(1);

            // Assert
            Assert.That(isValid, Is.EqualTo(predicateResponse));
        }

        [Test]
        public void Create_should_return_validator_that_prints_description_from_ToString()
        {
            // Arrange
            var validator = ArgumentConstraint.Create(this.scope, x => true, "Any Int32");

            // Act
            var description = validator.ToString();

            // Assert
            Assert.That(description, Is.EqualTo("<Any Int32>"));
        }

        [Test]
        public void Create_should_return_validator_with_the_passed_in_validations_object()
        {
            // Arrange
            var validations = A.Fake<ArgumentConstraintScope<int>>();
            
            // Act
            var validator = ArgumentConstraint.Create(validations, x => true, "foo");

            // Assert
            Assert.That(validator.Scope, Is.SameAs(validations));
        }

        [Test]
        public void Create_should_be_null_guarded()
        {
            // Assert
            NullGuardedConstraint.Assert(() =>
                ArgumentConstraint.Create(this.scope, x => true, "foo"));
        }

        [Test]
        public void Create_should_throw_when_description_is_an_empty_string()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() =>
                ArgumentConstraint.Create(this.scope, x => true, string.Empty));
        }

        [Test]
        public void ToString_should_return_formatted_description()
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.DescriptionToUse = "Some Description";

            // Act
            var description = validator.ToString();

            // Assert
            Assert.That(validator.ToString(), Is.EqualTo("<Some Description>"));
        }

        [Test]
        public void IsValid_should_return_false_when_the_validator_is_valid_in_itself_but_the_parent_validations_is_not_valid()
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.EvaluateReturnValue = true;

            A.CallTo(() => this.scope.IsValid(A<int>.Ignored)).Returns(false);
            
            // Act
            var result = validator.IsValid(10);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsValid_should_return_false_when_validator_is_not_valid_and_parent_validations_is_not_valid()
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.EvaluateReturnValue = false;

            A.CallTo(() => this.scope.IsValid(A<int>.Ignored)).Returns(false);

            // Act
            var result = validator.IsValid(10);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsValid_should_return_true_when_validations_is_valid_and_validator_is_validated_by_validations()
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.EvaluateReturnValue = true;

            A.CallTo(() => this.scope.ResultOfChildConstraintIsValid(true)).Returns(true);

            // Act
            var result = validator.IsValid(10);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValid_should_return_false_when_validations_is_valid_but_validator_is_not_validated_by_validations()
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.EvaluateReturnValue = true;

            A.CallTo(() => this.scope.ResultOfChildConstraintIsValid(true)).Returns(false);

            // Act
            var result = validator.IsValid(10);

            // Assert
            Assert.That(result, Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void And_should_return_validations_that_delegates_IsValid_to_IsValid_of_validator(bool validatorIsValid)
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.EvaluateReturnValue = validatorIsValid;

            A.CallTo(() => this.scope.IsValid(10)).Returns(true);

            // Act
            var result = validator.And;

            // Assert
            Assert.That(result.IsValid(10), Is.EqualTo(validatorIsValid));
        }

        [Test]
        public void And_should_return_validations_that_concatenates_the_full_description_of_the_parent_validator_with_AND()
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.DescriptionToUse = "foo";

            // Act
            var description = validator.And.ToString();

            // Assert
            Assert.That(description, Is.EqualTo("foo and"));
        }

        [TestCase(true, Result = true)]
        [TestCase(false, Result = false)]
        public bool And_should_return_validations_where_ResultOfChildValidatorIsValid_is_positive(bool validatorIsValid)
        {
            // Arrange
            var validator = this.CreateValidator();
            var and = validator.And;

            // Act

            // Assert
            return and.ResultOfChildConstraintIsValid(validatorIsValid);
        }

        [TestCase("foo and", "bar", Result = "foo and bar")]
        [TestCase("", "something", Result = "something")]
        [TestCase(null, "foo", Result = "foo")]
        public string FullDescription_should_return_description_of_validations_concatenated_with_own_description(string validationsDescription, string validatorDescription)
        {
            // Arrange
            A.CallTo(() => this.scope.ToString()).Returns(validationsDescription);

            var validator = this.CreateValidator();
            validator.DescriptionToUse = validatorDescription;

            // Act
            var description = validator.FullDescription;

            // Assert
            return description;
        }

        [TestCase(true, true, Result = true)]
        [TestCase(true, false, Result = true)]
        [TestCase(false, true, Result = true)]
        [TestCase(false, false, Result = false)]
        public bool Or_should_return_validator_that_is_valid_if_any_of_the_validators_are_valid(bool firstValidatorResult, bool secondValidatorResult)
        {
            // Arrange
            A.CallTo(() => this.scope.IsValid(3)).Returns(true);

            var firstValidator = this.CreateValidator();
            firstValidator.EvaluateReturnValue = firstValidatorResult;

            var secondValidator = this.CreateValidator();
            secondValidator.EvaluateReturnValue = secondValidatorResult;

            // Act
            var orValidator = firstValidator.Or(secondValidator);

            // Assert
            return orValidator.IsValid(3);
        }

        [Test]
        public void Or_should_return_validator_that_combines_the_descriptions_of_the_two_validators()
        {
            // Arrange
            var firstValidator = this.CreateValidator();
            firstValidator.DescriptionToUse = "first";

            var secondValidator = this.CreateValidator();
            secondValidator.DescriptionToUse = "second";

            var orValidator = firstValidator.Or(secondValidator);

            // Act

            // Assert
            Assert.That(orValidator.FullDescription, Is.EqualTo("first or (second)"));
        }

        [Test]
        public void Argument_should_return_default_value()
        {
            // Arrange
            var validator = this.CreateValidator();

            // Act
            var result = validator.Argument;

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Constraint_should_convert_to_argument_type_implicitly()
        {
            // Arrange
            var validator = this.CreateValidator();

            // Act
            int result = validator;

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Argument_should_convert_to_equality_constraint_implicitly()
        {
            // Arrange

            // Act
            ArgumentConstraint<string> constraint = "test";

            // Assert
            Assert.That(constraint, Is.InstanceOf<EqualityArgumentConstraint<string>>());
            Assert.That(constraint.IsValid("test"), Is.True);
            Assert.That(constraint.IsValid("something else"), Is.False);
        }

        [Test]
        public void Or_should_pass_root_scope_to_delegate()
        {
            // Arrange
            ArgumentConstraintScope<string> scopePassedToDelegate = null;

            // Act
            A<string>.That.IsNull().Or(x =>
            {
                scopePassedToDelegate = x;
                return scopePassedToDelegate.Contains("foo");
            });

            // Assert
            Assert.That(scopePassedToDelegate, Is.InstanceOf<RootArgumentConstraintScope<string>>());
        }

        [Test]
        public void Or_should_return_or_constraint()
        {
            // Arrange

            // Act
            var returned = A<string>.That.IsNull().Or(x => x.Contains("foo"));

            // Assert
            Assert.That(returned, Is.Not.Null);
        }

        [Test]
        public void Or_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A<string>.That.StartsWith("foo").Or(x => x.Contains("bar")));
        }

        [Test]
        public void Constructor_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                new TestableConstraint(new RootArgumentConstraintScope<int>()));
        }

        private class TestableConstraint
            : ArgumentConstraint<int>
        {
            public bool EvaluateReturnValue = true;

            public TestableConstraint(ArgumentConstraintScope<int> scope) : base(scope) { }

            public string DescriptionToUse = "";

            protected override string Description
            {
                get { return this.DescriptionToUse; }
            }

            protected override bool Evaluate(int value)
            {
                return this.EvaluateReturnValue;
            }
        }

        protected override System.Collections.Generic.IEnumerable<object> InvalidValues
        {
            get { return new object[] { }; }
        }

        protected override System.Collections.Generic.IEnumerable<object> ValidValues
        {
            get { return new object[] { 1, 2, 3 }; }
        }

        protected override string ExpectedDescription
        {
            get { return ""; }
        }

        protected override ArgumentConstraint<int> CreateConstraint(ArgumentConstraintScope<int> scope)
        {
            return new TestableConstraint(scope);
        }
    }
}
