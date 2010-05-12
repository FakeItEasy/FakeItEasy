namespace FakeItEasy.Core
{
    using System;

    public class ArgumentInfo
    {
        public bool WasSuccessfullyResolved { get; set; }

        public Type TypeOfArgument { get; set; }

        public object ResolvedValue { get; set; }
    }
}
