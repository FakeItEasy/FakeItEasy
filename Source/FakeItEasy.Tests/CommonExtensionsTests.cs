using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class CommonExtensionsTests
    {
        [Test]
        public void Pairwise_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                CommonExtensions.Zip(new List<string>(), new List<string>()));
        }

        [Test]
        public void Pairwise_returns_an_enumeral_of_tuples_paired_in_order()
        {
            var strings = new List<string>() { "a", "b", "c" };
            var ints = Enumerable.Range(1, int.MaxValue);

            var result = CommonExtensions.Zip(strings, ints).Select(x => x.First + x.Second.ToString());

            Assert.That(result, Is.EquivalentTo(new[] { "a1", "b2", "c3" }));
        }
    }
}