namespace FakeItEasy.Core
{
    /// <content>Default return value rule.</content>
    public partial class FakeManager
    {
        private class DefaultReturnValueRule
            : SharedFakeObjectCallRule
        {
            public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall) => true;

            public override void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall);

                fakeObjectCall.SetReturnValue(fakeObjectCall.GetDefaultReturnValue());
            }
        }
    }
}
