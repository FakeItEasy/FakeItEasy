namespace FakeItEasy.Tests
{
    using FakeItEasy.Core;
    using NUnit.Framework;

    internal abstract class ArgumentConstraintTestBase<T>
        : ArgumentConstraintTestBase
    {
        [SetUp]
        public void Setup()
        {
            this.CreateConstraint(new DefaultArgumentConstraintManager<T>(x => this.ConstraintField = x));
            this.OnSetup();
        }

        protected virtual void OnSetup()
        {
        }

        protected abstract void CreateConstraint(IArgumentConstraintManager<T> scope);
    }
}
