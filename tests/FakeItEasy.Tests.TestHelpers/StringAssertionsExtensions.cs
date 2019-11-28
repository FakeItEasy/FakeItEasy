namespace FakeItEasy.Tests.TestHelpers
{
    using FluentAssertions;
    using FluentAssertions.Primitives;

    public static class StringAssertionsExtensions
    {
        public static AndConstraint<StringAssertions> BeModuloLineEndings(
            this StringAssertions assertion,
            string expected,
            string? because = null,
            params object[] becauseArgs)
        {
            return assertion.Be(expected.NormalizeLineEndings(), because, becauseArgs);
        }

        public static AndConstraint<StringAssertions> ContainModuloLineEndings(
            this StringAssertions assertion,
            string expected,
            string? because = null,
            params object[] becauseArgs)
        {
            return assertion.Contain(expected.NormalizeLineEndings(), because, becauseArgs);
        }

        public static AndConstraint<StringAssertions> StartWithModuloLineEndings(
            this StringAssertions assertion,
            string expected,
            string? because = null,
            params object[] becauseArgs)
        {
            return assertion.StartWith(expected.NormalizeLineEndings(), because, becauseArgs);
        }

        public static AndConstraint<StringAssertions> EndWithModuloLineEndings(
            this StringAssertions assertion,
            string expected,
            string? because = null,
            params object[] becauseArgs)
        {
            return assertion.EndWith(expected.NormalizeLineEndings(), because, becauseArgs);
        }

        public static AndConstraint<StringAssertions> MatchModuloLineEndings(
            this StringAssertions assertion,
            string expected,
            string? because = null,
            params object[] becauseArgs)
        {
            return assertion.Match(expected.NormalizeLineEndings(), because, becauseArgs);
        }
    }
}
