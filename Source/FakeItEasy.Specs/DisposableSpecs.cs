namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_faking_a_disposable_class
        : EventRaisingSpecs
    {
        static IDisposable fake;
        private static Exception exception;

        Establish context = () =>
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            fake = A.Fake<SomeDisposable>();
        };

        Because of = () =>
        {
            fake = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        };

        It should_not_throw_when_finalized = () => exception.Should().BeNull();

        Cleanup after = () =>
        {
            AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionHandler;
        };

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