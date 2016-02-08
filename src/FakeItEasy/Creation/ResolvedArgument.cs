namespace FakeItEasy.Creation
{
    using System;

    internal class ResolvedArgument
    {
        public Type ArgumentType { get; set; }

        public object ResolvedValue { get; set; }

        public bool WasResolved { get; set; }
    }
}
