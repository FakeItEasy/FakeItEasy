namespace FakeItEasy.Specs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public class CancellationSpecs
    {
        public interface IFoo
        {
            int Bar();

            int Bar(CancellationToken cancellationToken);

            Task<int> BarAsync(CancellationToken cancellationToken);
        }

        [Scenario]
        public void NoCancellationToken(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "When a method which doesn't accept a cancellation token is called"
                .x(() => result = fake.Bar());

            "Then it doesn't throw and returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public void NonCancelledCancellationToken(IFoo fake, CancellationToken cancellationToken, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is not cancelled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When a method is called with this cancellation token"
                .x(() => result = fake.Bar(cancellationToken));

            "Then it doesn't throw and returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public void NonCancelledCancellationTokenWithConfiguredCall(IFoo fake, CancellationToken cancellationToken, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.Bar(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is not cancelled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When the configured method is called with this cancellation token"
                .x(() => result = fake.Bar(cancellationToken));

            "Then it doesn't throw and returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public void CancelledCancellationToken(IFoo fake, CancellationToken cancellationToken, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When a method is called with this cancellation token"
                .x(() => exception = Record.Exception(() => fake.Bar(cancellationToken)));

            "Then it throws an OperationCanceledException"
                .x(() => exception.Should().BeAnExceptionAssignableTo<OperationCanceledException>());
        }

        [Scenario]
        public void CancelledCancellationTokenWithConfiguredCall(IFoo fake, CancellationToken cancellationToken, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.Bar(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured method is called with this cancellation token"
                .x(() => exception = Record.Exception(() => fake.Bar(cancellationToken)));

            "Then it throws an OperationCanceledException, regardless of the configured call"
                .x(() => exception.Should().BeAnExceptionAssignableTo<OperationCanceledException>());
        }

        [Scenario]
        public void AsyncNonCancelledCancellationToken(IFoo fake, CancellationToken cancellationToken, Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is not cancelled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When an async method is called with this cancellation token"
                .x(() => task = fake.BarAsync(cancellationToken));

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));

            "And the task's result should be the default value"
                .x(() => task.Result.Should().Be(0));
        }

        [Scenario]
        public void AsyncNonCancelledCancellationTokenWithConfiguredCall(IFoo fake, CancellationToken cancellationToken, Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.BarAsync(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is not cancelled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When the configured async method is called with this cancellation token"
                .x(() => task = fake.BarAsync(cancellationToken));

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));

            "And the task's result is the configured value"
                .x(() => task.Result.Should().Be(42));
        }

        [Scenario]
        public void AsyncCancelledCancellationToken(IFoo fake, CancellationToken cancellationToken, Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When an async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a cancelled task"
                .x(() => task.Status.Should().Be(TaskStatus.Canceled));
        }

        [Scenario]
        public void AsyncCancelledCancellationTokenWithConfiguredCall(IFoo fake, CancellationToken cancellationToken, Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.BarAsync(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a cancelled task, regardless of the configured call"
                .x(() => task.Status.Should().Be(TaskStatus.Canceled));
        }
    }
}
