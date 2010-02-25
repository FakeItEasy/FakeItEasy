namespace FakeItEasy.Tests.SelfInitializedFakes
{
    using FakeItEasy.SelfInitializedFakes;
    using NUnit.Framework;
    
    [TestFixture]
    public class RecordingExceptionTests
        : ExceptionContractTests<RecordingException>
    {
        protected override RecordingException CreateException()
        {
            return new RecordingException();
        }
    }
}
