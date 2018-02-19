namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    public abstract class IsSameSequenceAsTestsBase
        : ArgumentConstraintTestBase<IEnumerable<string>>
    {
        protected override string ExpectedDescription => "\"a\", \"b\", NULL, \"y\", \"z\"";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                new[] { "1", "2", "x", "y" },
                Array.Empty<string>(),
                null,
                new[] { "a", "b", null, "z", "x" },
                new[] { "a", "b" });
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                new[] { "a", "b", null, "y", "z" },
                new List<string> { "a", "b", null, "y", "z" });
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public override void IsValid_should_return_false_for_invalid_values(object invalidValue)
        {
            base.IsValid_should_return_false_for_invalid_values(invalidValue);
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public override void IsValid_should_return_true_for_valid_values(object validValue)
        {
            base.IsValid_should_return_true_for_valid_values(validValue);
        }
    }

    public class IsSameSequenceAsTestsWithEnumerable : IsSameSequenceAsTestsBase
    {
        protected override void CreateConstraint(INegatableArgumentConstraintManager<IEnumerable<string>> scope)
        {
            scope.IsSameSequenceAs(new List<object> { "a", "b", null, "y", "z" });
        }
    }

    public class IsSameSequenceAsTestsWithParams : IsSameSequenceAsTestsBase
    {
        protected override void CreateConstraint(INegatableArgumentConstraintManager<IEnumerable<string>> scope)
        {
            scope.IsSameSequenceAs("a", "b", null, "y", "z");
        }
    }
}
