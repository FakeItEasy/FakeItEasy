namespace FakeItEasy.Core
{
#if FEATURE_BINARY_SERIALIZATION
    using System;
#endif

    /// <content>Default return value rule.</content>
    public partial class FakeManager
    {
#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        private class DefaultReturnValueRule
            : SharedFakeObjectCallRule
        {
            public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall) => true;

            public override void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                fakeObjectCall.SetReturnValue(fakeObjectCall.GetDefaultReturnValue());
            }
        }
    }
}
