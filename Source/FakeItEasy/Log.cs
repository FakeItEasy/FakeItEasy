namespace FakeItEasy
{
    using System;

    internal static class Log
    {
        public static ILogger GetLogger<T>()
        {
#if DEBUG
            return new ConsoleLogger(typeof(T).ToString());
#else
            return NullLogger.Instance;
#endif
        }

        private class ConsoleLogger
            : ILogger
        {
            private string name;

            public ConsoleLogger(string name)
            {
                this.name = name;
            }

            public void Debug(Func<string> message)
            {
                Console.WriteLine(string.Format("Log: {0} - {1}", this.name, message.Invoke()));
            }
        }

        private class NullLogger
            : ILogger
        {
            public static readonly NullLogger Instance = new NullLogger();

            private NullLogger()
            {

            }
            public void Debug(Func<string> message)
            {
                // Do nothing
            }
        }
    }
}
