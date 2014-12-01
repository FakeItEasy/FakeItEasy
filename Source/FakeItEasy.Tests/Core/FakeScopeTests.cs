namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections;
    using System.Linq;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class FakeScopeTests
    {
        [Test]
        public void Dispose_sets_outside_scope_as_current_scope()
        {
            var scope = FakeScope.Current;

            using (Fake.CreateScope())
            {
                Assert.That(FakeScope.Current, Is.Not.SameAs(scope));
            }

            Assert.That(FakeScope.Current, Is.SameAs(scope));
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
        public void Call_intercepted_in_child_scope_should_be_visible_in_parent_scope()
        {
            var fake = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                using (Fake.CreateScope())
                {
                    fake.Bar();
                }

                A.CallTo(() => fake.Bar()).MustHaveHappened();
            }

            A.CallTo(() => fake.Bar()).MustHaveHappened();
        }

        [Test]
        public void Call_configured_in_child_scope_should_not_affect_parent_scope()
        {
            var fake = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                using (Fake.CreateScope())
                {
                    A.CallTo(fake).Throws(new InvalidOperationException());
                }

                fake.Bar();
            }
        }

        [Test]
        public void GetEnumerator_on_root_scope_should_not_be_supported()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<NotSupportedException>(() =>
                FakeScope.Current.GetEnumerator());
        }

        [Test]
        public void Enumerating_should_enumerate_all_calls_that_were_made_in_the_scope()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var otherFake = A.Fake<IFoo>();

            // Act
            using (var scope = Fake.CreateScope())
            {
                fake.Bar();
                otherFake.Baz();

                // Assert
                Assert.That(scope.ToArray().Select(x => x.Method.Name), Is.EquivalentTo(new[] { "Bar", "Baz" }));
            }
        }

        [Test]
        public void Enumerating_through_non_generic_interface_should_enumerate_all_calls_that_were_made_in_the_scope()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var otherFake = A.Fake<IFoo>();

            // Act
            using (var scope = Fake.CreateScope())
            {
                fake.Bar();
                otherFake.Baz();

                // Assert
                Assert.That(((IEnumerable)scope).Cast<ICompletedFakeObjectCall>().ToArray().Select(x => x.Method.Name), Is.EquivalentTo(new[] { "Bar", "Baz" }));
            }
        }
    }
}
