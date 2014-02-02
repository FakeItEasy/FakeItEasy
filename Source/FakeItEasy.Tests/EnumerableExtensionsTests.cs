namespace FakeItEasy.Tests
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void Zip_returns_an_enumerable_of_tuples_paired_in_order()
        {
            var strings = new List<string>() { "a", "b", "c" };
            var ints = Enumerable.Range(1, int.MaxValue);

            var result = strings.Zip(ints).Select(x => x.Item1 + x.Item2.ToString(CultureInfo.CurrentCulture));

            result.Should().BeEquivalentTo(new[] { "a1", "b2", "c3" });
        }
    }
}