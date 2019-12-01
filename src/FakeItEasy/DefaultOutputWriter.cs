namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    internal class DefaultOutputWriter
        : IOutputWriter
    {
        private const string IndentString = "  ";
        private readonly Action<char> output;
        private readonly ArgumentValueFormatter argumentValueFormatter;
        private string currentIndent;
        private WriteState writerState;

        public DefaultOutputWriter(Action<char> output, ArgumentValueFormatter argumentValueFormatter)
        {
            this.output = output;
            this.argumentValueFormatter = argumentValueFormatter;
            this.currentIndent = string.Empty;
            this.writerState = new DefaultWriterState(this);
        }

        public IOutputWriter Write(string value)
        {
            Guard.AgainstNull(value, nameof(value));

            foreach (var character in value)
            {
                this.writerState.Write(character);
            }

            return this;
        }

        public IOutputWriter WriteArgumentValue(object? value)
        {
            return this.Write(this.argumentValueFormatter.GetArgumentValueAsString(value));
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
            public WriteState(DefaultOutputWriter writer)
            {
                this.Writer = writer;
            }

            protected DefaultOutputWriter Writer { get; }

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
