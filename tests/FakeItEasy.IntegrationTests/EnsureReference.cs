namespace FakeItEasy.IntegrationTests
{
    using FakeItEasy.Tests.ExtensionPoints;

    #pragma warning disable CA1812
    internal static class EnsureReference
    #pragma warning restore CA1812
    {
        private static void EnsureReferenceIsKept()
        {
            // This is never called, but ensures that the reference to FakeItEasy.Tests.ExtensionPoints is not removed.
            _ = new DayOfWeekDummyFactory();
        }
    }
}
