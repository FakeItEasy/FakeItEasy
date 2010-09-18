namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core.Creation;
    
    internal interface IExceptionThrower
    {
        void ThrowFailedToGenerateProxyWithArgumentsForConstructor(string reasonForFailure);
        void ThrowFailedToGenerateProxyWithResolvedConstructors(Type typeOfFake, string reasonForFailureOfUnspecifiedConstructor, IEnumerable<ResolvedConstructor> resolvedConstructors);
    }
}
