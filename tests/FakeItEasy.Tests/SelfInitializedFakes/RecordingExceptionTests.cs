#pragma warning disable CS0618 // Type or member is obsolete
namespace FakeItEasy.Tests.SelfInitializedFakes
{
    using FakeItEasy.SelfInitializedFakes;

    public class RecordingExceptionTests
        : ExceptionContractTests<RecordingException>
    {
        protected override RecordingException CreateException()
        {
            return new RecordingException();
        }
    }
}
