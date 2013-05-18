namespace FakeItEasy.Tests
{
    using FakeItEasy.Core;
    using NUnit.Framework;

    internal abstract class ArgumentConstraintTestBase<T>
        : ArgumentConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.CreateConstraint(new DefaultArgumentConstraintManager<T>(x => this.ConstraintField = x));
            this.OnSetUp();
        }

        protected virtual void OnSetUp()
        {
        }

        protected abstract void CreateConstraint(IArgumentConstraintManager<T> scope);
    }
}
