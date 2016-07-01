namespace FakeItEasy.Tests
{
    using System;

    public class ActionOverWriterValueFormatter : ArgumentValueFormatter<Action<IOutputWriter>>
    {
        protected override string GetStringValue(Action<IOutputWriter> argumentValue)
        {
            Guard.AgainstNull(argumentValue, nameof(argumentValue));

            var writer = new StringBuilderOutputWriter();
            argumentValue.Invoke(writer);
            return writer.Builder.ToString();
        }
    }
}
