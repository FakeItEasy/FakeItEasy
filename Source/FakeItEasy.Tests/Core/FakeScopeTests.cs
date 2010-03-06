using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Core;

namespace FakeItEasy.Tests.Core
{
    [TestFixture]
    public class FakeScopeTests
    {
        [Test]
        public void Dispose_sets_outside_scope_as_current_scope()
        {
            var scope = FakeScope.Current;

            using (Fake.CreateScope())
            {
                Assert.That(FakeScope.Current, Is.Not.EqualTo(scope));
            }

            Assert.That(FakeScope.Current, Is.EqualTo(scope));
        }

        [Test]
        public void Disposing_original_scope_does_nothing()
        {
            FakeScope.Current.Dispose();
        }

        [Test]
        public void OriginalScope_has_non_null_container()
        {
            Assert.That(FakeScope.Current.FakeObjectContainer, Is.Not.Null);
        }

        [Test]
        public void CreatingNewScope_without_container_has_container_set_to_same_container_as_parent_scope()
        {
            var parentContainer = FakeScope.Current.FakeObjectContainer;

            using (Fake.CreateScope())
            {
                Assert.That(FakeScope.Current.FakeObjectContainer, Is.SameAs(parentContainer));
            }
        }

        [Test]
        public void CreatingNewScope_with_container_sets_that_container_to_scope()
        {
            var newContainer = A.Fake<IFakeObjectContainer>();

            using (FakeScope.Create(newContainer))
            {
                Assert.That(FakeScope.Current.FakeObjectContainer, Is.SameAs(newContainer));
            }
        }

        [Test]
        public void Call_instercepted_in_child_scope_should_be_visible_in_parent_scope()
        {
            var fake = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                using (Fake.CreateScope())
                {
                    fake.Bar();
                }

                Fake.Assert(fake).WasCalled(x => x.Bar());
            }

            Fake.Assert(fake).WasCalled(x => x.Bar());
        }

        [Test]
        public void Call_configured_in_child_scope_should_not_affect_parent_scope()
        {
            var fake = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                using (Fake.CreateScope())
                {
                    Configure.Fake(fake).AnyCall().Throws(new Exception());
                }

                fake.Bar();
            }
        }
    }
}
