namespace FakeItEasy.Tests
{
    using System;
    using System.Text;

    public class ActionOverWriterValueFormatter : ArgumentValueFormatter<Action<IOutputWriter>>
    {
        protected override string GetStringValue(Action<IOutputWriter> argumentValue)
        {
            Guard.AgainstNull(argumentValue, nameof(argumentValue));

            var writer = ServiceLocator.Current.Resolve<StringBuilderOutputWriter>();
            argumentValue.Invoke(writer);
            return writer.Builder.ToString();
        }
    }
}
