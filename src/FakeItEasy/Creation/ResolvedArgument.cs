namespace FakeItEasy.Creation
{
    using System;

    internal class ResolvedArgument
    {
        public ResolvedArgument(Type argumentType) => this.ArgumentType = argumentType;

        public Type ArgumentType { get; }

        public object? ResolvedValue { get; set; }

        public bool WasResolved { get; set; }
    }
}
