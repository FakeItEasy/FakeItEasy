namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class RaiseTests
    {
        private readonly IFoo foo;
        private object sender;
        private EventArgs eventArguments;

        public RaiseTests()
        {
            this.foo = A.Fake<IFoo>();
            this.foo.SomethingHappened += this.Foo_SomethingHappened;
            this.sender = null;
            this.eventArguments = null;
        }

        [Fact]
        public void Raising_with_sender_and_arguments_should_raise_event_with_specified_sender()
        {
            // Arrange
            var senderToUse = new object();

            // Act
            this.foo.SomethingHappened += Raise.With(senderToUse, EventArgs.Empty);

            // Assert
            this.sender.Should().Be(senderToUse);
        }

        [Fact]
        public void Raising_with_sender_and_arguments_should_raise_event_with_specified_arguments()
        {
            // Arrange
            var arguments = new EventArgs();

            // Act
            this.foo.SomethingHappened += Raise.With(this.foo, arguments);

            // Assert
            this.eventArguments.Should().BeSameAs(arguments);
        }

        [Fact]
        public void Raising_with_arguments_only_should_raise_event_with_fake_as_sender()
        {
            // Arrange

            // Act
            this.foo.SomethingHappened += Raise.With(EventArgs.Empty);

            // Assert
            this.sender.Should().BeSameAs(this.foo);
        }

        [Fact]
        public void Raising_with_arguments_only_should_raise_event_with_specified_arguments()
        {
            // Arrange
            var arguments = new EventArgs();

            // Act
            this.foo.SomethingHappened += Raise.With(arguments);

            // Assert
            this.eventArguments.Should().BeSameAs(arguments);
        }

        [Fact]
        public void WithEmpty_should_return_raise_object_with_event_args_empty_set()
        {
            // Arrange
            this.foo.SomethingHappened += this.Foo_SomethingHappened;

            // Act
            this.foo.SomethingHappened += Raise.WithEmpty();

            // Assert
            this.eventArguments.Should().Be(EventArgs.Empty);
        }

        [Fact]
        public void Should_not_fail_when_raising_event_that_has_no_registered_listeners()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => { this.foo.SomethingHappened += Raise.WithEmpty(); });

            // Assert
            exception.Should().BeNull();
        }

        [Fact]
        public void Should_propagate_exception_thrown_by_event_handler()
        {
            // Arrange
            this.foo.SomethingHappened += this.Foo_SomethingHappenedThrows;

            // Act
            Action action = () => this.foo.SomethingHappened += Raise.WithEmpty();
            var exception = Record.Exception(action);

            // Assert
            exception.Should().BeAnExceptionOfType<NotImplementedException>()
                .And.StackTrace.Should().Contain("FakeItEasy.Tests.RaiseTests.Foo_SomethingHappenedThrows");
        }

        [Fact]
        public void Should_not_leak_handlers_when_raising()
        {
            // Arrange
            var eventHandlerArgumentProvider = ServiceLocator.Current.Resolve<EventHandlerArgumentProviderMap>();

            this.foo.SomethingHappened += this.Foo_SomethingHappened;

            EventHandler raisingHandler = Raise.WithEmpty(); // EventHandler to force the implicit conversion

            // Act
            this.foo.SomethingHappened += raisingHandler;

            // Assert
            IEventRaiserArgumentProvider ignoredArgumentProvider;
            eventHandlerArgumentProvider.TryTakeArgumentProviderFor(raisingHandler, out ignoredArgumentProvider)
                .Should().BeFalse();
        }

        private void Foo_SomethingHappened(object newSender, EventArgs e)
        {
            this.sender = newSender;
            this.eventArguments = e;
        }

        private void Foo_SomethingHappenedThrows(object newSender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
