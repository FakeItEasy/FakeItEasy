namespace FakeItEasy.Tests
{
    using System.Collections.Generic;
    using System.Text;
    using FakeItEasy.Core;
    using NUnit.Framework;

    internal abstract class ArgumentConstraintTestBase
    {
        protected internal IArgumentConstraint ConstraintField { get; set; }

        protected abstract IEnumerable<object> InvalidValues { get; }

        protected abstract IEnumerable<object> ValidValues { get; }

        protected abstract string ExpectedDescription { get; }

        private IArgumentConstraint Constraint
        {
            get
            {
                return (IArgumentConstraint)this.ConstraintField;
            }
        }

        [Test]
        [TestCaseSource("InvalidValues")]
        public void IsValid_should_return_false_for_invalid_values(object invalidValue)
        {
            Assert.That(this.Constraint.IsValid(invalidValue), Is.False);
        }

        [Test]
        [TestCaseSource("ValidValues")]
        public void IsValid_should_return_true_for_valid_values(object validValue)
        {
            var result = this.Constraint.IsValid(validValue);

            Assert.That(result, Is.True);
        }

        [Test]
        public virtual void Constraint_should_provide_correct_description()
        {
            var output = new StringBuilder();

            this.Constraint.WriteDescription(new StringBuilderOutputWriter(output));

            Assert.That(output.ToString(), Is.EqualTo("<" + this.ExpectedDescription + ">"));
        }
    }
}