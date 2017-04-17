namespace FakeItEasy.Core
{
    using FakeItEasy.Creation;

    internal static class InterceptedFakeObjectCallExtensions
    {
        private static IFakeAndDummyManager DummyManager => ServiceLocator.Current.Resolve<IFakeAndDummyManager>();

        public static object GetDefaultReturnValue(this IInterceptedFakeObjectCall call)
        {
            object result;
            if (!DummyManager.TryCreateDummy(call.Method.ReturnType, out result))
            {
                return call.Method.ReturnType.GetDefaultValue();
            }

            return result;
        }
    }
}
