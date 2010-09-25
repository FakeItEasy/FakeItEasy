using System.Linq;
using FakeItEasy.Core;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class FakeTests
    {
        [Test]
        public void GetCalls_gets_all_calls_to_fake_object()
        {
            var foo = A.Fake<IFoo>();

            foo.Bar();
            foo.Baz();
            foo.Biz();

            var calls = Fake.GetCalls(foo);
            var namesOfCalledMethods = calls.Select(x => x.Method.Name).ToArray();

            Assert.That(namesOfCalledMethods, Is.EquivalentTo(new[] { "Bar", "Baz", "Biz" }));            
        }

        [Test]
        public void GetCalls_is_properly_guarded()
        {
            NullGuardedConstraint.Assert(() => Fake.GetCalls(A.Fake<IFoo>()));
        }

        [Test]
        public void Static_equals_delegates_to_static_method_on_object()
        {
            Assert.That(Fake.Equals("foo", "foo"), Is.True);
        }

        [Test]
        public void Static_ReferenceEquals_delegates_to_static_method_on_object()
        {
            var s = "";

            Assert.That(Fake.ReferenceEquals(s, s), Is.True);
        }

        [Test]
        public void CreateScope_should_create_a_new_scope()
        {
            var originalScope = FakeScope.Current;
            using (Fake.CreateScope(A.Fake<IFakeObjectContainer>()))
            {
                Assert.That(FakeScope.Current, Is.Not.SameAs(originalScope));
            }
        }
    }
}
