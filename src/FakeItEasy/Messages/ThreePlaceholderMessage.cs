namespace FakeItEasy.Messages
{
    internal struct ThreePlaceholderMessage
    {
        private readonly string format;

        public ThreePlaceholderMessage(string format)
        {
            this.format = format;
        }

        public static implicit operator ThreePlaceholderMessage(string format)
        {
            return new ThreePlaceholderMessage(format);
        }

        public override string ToString()
        {
            return this.format;
        }

        public string Format(object arg1, object arg2, object arg3)
        {
            return string.Format(this.format, arg1, arg2, arg3);
        }
    }
}
