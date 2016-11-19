namespace FakeItEasy.Core
{
    using FakeItEasy.Creation;

    internal static class InterceptedFakeObjectCallExtensions
    {
        public static object GetDefaultReturnValue(this IInterceptedFakeObjectCall call)
        {
            var dummyManager = ServiceLocator.Current.Resolve<IFakeAndDummyManager>();
            object result;
            if (!dummyManager.TryCreateDummy(call.Method.ReturnType, out result))
            {
                return call.Method.ReturnType.GetDefaultValue();
            }

            return result;
        }
    }
}
