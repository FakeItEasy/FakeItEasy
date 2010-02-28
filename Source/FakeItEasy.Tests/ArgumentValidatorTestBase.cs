using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Api;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests
{
    public abstract class ArgumentConstraintTestBase
    {
        protected IArgumentConstraint Validator;
        protected abstract IEnumerable<object> InvalidValues { get; }
        protected abstract IEnumerable<object> ValidValues { get; }
        protected abstract string ExpectedDescription { get; }

        [Test]
        [TestCaseSource("InvalidValues")]
        public void IsValid_should_return_false_for_invalid_values(object invalidValue)
        {
            Assert.That(this.Validator.IsValid(invalidValue), Is.False);
        }

        [Test]
        [TestCaseSource("ValidValues")]
        public void IsValid_should_return_true_for_valid_values(object validValue)
        {
            Assert.That(this.Validator.IsValid(validValue), Is.True);
        }

        [Test]
        public virtual void Validator_should_provide_correct_description()
        {
            Assert.That(this.Validator.ToString(), Is.EqualTo("<" + this.ExpectedDescription + ">"));
        }
    }

    public abstract class ArgumentValidatorTestBase<T>
        : ArgumentConstraintTestBase
    {
        protected new ArgumentConstraint<T> Validator
        {
            get
            {
                return (ArgumentConstraint<T>)base.Validator;
            }
            set
            {
                base.Validator = value;
            }
        }

        [Test]
        public void FullDescription_should_provide_expected_description()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(this.Validator.FullDescription, Is.EqualTo(this.ExpectedDescription));
        }

    }

}
