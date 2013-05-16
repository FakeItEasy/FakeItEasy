namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    internal class SameAsConstraintTests
        : ArgumentConstraintTestBase<object>
    {
        protected override IEnumerable<object> InvalidValues
        {
            get 
            {
                yield return new Foo { Bar = "1" };
                yield return new Foo { Bar = "2" };
            }
        }
        
        class Foo
        {
            public string Bar { get; set; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { yield return new Foo { Bar = "1" } };
        }

        protected override string ExpectedDescription
        {
            get { return "same as Foo with Bar of 1"; }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<object> scope)
        {
            // huh?
            scope.IsEqualTo(10);
        }
    }
}
