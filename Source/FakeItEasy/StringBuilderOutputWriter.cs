namespace FakeItEasy
{
    using System.Text;

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