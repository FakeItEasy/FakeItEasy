namespace FakeItEasy.Core
{
    internal class FakeScopeFactory
        : IFakeScopeFactory
    {
        public IFakeScope Create()
        {
            return FakeScope.Create();
        }
    }
}
