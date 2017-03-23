namespace FakeItEasy.Specs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class CancellationSpecs
    {
        public interface IFoo
        {
            int Bar(CancellationToken cancellationToken);

            int Bar(object obj);

            Task<int> BarAsync(CancellationToken cancellationToken);

            Task BazAsync(CancellationToken cancellationToken);
        }

        [Scenario]
        public static void NonCanceledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is not canceled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When a method is called with this cancellation token"
                .x(() => result = fake.Bar(cancellationToken));

            "Then it doesn't throw and returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public static void NonCanceledTokenWithConfiguredCall(
            IFoo fake,
            CancellationToken cancellationToken,
            int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.Bar(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is not canceled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When the configured method is called with this cancellation token"
                .x(() => result = fake.Bar(cancellationToken));

            "Then it doesn't throw and returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void CanceledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is canceled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When a method is called with this cancellation token"
                .x(() => exception = Record.Exception(() => fake.Bar(cancellationToken)));

            "Then it throws an OperationCanceledException"
                .x(() => exception.Should().BeAnExceptionAssignableTo<OperationCanceledException>());
        }

        [Scenario]
        public static void CanceledTokenPassedAsObject(
            IFoo fake,
            CancellationToken cancellationToken,
            int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is canceled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When a method is called with this cancellation token passed as Object"
                .x(() => result = fake.Bar((object)cancellationToken));

            "Then it doesn't throw and returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public static void CanceledTokenWithConfiguredCallForAnyToken(
            IFoo fake,
            CancellationToken cancellationToken,
            int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.Bar(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is canceled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured method is called with this cancellation token"
                .x(() => result = fake.Bar(cancellationToken));

            "Then it doesn't throw and returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void CanceledTokenWithConfiguredCallForNonCanceledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake for a non-canceled token"
                .x(() => A.CallTo(() => fake.Bar(A<CancellationToken>.That.IsNotCanceled())).Returns(42));

            "And a cancellation token that is canceled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured method is called with this cancellation token"
                .x(() => exception = Record.Exception(() => fake.Bar(cancellationToken)));

            "Then it throws an OperationCanceledException"
                .x(() => exception.Should().BeAnExceptionAssignableTo<OperationCanceledException>());
        }

        [Scenario]
        public static void AsyncWithResultNonCanceledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is not canceled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When an async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));

            "And the task's result should be the default value"
                .x(() => task.Result.Should().Be(0));
        }

        [Scenario]
        public static void AsyncWithResultNonCanceledTokenWithConfiguredCall(
            IFoo fake,
            CancellationToken cancellationToken,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.BarAsync(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is not canceled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));

            "And the task's result is the configured value"
                .x(() => task.Result.Should().Be(42));
        }

        [Scenario]
        public static void AsyncWithResultCanceledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is canceled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When an async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a canceled task"
                .x(() => task.Status.Should().Be(TaskStatus.Canceled));
        }

        [Scenario]
        public static void AsyncWithResultCanceledTokenWithConfiguredCallForAnyToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.BarAsync(A<CancellationToken>._)).Returns(42));

            "And a cancellation token that is canceled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));

            "And the task's result is the configured value"
                .x(() => task.Result.Should().Be(42));
        }

        [Scenario]
        public static void AsyncWithResultCanceledTokenWithConfiguredCallForNonCanceledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake for a non-canceled token"
                .x(() => A.CallTo(() => fake.BarAsync(A<CancellationToken>.That.IsNotCanceled())).Returns(42));

            "And a cancellation token that is canceled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BarAsync(cancellationToken); });

            "Then it returns a canceled task"
                .x(() => task.Status.Should().Be(TaskStatus.Canceled));
        }

        [Scenario]
        public static void AsyncWithoutResultNonCanceledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is not canceled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When an async method is called with this cancellation token"
                .x(() => { task = fake.BazAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));
        }

        [Scenario]
        public static void AsyncWithoutResultNonCanceledTokenWithConfiguredCall(
            IFoo fake,
            CancellationToken cancellationToken,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.BazAsync(A<CancellationToken>._)).Returns(Task.FromResult(0)));

            "And a cancellation token that is not canceled"
                .x(() => cancellationToken = new CancellationToken(false));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BazAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));
        }

        [Scenario]
        public static void AsyncWithoutResultCanceledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a cancellation token that is canceled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When an async method is called with this cancellation token"
                .x(() => { task = fake.BazAsync(cancellationToken); });

            "Then it returns a canceled task"
                .x(() => task.Status.Should().Be(TaskStatus.Canceled));
        }

        [Scenario]
        public static void AsyncWithoutResultCanceledTokenWithConfiguredCallForAnyToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake"
                .x(() => A.CallTo(() => fake.BazAsync(A<CancellationToken>._)).Returns(Task.FromResult(0)));

            "And a cancellation token that is canceled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BazAsync(cancellationToken); });

            "Then it returns a successfully completed task"
                .x(() => task.Status.Should().Be(TaskStatus.RanToCompletion));
        }

        [Scenario]
        public static void AsyncWithoutResultCanceledTokenWithConfiguredCallForNonCanceledToken(
            IFoo fake,
            CancellationToken cancellationToken,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call configured on that fake for a non-canceled token"
                .x(() => A.CallTo(() => fake.BazAsync(A<CancellationToken>.That.IsNotCanceled())).Returns(Task.FromResult(0)));

            "And a cancellation token that is canceled"
                .x(() => cancellationToken = new CancellationToken(true));

            "When the configured async method is called with this cancellation token"
                .x(() => { task = fake.BazAsync(cancellationToken); });

            "Then it returns a canceled task"
                .x(() => task.Status.Should().Be(TaskStatus.Canceled));
        }
    }
}
