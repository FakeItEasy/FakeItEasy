﻿namespace FakeItEasy.Tests.Creation
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class ProxyGeneratorResultTests
    {
        [Fact]
        public void Should_set_that_proxy_was_not_successfully_created_when_constructor_with_error_message_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(reasonForFailure: "reason");

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeFalse();
        }

        [Fact]
        public void Should_set_that_proxy_was_not_successfully_created_when_constructor_with_error_message_and_exception_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(
                reasonForFailure: "reason",
                exception: new InvalidOperationException("exception message"));

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeFalse();
        }

        [Fact]
        public void Should_set_that_proxy_was_successfully_created_when_constructor_with_proxy_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(new object());

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeTrue();
        }

        [Fact]
        public void Should_set_reason_for_failure_when_constructor_with_reason_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(reasonForFailure: "reason");

            // Assert
            result.ReasonForFailure.Should().Be("reason");
        }

        [Fact]
        public void Should_set_reason_for_failure_when_constructor_with_reason_and_exception_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(
                reasonForFailure: "reason",
                exception: new InvalidOperationException("exception message"));

            // Assert
            var expectedReason = new[]
            {
                "reason",
                "An exception of type System.InvalidOperationException was caught during this call. Its message was:",
                "exception message"
            }.AsTextBlock();

            result.ReasonForFailure.Should().StartWith(expectedReason);
        }

        [Fact]
        public void Should_set_reason_for_failure_from_inner_exception_when_constructor_with_reason_and_TargetInvocationException_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(
                reasonForFailure: "reason",
                exception: new TargetInvocationException(new InvalidOperationException("target invocation inner exception message")));

            // Assert
            var expectedReason = new[]
            {
                "reason",
                "An exception of type System.InvalidOperationException was caught during this call. Its message was:",
                "target invocation inner exception message"
            }.AsTextBlock();

            result.ReasonForFailure.Should().StartWith(expectedReason);
        }

        [Fact]
        public void Should_set_reason_for_failure_from_exception_when_constructor_with_reason_and_TargetInvocationException_that_has_no_inner_exception_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(
                reasonForFailure: "reason",
                exception: new TargetInvocationException("target invocation exception message", null));

            // Assert
            var expectedReason = new[]
            {
                "reason",
                "An exception of type System.Reflection.TargetInvocationException was caught during this call. Its message was:",
                "target invocation exception message"
            }.AsTextBlock();

            result.ReasonForFailure.Should().StartWith(expectedReason);
        }

        [Fact]
        public void Should_set_proxy_when_constructor_with_proxy_is_used()
        {
            // Arrange
            var proxy = new object();

            // Act
            var result = new ProxyGeneratorResult(proxy);

            // Assert
            result.GeneratedProxy.Should().Be(proxy);
        }

        [Fact]
        public void Constructor_with_error_message_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => new ProxyGeneratorResult("reason");
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Constructor_with_proxy_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => new ProxyGeneratorResult(new object());
            call.Should().BeNullGuarded();
        }
    }
}
