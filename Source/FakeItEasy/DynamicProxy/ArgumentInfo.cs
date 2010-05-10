namespace FakeItEasy.DynamicProxy
{
    using System;

    internal class ArgumentInfo
    {
        public bool WasSuccessfullyResolved { get; set; }

        public Type TypeOfArgument { get; set; }

        public object ResolvedValue { get; set; }
    }
}
