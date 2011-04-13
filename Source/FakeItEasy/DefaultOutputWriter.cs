namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal class DefaultOutputWriter
        : IOutputWriter
    {
        private readonly Action<char> output;
        private const string IndentString = "  ";
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

        private void AppendIndent()
        {
            for (int i = 0; i < this.currentIndent.Length; i++)
            {
                this.output.Invoke(this.currentIndent[i]);
            }
        }

        private class Unindenter
            : IDisposable
        {
            private readonly DefaultOutputWriter writer;
            private readonly string previousIndentString;

            public Unindenter(DefaultOutputWriter writer, string previousIndentString)
            {
                this.writer = writer;
                this.previousIndentString = previousIndentString;
            }


            [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "This is not used to dispose any unmanaged resources.")]
            public void Dispose()
            {
                this.writer.currentIndent = this.previousIndentString;
            }
        }

        private abstract class WriteState
        {
            protected readonly DefaultOutputWriter writer;

            public WriteState(DefaultOutputWriter writer)
            {
                this.writer = writer;
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
                    this.writer.writerState = new IndentNextNonNewLineCharacterWriterState(this.writer);
                    this.writer.writerState.Write(c);
                }
                else
                {
                    this.writer.output.Invoke(c);
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
                    this.writer.writerState = new DefaultWriterState(this.writer);
                    this.writer.AppendIndent();
                }
                
                this.writer.output.Invoke(c);
            }
        }
    }
}