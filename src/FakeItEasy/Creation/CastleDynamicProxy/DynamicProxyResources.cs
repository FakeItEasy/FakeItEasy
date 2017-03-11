namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using FakeItEasy.Messages;

    internal static class DynamicProxyResources
    {
        public static NoPlaceholderMessage ArgumentsForConstructorDoesNotMatchAnyConstructorMessage =>
            @"No constructor matches the passed arguments for constructor.";

        public static NoPlaceholderMessage ArgumentsForConstructorOnInterfaceTypeMessage =>
            @"Arguments for constructor specified for interface type.";

        public static OnePlaceholderMessage ProxyIsSealedTypeMessage =>
            @"The type of proxy ""{0}"" is sealed.";

        public static OnePlaceholderMessage ProxyIsValueTypeMessage =>
            @"The type of proxy must be an interface or a class but it was {0}.";

        public static OnePlaceholderMessage ProxyTypeWithNoDefaultConstructorMessage =>
            @"No usable default constructor was found on the type {0}.";
    }
}
