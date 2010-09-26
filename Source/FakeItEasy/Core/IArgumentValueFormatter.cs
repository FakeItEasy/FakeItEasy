namespace FakeItEasy.Core
{
    using System;

    public interface IArgumentValueFormatter
    {
        Type ForType { get; }
        int Priority { get; }
        string GetArgumentValueAsString(object argumentValue);
    }
}
