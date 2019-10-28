namespace FakeItEasy.Core
{
    using FakeItEasy.Creation;

    internal static class InterceptedFakeObjectCallExtensions
    {
        private static FakeAndDummyManager DummyManager => ServiceLocator.Resolve<FakeAndDummyManager>();

        public static object? GetDefaultReturnValue(this IInterceptedFakeObjectCall call) =>
            DummyManager.TryCreateDummy(call.Method.ReturnType, new LoopDetectingResolutionContext(), out object? result)
                ? result
                : call.Method.ReturnType.GetDefaultValue();
    }
}
