namespace FakeItEasy.Core
{
    using System;

#if FEATURE_BINARY_SERIALIZATION
    [Serializable]
#endif
    internal class DefaultReturnValueRule
        : IFakeObjectCallRule
    {
        public int? NumberOfTimesToCall => null;

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

            fakeObjectCall.SetReturnValue(fakeObjectCall.GetDefaultReturnValue());
        }
    }
}
