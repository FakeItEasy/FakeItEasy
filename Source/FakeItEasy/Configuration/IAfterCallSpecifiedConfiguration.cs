namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Lets you set up expectations and configure repeat for the configured call.
    /// </summary>
    /// <typeparam name="TFake">The type of fake.</typeparam>
    public interface IAfterCallSpecifiedConfiguration
        : IRepeatConfiguration
    {
        
    }
}