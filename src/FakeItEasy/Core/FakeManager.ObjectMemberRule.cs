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
                fakeObjectCall.Method.HasSameBaseMethodAs(EqualsMethod) ||
                fakeObjectCall.Method.HasSameBaseMethodAs(ToStringMethod) ||
                fakeObjectCall.Method.HasSameBaseMethodAs(GetHashCodeMethod);

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
                if (!fakeObjectCall.Method.HasSameBaseMethodAs(GetHashCodeMethod))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue(fakeManager.GetHashCode());

                return true;
            }

            private static bool TryHandleToString(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
            {
                if (!fakeObjectCall.Method.HasSameBaseMethodAs(ToStringMethod))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue(fakeManager.FakeObjectDisplayName);

                return true;
            }

            private static bool TryHandleEquals(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
            {
                if (!fakeObjectCall.Method.HasSameBaseMethodAs(EqualsMethod))
                {
                    return false;
                }

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

                return true;
            }
        }
    }
}
