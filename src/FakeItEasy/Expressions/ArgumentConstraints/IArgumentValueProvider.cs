namespace FakeItEasy.Expressions.ArgumentConstraints
{
    /// <summary>
    /// An object that may provide argument values, for example as implicitly-defined values for
    /// methods' out parameters.
    /// </summary>
    internal interface IArgumentValueProvider
    {
        object? Value { get; }
    }
}
