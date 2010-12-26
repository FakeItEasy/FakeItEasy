namespace FakeItEasy.Core
{
    internal interface IFakeScopeFactory
    {
        IFakeScope Create();

        IFakeScope Create(IFakeObjectContainer container);
    }
}