namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;

    internal static class DynamicProxyResources
    {
        public static string ArgumentsForConstructorDoesNotMatchAnyConstructorMessage =>
            "No constructor matches the passed arguments for constructor.";

        public static string ArgumentsForConstructorOnInterfaceTypeMessage =>
            "Arguments for constructor specified for interface type.";

        public static string ProxyIsSealedTypeMessage(Type type) =>
            $@"The type of proxy ""{type}"" is sealed.";

        public static string ProxyIsValueTypeMessage(Type type) =>
            $"The type of proxy must be an interface or a class but it was {type}.";

        public static string ProxyTypeWithNoDefaultConstructorMessage(Type type) =>
            $"No usable default constructor was found on the type {type}.";
    }
}
