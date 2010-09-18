namespace FakeItEasy
{
    using System;

    internal static class Log
    {
#if DEBUG
        private const bool UseLogging = true;
#else
        private const bool UseLogging = false;
#endif
        

        public static Logger GetLogger<T>()
        {
            if (UseLogging)
            {
                return new ConsoleLogger(typeof(T).ToString());
            }
            else
            {
                return NullLogger.Instance;
            }
        }

        private class ConsoleLogger
            : Logger
        {
            private string name;

            public ConsoleLogger(string name)
            {
                this.name = name;
            }

            public override void Debug(Func<string> message)
            {
                Console.WriteLine(string.Format("Log: {0} - {1}", this.name, message.Invoke()));
            }
        }

        private class NullLogger
            : Logger
        {
            public static readonly NullLogger Instance = new NullLogger();

            private NullLogger()
            {

            }
            public override void Debug(Func<string> message)
            {
                // Do nothing
            }
        }
    }
}
