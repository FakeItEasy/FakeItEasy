namespace FakeItEasy.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    public abstract class ArgumentConstraintTestBase<T>
    {
        public abstract string ExpectedDescription { get; }

        internal IArgumentConstraint ConstraintField { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors",
            Justification = "CreateCosntraint has no unsafe side effects.")]
        protected ArgumentConstraintTestBase()
        {
            this.CreateConstraint(new DefaultArgumentConstraintManager<T>(x => this.ConstraintField = x));
        }

        protected abstract void CreateConstraint(IArgumentConstraintManager<T> scope);
        public abstract IEnumerable<object[]> ValidValues();
        public abstract IEnumerable<object[]> InvalidValues();
    }
}
