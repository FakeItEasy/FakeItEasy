namespace FakeItEasy.Messages
{
    internal class NoPlaceholderMessage
    {
        private readonly string value;

        public NoPlaceholderMessage(string value)
        {
            this.value = value;
        }

        public static implicit operator NoPlaceholderMessage(string value)
        {
            return new NoPlaceholderMessage(value);
        }

        public static implicit operator string(NoPlaceholderMessage message)
        {
            return message.value;
        }

        public override string ToString()
        {
            return this.value;
        }
    }
}