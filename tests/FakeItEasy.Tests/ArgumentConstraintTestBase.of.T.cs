namespace FakeItEasy.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    public abstract class ArgumentConstraintTestBase<T>
        : ArgumentConstraintTestBase
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "CreateCosntraint has no unsafe side effects.")]
        protected ArgumentConstraintTestBase()
        {
            this.CreateConstraint(new DefaultArgumentConstraintManager<T>(x => this.ConstraintField = x));
        }

        protected abstract void CreateConstraint(INegatableArgumentConstraintManager<T> scope);
    }
}
