namespace FakeItEasy.Tests.Creation
{
    using FakeItEasy.Creation;
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
            Assert.That(result.ProxyWasSuccessfullyGenerated, Is.False);
        }

        [Test]
        public void Should_set_that_proxy_was_successfully_created_when_constructor_with_proxy_and_raiser_is_used()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(A.Fake<ITaggable>(), A.Fake<ICallInterceptedEventRaiser>());

            // Assert
            Assert.That(result.ProxyWasSuccessfullyGenerated, Is.True);
        }

        [Test]
        public void Should_set_reason_for_failure_when_using_constructor_with_reason()
        {
            // Arrange

            // Act
            var result = new ProxyGeneratorResult(reasonForFailure: "reason");

            // Assert
            Assert.That(result.ReasonForFailure, Is.EqualTo("reason"));
        }

        [Test]
        public void Should_set_proxy_when_constructor_with_proxy_is_used()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();

            // Act
            var result = new ProxyGeneratorResult(proxy, A.Dummy<ICallInterceptedEventRaiser>());

            // Assert
            Assert.That(result.GeneratedProxy, Is.EqualTo(proxy));
        }

        [Test]
        public void Should_set_event_raiser()
        {
            // Arrange
            var eventRaiser = A.Fake<ICallInterceptedEventRaiser>();

            // Act
            var result = new ProxyGeneratorResult(A.Dummy<ITaggable>(), eventRaiser);

            // Assert
            Assert.That(result.CallInterceptedEventRaiser, Is.EqualTo(eventRaiser));
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
                new ProxyGeneratorResult(A.Dummy<ITaggable>(), A.Dummy<ICallInterceptedEventRaiser>()));
        }
    }
}
