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

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            var returnValue = ResolveReturnValue(fakeObjectCall);

            fakeObjectCall.SetReturnValue(returnValue);
        }

        private static object ResolveReturnValue(IInterceptedFakeObjectCall fakeObjectCall)
        {
            object result = null;

            if (!FakeManager.TryCreateDummy(fakeObjectCall.Method.ReturnType, out result))
            {
                result = Helpers.GetDefaultValueOfType(fakeObjectCall.Method.ReturnType);
            }

            return result;
        }
    }
}