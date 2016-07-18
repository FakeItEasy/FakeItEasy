#if FEATURE_SELF_INITIALIZED_FAKES
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
#endif
