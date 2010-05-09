namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public partial class FakeManager
    {
        [Serializable]
        private class ObjectMemberRule
            : IFakeObjectCallRule
        {
            private static readonly List<MethodInfo> objectMethods = new List<MethodInfo>()
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

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                if (this.TryHandleToString(fakeObjectCall))
                {
                    return;
                }

                if (TryHandleGetHashCode(fakeObjectCall))
                {
                    return;
                }

                if (this.TryHandleEquals(fakeObjectCall))
                {
                    return;
                }
            }

            private static bool IsObjetMethod(IFakeObjectCall fakeObjectCall)
            {
                return objectMethods.Contains(fakeObjectCall.Method);
            }
            
            private static bool TryHandleGetHashCode(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.Equals(objectMethods[2]))
                {
                    return false;
                }

                var fakeManager = Fake.GetFakeManager(fakeObjectCall.FakedObject);
                fakeObjectCall.SetReturnValue(fakeManager.GetHashCode());

                return true;
            }

            private bool TryHandleToString(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.Equals(objectMethods[1]))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue("Faked {0}".FormatInvariant(this.FakeManager.FakeObjectType.FullName));

                return true;
            }

            private bool TryHandleEquals(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.Equals(objectMethods[0]))
                {
                    return false;
                }

                var argument = fakeObjectCall.Arguments[0] as IFakedProxy;
                if (argument != null)
                {
                    fakeObjectCall.SetReturnValue(argument.FakeManager.Equals(this.FakeManager));
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
