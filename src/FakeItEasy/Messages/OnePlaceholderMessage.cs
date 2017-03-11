namespace FakeItEasy.Messages
{
    internal class OnePlaceholderMessage
    {
        private readonly string format;

        public OnePlaceholderMessage(string format)
        {
            this.format = format;
        }

        public static implicit operator OnePlaceholderMessage(string format)
        {
            return new OnePlaceholderMessage(format);
        }

        public override string ToString()
        {
            return this.format;
        }

        public string Format(object arg)
        {
            return string.Format(this.format, arg);
        }
    }
}