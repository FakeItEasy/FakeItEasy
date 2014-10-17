namespace FakeItEasy.Tests.Creation
{
    using System;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class ProxyGeneratorResultTests
    {
        [Test]
        public void Should_set_that_proxy_was_not_successfully_created_when_constructor_with_error_message_is_used()
        {
            // Arrange
            
            // Act
            var result = new ProxyGeneratorResult(reasonForFailure: "reason");

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeFalse();
        }

        [Test]
        public void Should_set_that_proxy_was_not_successfully_created_when_constructor_with_error_message_and_exception_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(reasonForFailure: "reason", exception: new InvalidOperationException("exception message"));

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeFalse();
        }

        [Test]
        public void Should_set_that_proxy_was_successfully_created_when_constructor_with_proxy_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(A.Fake<ITaggable>());

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeTrue();
        }

        [Test]
        public void Should_set_reason_for_failure_when_constructor_with_reason_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(reasonForFailure: "reason");

            // Assert
            result.ReasonForFailure.Should().Be("reason");
        }

        [Test]
        public void Should_set_reason_for_failure_when_constructor_with_reason_and_exception_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(reasonForFailure: "reason", exception: new InvalidOperationException("exception message"));

            // Assert
            result.ReasonForFailure.Should().Be("reason\r\nAn exception was caught during this call. Its message was:\r\nexception message");
        }

        [Test]
        public void Should_set_proxy_when_constructor_with_proxy_is_used()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();

            // Act
            var result = new ProxyGeneratorResult(proxy);

            // Assert
            result.GeneratedProxy.Should().Be(proxy);
        }

        [Test]
        public void Constructor_with_error_message_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                new ProxyGeneratorResult("reason"));
        }

        [Test]
        public void Constructor_with_proxy_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                new ProxyGeneratorResult(A.Dummy<ITaggable>()));
        }
    }
}
