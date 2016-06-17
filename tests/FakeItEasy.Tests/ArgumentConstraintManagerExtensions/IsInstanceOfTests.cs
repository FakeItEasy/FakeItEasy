namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System;
    using System.Collections.Generic;

    public class IsInstanceOfTests
        : ArgumentConstraintTestBase<object>
    {
        protected override string ExpectedDescription => "Instance of System.DateTime";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                new object(),
                1,
                "foo",
                null);
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                new DateTime(2000, 1, 1));
        }

        protected override void CreateConstraint(IArgumentConstraintManager<object> scope)
        {
            scope.IsInstanceOf(typeof(DateTime));
        }
    }
}
