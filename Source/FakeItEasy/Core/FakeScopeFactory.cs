namespace FakeItEasy.Core
{
    internal class FakeScopeFactory
        : IFakeScopeFactory
    {
        public IFakeScope Create()
        {
            return FakeScope.Create();
        }

        public IFakeScope Create(IFakeObjectContainer container)
        {
            return FakeScope.Create(container);
        }
    }
}