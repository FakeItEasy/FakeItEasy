namespace FakeItEasy
{
    using System;
    using System.Text;

    internal class StringBuilderOutputWriter
        : IOutputWriter
    {
        private const string IndentString = "    ";
        private string currentIndent;
        private WriteState writerState;

        public StringBuilderOutputWriter(StringBuilder builder)
        {
            this.Builder = builder;
            this.currentIndent = string.Empty;
            this.writerState = new DefaultWriterState(this);
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
            foreach (var c in value)
            {
                this.writerState.Write(c);
            }

            return this;
        }

        public IOutputWriter WriteArgumentValue(object value)
        {
            if (value == null)
            {
                this.Write("NULL");
                return this;
            }

            var stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue.Length == 0)
                {
                    this.Write("string.Empty");
                }
                else
                {
                    this.Write("\"").Write(stringValue).Write("\"");
                }

                return this;
            }

            this.Write(value.ToString());
            return this;
        }

        public IDisposable Indent()
        {
            var unindenter = new Unindenter(this, this.currentIndent);
            this.currentIndent = this.currentIndent + IndentString;
            return unindenter;
        }

        private class Unindenter
            : IDisposable
        {
            private readonly StringBuilderOutputWriter writer;
            private readonly string previousIndentString;

            public Unindenter(StringBuilderOutputWriter writer, string previousIndentString)
            {
                this.writer = writer;
                this.previousIndentString = previousIndentString;
            }

            public void Dispose()
            {
                this.writer.currentIndent = this.previousIndentString;
            }
        }

        private abstract class WriteState
        {
            protected readonly StringBuilderOutputWriter writer;

            public WriteState(StringBuilderOutputWriter writer)
            {
                this.writer = writer;
            }

            public abstract void Write(char c);
        }

        private class DefaultWriterState
            : WriteState
        {
            public DefaultWriterState(StringBuilderOutputWriter writer)
                : base(writer)
            {
            }

            public override void Write(char c)
            {
                if (c.Equals('\r') || c.Equals('\n'))
                {
                    this.writer.writerState = new IndentNextNonNewLineCharacterWriterState(this.writer);
                    this.writer.writerState.Write(c);
                }
                else
                {
                    this.writer.Builder.Append(c);
                }
            }
        }

        private class IndentNextNonNewLineCharacterWriterState
            : WriteState
        {
            public IndentNextNonNewLineCharacterWriterState(StringBuilderOutputWriter writer)
                : base(writer)
            {
            }

            public override void Write(char c)
            {
                if (c.Equals('\r') || c.Equals('\n'))
                {
                    this.writer.Builder.Append(c);
                }
                else
                {
                    this.writer.writerState = new DefaultWriterState(this.writer);
                    this.writer.Builder.Append(this.writer.currentIndent);
                    this.writer.Builder.Append(c);
                }
            }
        }
    }
}