using System;
using System.Reflection;
using System.Collections.Generic;

namespace FakeItEasy.Core
{
    public partial class FakeObject
    {
        [Serializable]
        private class ObjectMemberRule
            : IFakeObjectCallRule
        {
            public FakeObject FakeObject;

            private static readonly List<MethodInfo> objectMethods = new List<MethodInfo>()
            {
                typeof(object).GetMethod("Equals", new [] { typeof(object) }),
                typeof(object).GetMethod("ToString", new Type[] { }),
                typeof(object).GetMethod("GetHashCode", new Type[] { })
            };

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return IsObjetMethod(fakeObjectCall);
            }

            private static bool IsObjetMethod(IFakeObjectCall fakeObjectCall)
            {
                return objectMethods.Contains(fakeObjectCall.Method);
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                if (TryHandleToString(fakeObjectCall))
                {
                    return;
                }

                if (TryHandleGetHashCode(fakeObjectCall))
                {
                    return;
                }

                if (TryHandleEquals(fakeObjectCall))
                {
                    return;
                }
            }

            private bool TryHandleToString(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.Equals(objectMethods[1]))
                {
                    return false;
                }

                fakeObjectCall.SetReturnValue("Faked {0}".FormatInvariant(this.FakeObject.FakeObjectType.FullName));

                return true;
            }

            private static bool TryHandleGetHashCode(IWritableFakeObjectCall fakeObjectCall)
            {
                if (!fakeObjectCall.Method.Equals(objectMethods[2]))
                {
                    return false;
                }

                var fakeObject = Fake.GetFakeObject(fakeObjectCall.FakedObject);
                fakeObjectCall.SetReturnValue(fakeObject.GetHashCode());

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
                    fakeObjectCall.SetReturnValue(argument.FakeObject.Equals(this.FakeObject));
                }
                else
                {
                    fakeObjectCall.SetReturnValue(false);
                }

                return true;
            }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }
        }
    }
}
