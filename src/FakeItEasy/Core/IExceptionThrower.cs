namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Creation;

    internal interface IExceptionThrower
    {
        void ThrowFailedToGenerateProxyWithoutTryingConstructors(Type typeOfFake, string reasonForFailure);

        void ThrowFailedToGenerateProxyWithArgumentsForConstructor(Type typeOfFake, string reasonForFailure);

        void ThrowFailedToGenerateProxyWithResolvedConstructors(Type typeOfFake, string reasonForFailureOfUnspecifiedConstructor, IEnumerable<ResolvedConstructor> resolvedConstructors);
    }
}
