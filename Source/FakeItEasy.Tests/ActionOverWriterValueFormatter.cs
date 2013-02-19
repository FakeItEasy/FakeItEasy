﻿namespace FakeItEasy.Tests
{
    using System;

    public class ActionOverWriterValueFormatter : ArgumentValueFormatter<Action<IOutputWriter>>
    {
        protected override string GetStringValue(Action<IOutputWriter> argumentValue)
        {
            var writer = new StringBuilderOutputWriter();
            argumentValue.Invoke(writer);

            return writer.Builder.ToString();
        }
    }
}