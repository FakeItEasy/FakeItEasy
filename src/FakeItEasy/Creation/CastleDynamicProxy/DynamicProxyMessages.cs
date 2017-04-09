namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;

    internal static class DynamicProxyMessages
    {
        public static string ArgumentsForConstructorDoesNotMatchAnyConstructor =>
            "No constructor matches the passed arguments for constructor.";

        public static string ArgumentsForConstructorOnInterfaceType =>
            "Arguments for constructor specified for interface type.";

        public static string ProxyIsSealedType(Type type) =>
            $"The type of proxy {type} is sealed.";

        public static string ProxyIsValueType(Type type) =>
            $"The type of proxy must be an interface or a class but it was {type}.";

        public static string ProxyTypeWithNoDefaultConstructor(Type type) =>
            $"No usable default constructor was found on the type {type}.";
    }
}
