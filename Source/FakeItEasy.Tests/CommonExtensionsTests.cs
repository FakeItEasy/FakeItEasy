using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class CommonExtensionsTests
    {
        [Test]
        public void Zip_returns_an_enumeral_of_tuples_paired_in_order()
        {
            var strings = new List<string>() { "a", "b", "c" };
            var ints = Enumerable.Range(1, int.MaxValue);

            var result = CommonExtensions.Zip(strings, ints).Select(x => x.Item1 + x.Item2.ToString());

            Assert.That(result, Is.EquivalentTo(new[] { "a1", "b2", "c3" }));
        }
    }
}