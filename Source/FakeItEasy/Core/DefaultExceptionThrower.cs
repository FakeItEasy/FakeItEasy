namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core.Creation;

    internal class DefaultExceptionThrower
        : IExceptionThrower
    {
        public void ThrowFailedToGenerateProxyWithArgumentsForConstructor(string reasonForFailure)
        {
            throw new FakeCreationException();
        }

        public void ThrowFailedToGenerateProxyWithResolvedConstructors(Type typeOfFake, string reasonForFailureOfUnspecifiedConstructor, IEnumerable<ResolvedConstructor> resolvedConstructors)
        {
            throw new FakeCreationException();
        }
    }
}
