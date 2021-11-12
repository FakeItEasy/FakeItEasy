namespace FakeItEasy.Configuration
{
    internal static class AnyCallConfigurationWithNoReturnTypeSpecifiedExtensions
    {
        public static IAnyCallConfigurationWithVoidReturnType MatchingEventAction(
            this IAnyCallConfigurationWithNoReturnTypeSpecified configuration,
            object fake,
            EventAction action)
        {
            Guard.AgainstNull(configuration);
            Guard.AgainstNull(fake);
            Guard.AgainstNull(action);
            return configuration.WithVoidReturnType().Where(action.Matches, writer => action.WriteDescription(fake, writer));
        }
    }
}
