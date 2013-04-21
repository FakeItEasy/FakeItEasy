namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal class DefaultOutputWriter
        : IOutputWriter
    {
        private const string IndentString = "  ";
        private readonly Action<char> output;
        private string currentIndent;
        private WriteState writerState;

        public DefaultOutputWriter(Action<char> output)
        {
            this.output = output;
            this.currentIndent = string.Empty;
            this.writerState = new DefaultWriterState(this);
        }

        public IOutputWriter Write(string value)
        {
            Guard.AgainstNull(value, "value");

            foreach (var character in value)
            {
                this.writerState.Write(character);
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

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Not critical that dispose runs in all exception paths.")]
        public IDisposable Indent()
        {
            var unindenter = new Unindenter(this, this.currentIndent);
            this.currentIndent = this.currentIndent + IndentString;
            return unindenter;
        }

        private void AppendIndent()
        {
            for (int i = 0; i < this.currentIndent.Length; i++)
            {
                this.output.Invoke(this.currentIndent[i]);
            }
        }

        private sealed class Unindenter
            : IDisposable
        {
            private readonly DefaultOutputWriter writer;
            private readonly string previousIndentString;

            public Unindenter(DefaultOutputWriter writer, string previousIndentString)
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
            protected readonly DefaultOutputWriter Writer;

            public WriteState(DefaultOutputWriter writer)
            {
                this.Writer = writer;
            }

            public abstract void Write(char c);
        }

        private class DefaultWriterState
            : WriteState
        {
            public DefaultWriterState(DefaultOutputWriter writer)
                : base(writer)
            {
            }

            public override void Write(char c)
            {
                if (c.Equals('\r') || c.Equals('\n'))
                {
                    this.Writer.writerState = new IndentNextNonNewLineCharacterWriterState(this.Writer);
                    this.Writer.writerState.Write(c);
                }
                else
                {
                    this.Writer.output.Invoke(c);
                }
            }
        }

        private class IndentNextNonNewLineCharacterWriterState
            : WriteState
        {
            public IndentNextNonNewLineCharacterWriterState(DefaultOutputWriter writer)
                : base(writer)
            {
            }

            public override void Write(char c)
            {
                if (!c.Equals('\r') && !c.Equals('\n'))
                {
                    this.Writer.writerState = new DefaultWriterState(this.Writer);
                    this.Writer.AppendIndent();
                }

                this.Writer.output.Invoke(c);
            }
        }
    }
}