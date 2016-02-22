namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class FakeScopeTests
    {
        [Test]
        public void Dispose_sets_outside_scope_as_current_scope()
        {
            var scope = FakeScope.Current;

            FakeScope newCurrentScope;

            using (Fake.CreateScope())
            {
                newCurrentScope = FakeScope.Current;
            }

            newCurrentScope.Should().NotBeSameAs(scope, "new scopes should not be the original scope");
            FakeScope.Current.Should().BeSameAs(scope, "current scope should revert to original scope");
        }

        [Test]
        public void Disposing_original_scope_does_nothing()
        {
            FakeScope.Current.Dispose();
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
            var exception = Record.Exception(() => FakeScope.Current.GetEnumerator());

            // Assert
            exception.Should().BeAnExceptionOfType<NotSupportedException>();
        }

        [Test]
        public void Enumerating_should_enumerate_all_calls_that_were_made_in_the_scope()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var otherFake = A.Fake<IFoo>();

            IEnumerable<string> methodNames;

            // Act
            using (var scope = Fake.CreateScope())
            {
                fake.Bar();
                otherFake.Baz();

                methodNames = scope.Select(x => x.Method.Name);
            }

            // Assert
            methodNames.Should().BeEquivalentTo("Bar", "Baz");
        }

        [Test]
        public void Enumerating_through_non_generic_interface_should_enumerate_all_calls_that_were_made_in_the_scope()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var otherFake = A.Fake<IFoo>();
            IEnumerable<string> methodNames;

            // Act
            using (var scope = Fake.CreateScope())
            {
                fake.Bar();
                otherFake.Baz();

                methodNames = scope.ToArray().Select(x => x.Method.Name);
            }

            // Assert
            methodNames.Should().BeEquivalentTo("Bar", "Baz");
        }
    }
}
