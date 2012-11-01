using System;
using System.Collections;
using System.Linq;
using FakeItEasy.Core;
using NUnit.Framework;

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
        public void Call_instercepted_in_child_scope_should_be_visible_in_parent_scope()
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
                    Any.CallTo(fake).Throws(new Exception());
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

        [Test]
        public void Adding_first_rule_to_scope_affects_all_calles_in_scope()
        {
            // Arrange
            var fake1 = A.Fake<IFoo>();
            var fake2 = A.Fake<IFoo>();

            int applyCalledNumber = 0;
            var ruleFirst = new FakeCallRule() { IsApplicableTo = x => true, Apply = x => applyCalledNumber++ };
            var ruleLast = new FakeCallRule() { IsApplicableTo = x => true };


            // Act
            using (var scope = Fake.CreateScope())
            {
                scope.AddScopeRuleFirst(ruleFirst);
                scope.AddScopeRuleLast(ruleLast);
                fake1.Bar();
                fake2.Bar();
            }

            // Assert
            Assert.That(ruleFirst.ApplyWasCalled, Is.True);
            Assert.That(ruleLast.ApplyWasCalled, Is.False);
            Assert.That(applyCalledNumber, Is.EqualTo(2));
        }

        [Test]
        public void Adding_last_rule_to_scope_affects_all_calles_in_scope()
        {
            // Arrange
            var fake1 = A.Fake<IFoo>();
            var fake2 = A.Fake<IFoo>();

            var ruleLast = new FakeCallRule() { IsApplicableTo = x => true };


            // Act
            using (var scope = Fake.CreateScope())
            {
                scope.AddScopeRuleLast(ruleLast);
                fake1.Bar();
            }

            // Assert
            Assert.That(ruleLast.ApplyWasCalled, Is.True);
        }

        [Test]
        public void Adding_rule_to_scope_does_not_affect_calles_out_of_scope()
        {
            // Arrange
            var fake1 = A.Fake<IFoo>();

            var rule = new FakeCallRule() { IsApplicableTo = x => true };

            // Act
            using (var scope = Fake.CreateScope())
            {
                scope.AddScopeRuleFirst(rule);
            }

            fake1.Bar();

            // Assert
            Assert.That(rule.ApplyWasCalled, Is.False);
        }

        [Test]
        public void Adding_rule_to_scope_affects_calles_in_nested_scope()
        {
            // Arrange
            var fake1 = A.Fake<IFoo>();

            var rule = new FakeCallRule() { IsApplicableTo = x => true };

            // Act
            using (var scope = Fake.CreateScope())
            {
                scope.AddScopeRuleFirst(rule);

                using (Fake.CreateScope())
                {
                    fake1.Bar();
                }
            }

            // Assert
            Assert.That(rule.ApplyWasCalled, Is.True);
        }
    }
}
