namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Tests.TestHelpers;
    using Xunit;

    public abstract class HasSameElementsAsTestsBase
        : ArgumentConstraintTestBase<IEnumerable<string?>>
    {
        protected override string ExpectedDescription => "\"a\", \"b\", NULL, \"y\", \"z\" (in any order)";

        public static IEnumerable<object?[]> InvalidValues()
        {
            return TestCases.FromObject(
                new[] { "1", "2", "x", "y" },
                Array.Empty<string>(),
                (object?)null,
                new[] { "a", "b", null, "z", "x" },
                new[] { "a", "b" });
        }

        public static IEnumerable<object?[]> ValidValues()
        {
            return TestCases.FromObject(
                new[] { "a", "b", null, "y", "z" },
                new List<string?> { "a", "b", null, "y", "z" },
                new[] { "a", "z", null, "y", "b" },
                new[] { null, "a", "b", "y", "z" });
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

    public class HasSameElementsAsTestsWithEnumerable : HasSameElementsAsTestsBase
    {
        protected override void CreateConstraint(INegatableArgumentConstraintManager<IEnumerable<string?>> scope)
        {
            scope.HasSameElementsAs(new List<object?> { "a", "b", null, "y", "z" });
        }
    }

    public class HasSameElementsAsTestsWithParams : HasSameElementsAsTestsBase
    {
        protected override void CreateConstraint(INegatableArgumentConstraintManager<IEnumerable<string?>> scope)
        {
            scope.HasSameElementsAs("a", "b", null, "y", "z");
        }
    }
}
