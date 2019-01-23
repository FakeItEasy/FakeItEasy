namespace FakeItEasy.Core
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <content>Object member rule.</content>
    public partial class FakeManager
    {
        private class ObjectMemberRule
            : SharedFakeObjectCallRule
        {
            private static readonly MethodInfo EqualsMethod = typeof(object).GetMethod(nameof(object.Equals), new[] { typeof(object) });
            private static readonly MethodInfo ToStringMethod = typeof(object).GetMethod(nameof(object.ToString), Type.EmptyTypes);
            private static readonly MethodInfo GetHashCodeMethod = typeof(object).GetMethod(nameof(object.GetHashCode), Type.EmptyTypes);

            public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall) =>
                IsSameMethod(fakeObjectCall.Method, EqualsMethod) ||
                IsSameMethod(fakeObjectCall.Method, ToStringMethod) ||
                IsSameMethod(fakeObjectCall.Method, GetHashCodeMethod);

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

            private static bool IsSameMethod(MethodInfo first, MethodInfo second)
            {
                return first.DeclaringType == second.DeclaringType
                       && first.MetadataToken == second.MetadataToken
                       && first.Module == second.Module
                       && first.GetGenericArguments().SequenceEqual(second.GetGenericArguments());
            }

            private static bool TryHandleGetHashCode(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
            {
                if (!IsSameMethod(fakeObjectCall.Method, GetHashCodeMethod))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue(fakeManager.GetHashCode());

                return true;
            }

            private static bool TryHandleToString(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
            {
                if (!IsSameMethod(fakeObjectCall.Method, ToStringMethod))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue(fakeManager.FakeObjectDisplayName);

                return true;
            }

            private static bool TryHandleEquals(IInterceptedFakeObjectCall fakeObjectCall, FakeManager fakeManager)
            {
                if (!IsSameMethod(fakeObjectCall.Method, EqualsMethod))
                {
                    return false;
                }

                var argument = fakeObjectCall.Arguments[0];
                if (argument != null)
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
