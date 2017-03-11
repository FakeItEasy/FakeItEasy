namespace FakeItEasy.Messages
{
    internal class TwoPlaceholderMessage
    {
        private readonly string format;

        public TwoPlaceholderMessage(string format)
        {
            this.format = format;
        }

        public static implicit operator TwoPlaceholderMessage(string format)
        {
            return new TwoPlaceholderMessage(format);
        }

        public override string ToString()
        {
            return this.format;
        }

        public string Format(object arg1, object arg2)
        {
            return string.Format(this.format, arg1, arg2);
        }
    }
}