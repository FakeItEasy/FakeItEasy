using NUnit.Framework;
using FakeItEasy.Tests;
using System;
using FakeItEasy.Core;
using System.Collections.Generic;
using System.Linq;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class AssertionsTests
    {
        [Test]
        public void Method_that_is_configured_to_throw_should_still_be_recorded()
        {
            var fake = A.Fake<IFoo>();
            
            A.CallTo(() => fake.Bar()).Throws(new Exception()).Once();

            try
            {
                fake.Bar();
            }
            catch { }

            A.CallTo(() => fake.Bar()).MustHaveHappened();
        }

        [Test]
        public void Should_be_able_to_assert_ordered_on_collections_of_calls()
        {
            using (var scope = Fake.CreateScope())
            {
                // Arrange
                var foo = A.Fake<IFoo>();
               
                // Act
                foo.Bar();
                foo.Baz();

                // Assert            
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => foo.Bar()).MustHaveHappened();
                    A.CallTo(() => foo.Baz()).MustHaveHappened();
                }
            }

            
        }

        [Test]
        public void Should_fail_when_calls_did_not_happen_in_specified_order()
        {
            using (var scope = Fake.CreateScope())
            {
                // Arrange
                var foo = A.Fake<IFoo>();
                
                // Act
                foo.Baz();
                foo.Bar();
                
                // Assert       
                Assert.That(() =>
                {
                    using (scope.OrderedAssertions())
                    {
                        A.CallTo(() => foo.Bar()).MustHaveHappened();
                        A.CallTo(() => foo.Baz()).MustHaveHappened();
                    }
                },
                Throws.Exception.InstanceOf<ExpectationException>());
            }
        }

        [Test]
        public void Should_be_able_to_use_ordered_asserts_on_single_fake_without_scope()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            foo.Bar();
            foo.Baz();

            // Assert
            Assert.That(() =>
                {
                    using (Fake.OrderedAssertions(foo))
                    { 
                        
                    }
                },
            Throws.Exception.InstanceOf<ExpectationException>());
        }
    }
}
