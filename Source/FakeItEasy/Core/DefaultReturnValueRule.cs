namespace FakeItEasy.Core
{
    using System;
    using FakeItEasy.Creation;

    [Serializable]
    internal class DefaultReturnValueRule
        : IFakeObjectCallRule
    {
        public int? NumberOfTimesToCall
        {
            get { return null; }
        }

        private static IFakeAndDummyManager FakeManager
        {
            get { return ServiceLocator.Current.Resolve<IFakeAndDummyManager>(); }
        }

        public static object ResolveReturnValue(IInterceptedFakeObjectCall fakeObjectCall)
        {
            object result;
            if (!FakeManager.TryCreateDummy(fakeObjectCall.Method.ReturnType, out result))
            {
                return fakeObjectCall.Method.ReturnType.GetDefaultValue();
            }

            return result;
        }

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

            var returnValue = ResolveReturnValue(fakeObjectCall);
            fakeObjectCall.SetReturnValue(returnValue);
        }
    }
}