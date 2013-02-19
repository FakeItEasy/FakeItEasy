namespace FakeItEasy
{
    using System.Text;

    internal class StringBuilderOutputWriter
        : DefaultOutputWriter
    {
        public StringBuilderOutputWriter(StringBuilder builder)
            : base(c => builder.Append(c))
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
    }
}