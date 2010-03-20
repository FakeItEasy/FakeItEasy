namespace FakeItEasy.Assertion
{
    using FakeItEasy.Core;

    public interface IFakeAssertionsFactory
    {
        IFakeAssertions<TFake> CreateAsserter<TFake>(FakeObject fake);
    }
}
