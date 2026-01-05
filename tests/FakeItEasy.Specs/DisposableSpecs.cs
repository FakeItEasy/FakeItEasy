namespace FakeItEasy.Specs;

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using LambdaTale;

public static class DisposableSpecs
{
    private static Exception? exception;

    [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Required for testing.")]
    [Scenario]
    public static void FakingDisposable(IDisposable? fake)
    {
        "Given a fake of a disposable class"
            .x(() =>
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

                fake = A.Fake<SomeDisposable>();
            })
            .Teardown(() => AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionHandler);

        "When the fake is finalized"
            .x(() =>
            {
                fake = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            });

        "Then no exception is thrown"
            .x(() => exception.Should().BeNull());
    }

    private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        exception = (Exception)e.ExceptionObject;
    }

    public abstract class SomeDisposable : IDisposable
    {
        ~SomeDisposable()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool shouldCleanupManaged);
    }
}
