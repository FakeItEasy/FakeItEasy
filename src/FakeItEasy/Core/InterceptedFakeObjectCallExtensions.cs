namespace FakeItEasy.Core
{
    using FakeItEasy.Creation;

    internal static class InterceptedFakeObjectCallExtensions
    {
        private static IFakeAndDummyManager DummyManager => ServiceLocator.Resolve<IFakeAndDummyManager>();

        public static object GetDefaultReturnValue(this IInterceptedFakeObjectCall call) =>
            DummyManager.TryCreateDummy(call.Method.ReturnType, new LoopDetectingResolutionContext(), out object result)
                ? result
                : call.Method.ReturnType.GetDefaultValue();
    }
}
