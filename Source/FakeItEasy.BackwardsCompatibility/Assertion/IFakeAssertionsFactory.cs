namespace FakeItEasy.Assertion
{
    using FakeItEasy.Core;

    internal interface IFakeAssertionsFactory
    {
        IFakeAssertions<TFake> CreateAsserter<TFake>(FakeManager fake);
    }
}
