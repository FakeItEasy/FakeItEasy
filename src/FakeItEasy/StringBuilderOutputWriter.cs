namespace FakeItEasy;

using System.Text;
using FakeItEasy.Core;

internal class StringBuilderOutputWriter
    : DefaultOutputWriter
{
    public StringBuilderOutputWriter(ArgumentValueFormatter argumentValueFormatter)
        : this(new StringBuilder(), argumentValueFormatter)
    {
    }

    private StringBuilderOutputWriter(StringBuilder builder, ArgumentValueFormatter argumentValueFormatter)
        : base(c => builder.Append(c), argumentValueFormatter)
    {
        this.Builder = builder;
    }

    public delegate StringBuilderOutputWriter Factory();

    public StringBuilder Builder { get; }
}
