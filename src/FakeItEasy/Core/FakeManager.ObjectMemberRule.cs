namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
#if !FEATURE_REFLECTION_METADATATOKEN
    using System.Linq;
#endif
    using System.Reflection;
    using FakeItEasy.Creation;

    /// <content>Object member rule.</content>
    public partial class FakeManager
    {
#if FEATURE_SERIALIZATION
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

            public FakeManager FakeManager { get; set; }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

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
#if FEATURE_REFLECTION_METADATATOKEN
                   && first.MetadataToken == second.MetadataToken
#else
                   && first.Name == second.Name
                   && first.GetParameters().Select(p => p.ParameterType).SequenceEqual(second.GetParameters().Select(p => p.ParameterType))
#endif
                   && first.Module == second.Module
                   && first.GetGenericArguments().SequenceEqual(second.GetGenericArguments());
            }

            private static bool IsObjetMethod(IFakeObjectCall fakeObjectCall)
            {
                return IsSameMethod(ObjectMethods[0], fakeObjectCall.Method)
                    || IsSameMethod(ObjectMethods[1], fakeObjectCall.Method)
                    || IsSameMethod(ObjectMethods[2], fakeObjectCall.Method);
            }

            private bool TryHandleGetHashCode(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (!IsSameMethod(fakeObjectCall.Method, ObjectMethods[2]))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue(this.FakeManager.GetHashCode());

                return true;
            }

            private bool TryHandleToString(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (!IsSameMethod(fakeObjectCall.Method, ObjectMethods[1]))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue("Faked {0}".FormatInvariant(this.FakeManager.FakeObjectType.FullName));

                return true;
            }

            private bool TryHandleEquals(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (!IsSameMethod(fakeObjectCall.Method, ObjectMethods[0]))
                {
                    return false;
                }

                var argument = fakeObjectCall.Arguments[0] as ITaggable;

                if (argument != null)
                {
                    fakeObjectCall.SetReturnValue(argument.Tag.Equals(this.FakeManager));
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
