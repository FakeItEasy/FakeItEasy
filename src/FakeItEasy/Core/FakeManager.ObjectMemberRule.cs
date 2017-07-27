namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <content>Object member rule.</content>
    public partial class FakeManager
    {
#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        private class ObjectMemberRule
            : IFakeObjectCallRule
        {
            private static readonly List<MethodInfo> ObjectMethods =
                new List<MethodInfo>
                    {
                        typeof(object).GetMethod("Equals", new[] { typeof(object) }),
                        typeof(object).GetMethod("ToString", new Type[] { }),
                        typeof(object).GetMethod("GetHashCode", new Type[] { })
                    };

            private readonly FakeManager fakeManager;
            private readonly IFakeManagerAccessor fakeManagerAccessor;

            public ObjectMemberRule(FakeManager fakeManager, IFakeManagerAccessor fakeManagerAccessor)
            {
                this.fakeManager = fakeManager;
                this.fakeManagerAccessor = fakeManagerAccessor;
            }

            public int? NumberOfTimesToCall => null;

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return IsObjetMethod(fakeObjectCall);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (this.TryHandleToString(fakeObjectCall))
                {
                    return;
                }

                if (this.TryHandleGetHashCode(fakeObjectCall))
                {
                    return;
                }

                if (this.TryHandleEquals(fakeObjectCall))
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

            private static bool IsObjetMethod(IFakeObjectCall fakeObjectCall)
            {
                return ObjectMethods.Any(m => IsSameMethod(m, fakeObjectCall.Method));
            }

            private bool TryHandleGetHashCode(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (!IsSameMethod(fakeObjectCall.Method, ObjectMethods[2]))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue(this.fakeManager.GetHashCode());

                return true;
            }

            private bool TryHandleToString(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (!IsSameMethod(fakeObjectCall.Method, ObjectMethods[1]))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue("Faked " + this.fakeManager.FakeObjectType);

                return true;
            }

            private bool TryHandleEquals(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (!IsSameMethod(fakeObjectCall.Method, ObjectMethods[0]))
                {
                    return false;
                }

                var argument = fakeObjectCall.Arguments[0];
                if (argument != null)
                {
                    var argumentFakeManager = this.fakeManagerAccessor.TryGetFakeManager(argument);
                    bool hasSameFakeManager = ReferenceEquals(argumentFakeManager, this.fakeManager);
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
