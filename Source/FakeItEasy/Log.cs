using System.Text;
using FakeItEasy.Core;

namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal static class Log
    {
#if DEBUG
        private const bool UseLogging = false;
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

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Can be instanticated at will.")]
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

    internal class StringBuilderOutputWriter
        : IOutputWriter
    {
        public StringBuilderOutputWriter(StringBuilder builder)
        {
            this.Builder = builder;
        }

        public StringBuilderOutputWriter()
            : this(new StringBuilder())
        {
        }

        public StringBuilder Builder
        {
            get;
            private set;
        }

        public IOutputWriter Write(string value)
        {
            this.Builder.Append(value);
            return this;
        }

        public IOutputWriter WriteArgumentValue(object value)
        {
            if (value == null)
            {
                this.Builder.Append("NULL");
                return this;
            }

            var stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue.Length == 0)
                {
                    this.Builder.Append("string.Empty");
                }
                else
                {
                    this.Builder.Append("\"").Append(stringValue).Append("\"");
                }

                return this;
            }

            this.Builder.Append(value);
            return this;
        }
    }


}