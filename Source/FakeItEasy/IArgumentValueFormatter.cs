namespace FakeItEasy
{
    using System;

    public interface IArgumentValueFormatter
    {
        Type ForType { get; }
        int Priority { get; }
        string GetArgumentValueAsString(object argumentValue);
    }
}
