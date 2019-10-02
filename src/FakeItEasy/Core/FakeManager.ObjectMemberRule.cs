namespace FakeItEasy.Core
{
    using static ObjectMembers;

    /// <content>Object member rule.</content>
    public partial class FakeManager
    {
        private class ObjectMemberRule
            : SharedFakeObjectCallRule
        {
            public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall) =>
                fakeObjectCall.Method.IsSameMethodAs(EqualsMethod) ||
                fakeObjectCall.Method.IsSameMethodAs(ToStringMethod) ||
                fakeObjectCall.Method.IsSameMethodAs(GetHashCodeMethod);

            public override void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                var fakeManager = Fake.GetFakeManager(fakeObjectCall.FakedObject);
                if (TryHandleToString(fakeObjectCall, fakeManager))
                {
                    return;
                }

                if (TryHandleGetHashCode(fakeObjectCall, fakeManager))
                {
                    return;
                }

                if (TryHandleEquals(fakeObjectCall, fakeManager))
                {
                    return;
                }
            }

            private static bool TryHandleGetHashCode(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
            {
                if (!fakeObjectCall.Method.IsSameMethodAs(GetHashCodeMethod))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue(fakeManager.GetHashCode());

                return true;
            }

            private static bool TryHandleToString(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
            {
                if (!fakeObjectCall.Method.IsSameMethodAs(ToStringMethod))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue(fakeManager.FakeObjectDisplayName);

                return true;
            }

            private static bool TryHandleEquals(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
            {
                if (!fakeObjectCall.Method.IsSameMethodAs(EqualsMethod))
                {
                    return false;
                }

                var argument = fakeObjectCall.Arguments[0];
                if (argument is object)
                {
                    var argumentFakeManager = Fake.TryGetFakeManager(argument);
                    bool hasSameFakeManager = ReferenceEquals(argumentFakeManager, fakeManager);
                    fakeObjectCall.SetReturnValue(hasSameFakeManager);
                }
                else
                {
                    fakeObjectCall.SetReturnValue(false);
                }

                return true;
            }
        }
    }
}
