namespace FakeItEasy.Core;

using static FakeItEasy.ObjectMethod;

/// <content>Object member rule.</content>
public partial class FakeManager
{
    private class ObjectMemberRule
        : SharedFakeObjectCallRule
    {
        public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall) =>
            fakeObjectCall.Method.GetObjectMethod() != None;

        public override void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            var fakeManager = Fake.GetFakeManager(fakeObjectCall.FakedObject);
            switch (fakeObjectCall.Method.GetObjectMethod())
            {
                case ToStringMethod:
                    HandleToString(fakeObjectCall, fakeManager);
                    return;

                case GetHashCodeMethod:
                    HandleGetHashCode(fakeObjectCall, fakeManager);
                    return;

                case EqualsMethod:
                    HandleEquals(fakeObjectCall, fakeManager);
                    return;
            }
        }

        private static void HandleGetHashCode(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
        {
            fakeObjectCall.SetReturnValue(fakeManager.GetHashCode());
        }

        private static void HandleToString(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
        {
            fakeObjectCall.SetReturnValue(fakeManager.FakeObjectDisplayName);
        }

        private static void HandleEquals(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
        {
            var argument = fakeObjectCall.Arguments[0];
            if (argument is not null)
            {
                Fake.TryGetFakeManager(argument, out var argumentFakeManager);
                bool hasSameFakeManager = ReferenceEquals(argumentFakeManager, fakeManager);
                fakeObjectCall.SetReturnValue(hasSameFakeManager);
            }
            else
            {
                fakeObjectCall.SetReturnValue(false);
            }
        }
    }
}
