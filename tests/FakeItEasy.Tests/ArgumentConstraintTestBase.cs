namespace FakeItEasy.Tests
{
    using System.Collections.Generic;
    using FakeItEasy.Core;

    public abstract class ArgumentConstraintTestBase
    {
        public abstract string ExpectedDescription { get; }

        internal IArgumentConstraint ConstraintField { get; set; }

        public abstract IEnumerable<object[]> ValidValues();

        public abstract IEnumerable<object[]> InvalidValues();
    }
}
