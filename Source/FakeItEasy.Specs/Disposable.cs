namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class Disposable
    {
        private static Exception exception;

        [Scenario]
        public void when_faking_a_disposable_class(
            IDisposable fake)
        {
            "establish"._(() =>
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

                fake = A.Fake<SomeDisposable>();
            })
            .Teardown(() => AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionHandler);

            "when faking a disposable class"._(() =>
            {
                fake = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
        });

            "it should not throw when finalized"._(() =>
            {
                exception.Should().BeNull();
            });
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
}