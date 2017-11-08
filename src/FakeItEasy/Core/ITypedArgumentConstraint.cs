namespace FakeItEasy.Core
{
    using System;

    internal interface ITypedArgumentConstraint : IArgumentConstraint
    {
        Type Type { get; }
    }
}
