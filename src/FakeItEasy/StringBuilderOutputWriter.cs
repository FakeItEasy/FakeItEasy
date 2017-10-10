namespace FakeItEasy
{
    using System.Text;
    using FakeItEasy.Core;

    internal class StringBuilderOutputWriter
        : DefaultOutputWriter
    {
        public StringBuilderOutputWriter(StringBuilder builder, ArgumentValueFormatter argumentValueFormatter)
            : base(c => builder.Append(c), argumentValueFormatter)
        {
            this.Builder = builder;
        }

        public delegate StringBuilderOutputWriter Factory(StringBuilder builder);

        public StringBuilder Builder { get; }
    }
}
