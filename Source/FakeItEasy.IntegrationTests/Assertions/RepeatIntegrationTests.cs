using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;

namespace FakeItEasy.IntegrationTests.Assertions
{
    [TestFixture]
    public class RepeatIntegrationTests
    {
        private IFoo foo;

        [SetUp]
        public void SetUp()
        {
            this.foo = A.Fake<IFoo>();
        }

        [Test]
        public void Assert_happened_once_exactly_should_pass_when_call_has_been_made_once()
        {
            // Arrange
            this.foo.Bar();

            // Act

            // Assert
            A.CallTo(() => this.foo.Bar()).Assert(Happened.Once.Exactly);
        }

        [Test]
        public void Assert_happened_once_exactly_should_fail_when_call_never_happened()
        {
            // Arrange
            
            // Act

            // Assert
            Assert.Throws<ExpectationException>(() => 
                A.CallTo(() => this.foo.Bar()).Assert(Happened.Once.Exactly));
        }

        [Test]
        public void Assert_happened_once_exactly_should_fail_when_call_happened_more_than_once()
        {
            // Arrange
            this.foo.Bar();
            this.foo.Bar();

            // Act

            // Assert
            Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => this.foo.Bar()).Assert(Happened.Once.Exactly));
        }

        [Test]
        public void Assert_happened_once_should_fail_when_call_never_happened()
        {
            // Arrange

            // Act
            
            // Assert
            Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => this.foo.Bar()).Assert(Happened.Once));
        }

        [Test]
        public void Assert_happened_once_should_pass_when_call_happened_once()
        {
            // Arrange
            this.foo.Bar();

            // Act

            // Assert
            A.CallTo(() => this.foo.Bar()).Assert(Happened.Once);
        }

        [Test]
        public void Assert_happened_once_should_pass_when_call_happened_twice()
        {
            // Arrange
            this.foo.Bar();
            this.foo.Bar();

            // Act

            // Assert
            A.CallTo(() => this.foo.Bar()).Assert(Happened.Once);
        }

        [Test]
        public void Assert_happened_times_should_pass_when_call_has_happened_specified_number_of_times()
        {
            // Arrange
            this.foo.Bar();
            this.foo.Bar();
            this.foo.Bar();

            // Act
            
            // Assert
            A.CallTo(() => foo.Bar()).Assert(Happened.Times(3));
        }

        [Test]
        public void Assert_happened_never_should_pass_when_no_call_has_been_made()
        {
            // Arrange

            // Act
            
            // Assert
            A.CallTo(() => this.foo.Bar()).Assert(Happened.Never);
        }

        [Test]
        public void Assert_happened_never_should_fail_when_a_call_has_been_made()
        {
            // Arrange
            foo.Bar();
            // Act

            // Assert
            Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => this.foo.Bar()).Assert(Happened.Never));
        }
    }
}
