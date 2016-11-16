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
            int Bar(CancellationToken cancellationToken);

            int Bar(object obj);

            Task<int> BarAsync(CancellationToken cancellationToken);

            Task BazAsync(CancellationToken cancellationToken);
        }

        [Scenario]
        public void NonCancelledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            int result)
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
        public void NonCancelledTokenWithConfiguredCall(
            IFoo fake,
            CancellationToken cancellationToken,
            int result)
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
        public void CancelledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Exception exception)
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
        public void CancelledTokenPassedAsObject(
            IFoo fake,
            CancellationToken cancellationToken,
            int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When a method is called with this cancellation token passed as Object"
                .x(() => result = fake.Bar((object)cancellationToken));

            "Then it doesn't throw and returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public void CancelledTokenWithConfiguredCallForAnyToken(
            IFoo fake,
            CancellationToken cancellationToken,
            int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.Bar(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured method is called with this cancellation token"
                .x(() => result = fake.Bar(cancellationToken));

            "Then it doesn't throw and returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public void CancelledTokenWithConfiguredCallForNonCancelledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake for a non-cancelled token"
                .x(() => A.CallTo(() => fake.Bar(A<CancellationToken>.That.IsNotCancelled())).Returns(42));

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured method is called with this cancellation token"
                .x(() => exception = Record.Exception(() => fake.Bar(cancellationToken)));

            "Then it throws an OperationCancelledException"
                .x(() => exception.Should().BeAnExceptionAssignableTo<OperationCanceledException>());
        }

        [Scenario]
        public void AsyncWithResultNonCancelledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is not cancelled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When an async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));

            "And the task's result should be the default value"
                .x(() => task.Result.Should().Be(0));
        }

        [Scenario]
        public void AsyncWithResultNonCancelledTokenWithConfiguredCall(
            IFoo fake,
            CancellationToken cancellationToken,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.BarAsync(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is not cancelled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));

            "And the task's result is the configured value"
                .x(() => task.Result.Should().Be(42));
        }

        [Scenario]
        public void AsyncWithResultCancelledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task<int> task)
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
        public void AsyncWithResultCancelledTokenWithConfiguredCallForAnyToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.BarAsync(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));

            "And the task's result is the configured value"
                .x(() => task.Result.Should().Be(42));
        }

        [Scenario]
        public void AsyncWithResultCancelledTokenWithConfiguredCallForNonCancelledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake for a non-cancelled token"
                .x(() => A.CallTo(() => fake.BarAsync(A<CancellationToken>.That.IsNotCancelled())).Returns(42));

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a cancelled task"
                .x(() => task.Status.Should().Be(TaskStatus.Canceled));
        }

        [Scenario]
        public void AsyncWithoutResultNonCancelledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is not cancelled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When an async method is called with this cancellation token"
                .x(() => { task = fake.BazAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));
        }

        [Scenario]
        public void AsyncWithoutResultNonCancelledTokenWithConfiguredCall(
            IFoo fake,
            CancellationToken cancellationToken,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.BazAsync(A<CancellationToken>._)).Returns(Task.FromResult(0)));

            "And a cancellation token that is not cancelled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BazAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));
        }

        [Scenario]
        public void AsyncWithoutResultCancelledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When an async method is called with this cancellation token"
                .x(() => { task = fake.BazAsync(cancellationToken); });

            "Then it returns a cancelled task"
                .x(() => task.Status.Should().Be(TaskStatus.Canceled));
        }

        [Scenario]
        public void AsyncWithoutResultCancelledTokenWithConfiguredCallForAnyToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.BazAsync(A<CancellationToken>._)).Returns(Task.FromResult(0)));

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BazAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));
        }

        [Scenario]
        public void AsyncWithoutResultCancelledTokenWithConfiguredCallForNonCancelledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake for a non-cancelled token"
                .x(() => A.CallTo(() => fake.BazAsync(A<CancellationToken>.That.IsNotCancelled())).Returns(Task.FromResult(0)));

            "And a cancellation token that is cancelled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BazAsync(cancellationToken); });

            "Then it returns a cancelled task"
                .x(() => task.Status.Should().Be(TaskStatus.Canceled));
        }
    }
}
