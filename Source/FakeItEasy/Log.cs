namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal static class Log
    {
        static Log()
        {
            UseLogging = false;
        }

        private static bool UseLogging { get; set; }

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

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Can be instantiated at will.")]
        private class ConsoleLogger
            : Logger
        {
            private readonly string name;

            public ConsoleLogger(string name)
            {
                this.name = name;
            }

            public override void Debug(Func<string> message)
            {
                Console.WriteLine("Log: {0}\r\n\t {1}".FormatInvariant(this.name, message.Invoke()));
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