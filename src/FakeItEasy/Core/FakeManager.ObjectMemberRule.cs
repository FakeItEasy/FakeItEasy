namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Creation;

    /// <content>Object member rule.</content>
    public partial class FakeManager
    {
#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        private class ObjectMemberRule
            : IFakeObjectCallRule
        {
            private static readonly MethodInfo ObjectEqualsMethod = typeof(object).GetMethod("Equals", new[] { typeof(object) });
            private static readonly MethodInfo ObjectGetHashCodeMethod = typeof(object).GetMethod("GetHashCode", new Type[] { });
            private static readonly MethodInfo ObjectToStringMethod = typeof(object).GetMethod("ToString", new Type[] { });
            private static readonly MethodInfo FakeObjectEqualsMethod = typeof(IFakeObject).GetMethod("Equals", new[] { typeof(object) });
            private static readonly MethodInfo FakeObjectGetHashCodeMethod = typeof(IFakeObject).GetMethod("GetHashCode", new Type[] { });
            private static readonly MethodInfo FakeObjectToStringMethod = typeof(IFakeObject).GetMethod("ToString", new Type[] { });

            private static readonly List<MethodInfo> ObjectMethods =
                new List<MethodInfo>
                    {
                        ObjectEqualsMethod,
                        ObjectGetHashCodeMethod,
                        ObjectToStringMethod,
                        FakeObjectEqualsMethod,
                        FakeObjectGetHashCodeMethod,
                        FakeObjectToStringMethod,
                    };

            private readonly FakeManager fakeManager;

            public ObjectMemberRule(FakeManager fakeManager)
            {
                this.fakeManager = fakeManager;
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
                if (!IsSameMethod(fakeObjectCall.Method, ObjectGetHashCodeMethod) &&
                    !IsSameMethod(fakeObjectCall.Method, FakeObjectGetHashCodeMethod))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue(this.fakeManager.GetHashCode());

                return true;
            }

            private bool TryHandleToString(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (!IsSameMethod(fakeObjectCall.Method, ObjectToStringMethod) &&
                    !IsSameMethod(fakeObjectCall.Method, FakeObjectToStringMethod))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue("Faked {0}".FormatInvariant(this.fakeManager.FakeObjectType.FullName));

                return true;
            }

            private bool TryHandleEquals(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (!IsSameMethod(fakeObjectCall.Method, ObjectEqualsMethod) &&
                    !IsSameMethod(fakeObjectCall.Method, FakeObjectEqualsMethod))
                {
                    return false;
                }

                var argument = fakeObjectCall.Arguments[0] as ITaggable;

                if (argument != null)
                {
                    fakeObjectCall.SetReturnValue(argument.Tag.Equals(this.fakeManager));
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
