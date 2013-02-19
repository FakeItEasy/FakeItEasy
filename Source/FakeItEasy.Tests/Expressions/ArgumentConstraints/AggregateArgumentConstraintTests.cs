namespace FakeItEasy.Tests.Expressions.ArgumentConstraints
{
    using System.Collections.Generic;
    using System.Text;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions.ArgumentConstraints;
    using NUnit.Framework;

    [TestFixture]
    internal class AggregateArgumentConstraintTests
        : ArgumentConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.constraintField = new AggregateArgumentConstraint(new[] { new EqualityArgumentConstraint("foo"), new EqualityArgumentConstraint("bar") });
        }

        public interface ITypeWithPaldkf
        {
            void Method(string firstArgument, params object[] args);
        }

        protected override IEnumerable<object> InvalidValues
        {
            get
            {
                return new[]
                    {
                        new object(),
                        null,
                        new[] {"one", "two"},
                        new[] {"foo", "bar", "biz"}
                    };
            }
        }

        protected override IEnumerable<object> ValidValues
        {
            get
            {
                return new object[]
                    {
                        new[] {"foo", "bar"},
                        new List<string>(new[] {"foo", "bar"})
                    };
            }
        }

        protected override string ExpectedDescription
        {
            get { return "[\"foo\", \"bar\"]"; }
        }

        [Test]
        public override void Constraint_should_provide_correct_description()
        {
            var output = new StringBuilder();

            this.constraintField.WriteDescription(new StringBuilderOutputWriter(output));

            Assert.That(output.ToString(), Is.EqualTo(this.ExpectedDescription));
        }
    }
}