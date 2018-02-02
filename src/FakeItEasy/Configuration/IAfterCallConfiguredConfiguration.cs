namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Lets you set up expectations and configure number of occurrences for the configured call.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface IAfterCallConfiguredConfiguration<out TInterface>
        : IRepeatConfiguration<TInterface>
    {
    }
}
