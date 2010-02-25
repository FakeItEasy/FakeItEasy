namespace FakeItEasy.Assertion
{
    using FakeItEasy.Api;

    public interface IFakeAssertionsFactory
    {
        IFakeAssertions<TFake> CreateAsserter<TFake>(FakeObject fake);
    }
}
