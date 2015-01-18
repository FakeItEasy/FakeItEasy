namespace FakeItEasy.Tests.SelfInitializedFakes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.SelfInitializedFakes;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class RecordingManagerTests
    {
        private ICallStorage callStorage;
        private List<CallData> recordedCalls;

        private MethodInfo TypeWithOutAndRefFooMethod
        {
            get { return typeof(IOutputAndRef).GetMethod("Foo"); }
        }

        [SetUp]
        public void Setup()
        {
            this.recordedCalls = new List<CallData>();

            this.callStorage = A.Fake<ICallStorage>();
            A.CallTo(() => this.callStorage.Load()).Returns(this.recordedCalls);
        }

        [Test]
        public void ApplyCall_should_apply_values_from_recorded_call()
        {
            this.recordedCalls.Add(new CallData(this.TypeWithOutAndRefFooMethod, new object[] { 10, "20" }, 10));

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Method).Returns(this.TypeWithOutAndRefFooMethod);
            A.CallTo(() => call.Arguments).Returns(new ArgumentCollection(new object[] { 1, "2", null, null }, this.TypeWithOutAndRefFooMethod));

            var recorder = this.CreateRecorder();

            recorder.ApplyNext(call);

            A.CallTo(() => call.SetReturnValue(10)).MustHaveHappened();
            A.CallTo(() => call.SetArgumentValue(2, 10)).MustHaveHappened();
            A.CallTo(() => call.SetArgumentValue(3, "20")).MustHaveHappened();
        }

        [Test]
        public void ApplyCall_should_apply_each_recorded_call_once_only_then_use_next_existing_call()
        {
            this.recordedCalls.Add(new CallData(this.TypeWithOutAndRefFooMethod, new object[] { 10, "20" }, 10));
            this.recordedCalls.Add(new CallData(this.TypeWithOutAndRefFooMethod, new object[] { 100, "200" }, 100));

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Method).Returns(this.TypeWithOutAndRefFooMethod);
            A.CallTo(() => call.Arguments).Returns(new ArgumentCollection(new object[] { 1, "2", null, null }, this.TypeWithOutAndRefFooMethod));

            var recorder = this.CreateRecorder();

            recorder.ApplyNext(call);
            recorder.ApplyNext(call);

            A.CallTo(() => call.SetReturnValue(100)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => call.SetArgumentValue(2, 100)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => call.SetArgumentValue(3, "200")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void IsRecording_should_return_true_when_storage_returns_null_from_load()
        {
            // Arrange
            A.CallTo(() => this.callStorage.Load()).Returns(null);

            // Act
            var recorder = this.CreateRecorder();

            // Assert
            recorder.IsRecording.Should().BeTrue();
        }

        [Test]
        public void IsRecording_should_return_false_when_calls_are_loaded_from_storage()
        {
            // Arrange
            A.CallTo(() => this.callStorage.Load()).Returns(this.recordedCalls);

            // Act
            var recorder = this.CreateRecorder();

            // Assert
            recorder.IsRecording.Should().BeFalse();
        }

        [Test]
        [SetCulture("en-US")]
        public void ApplyNext_should_throw_when_all_calls_have_been_applied()
        {
            // Arrange
            var method = ExpressionHelper.GetMethod<IFoo>(x => x.Bar());
            this.recordedCalls.Add(new CallData(method, Enumerable.Empty<object>(), null));

            var call = this.CreateFakeCall(method);

            var recorder = this.CreateRecorder();
            recorder.ApplyNext(call);

            // Act
            var exception = Record.Exception(() => recorder.ApplyNext(call));

            // Assert
            exception.Should()
                .BeAnExceptionOfType<RecordingException>()
                .WithMessage("All the recorded calls has been applied, the recorded sequence is no longer valid.");
        }

        [Test]
        [SetCulture("en-US")]
        public void ApplyNext_should_throw_when_method_of_call_being_applied_does_not_match_the_next_non_applied_recorded_call()
        {
            // Arrange
            this.recordedCalls.Add(new CallData(ExpressionHelper.GetMethod<IFoo>(x => x.Baz()), new object[] { }, null));
            var call = this.CreateFakeCall(ExpressionHelper.GetMethod<IFoo>(x => x.Bar()));
            var recorder = this.CreateRecorder();

            // Act
            var exception = Record.Exception(() => recorder.ApplyNext(call));

            // Assert
            exception.Should()
                .BeAnExceptionOfType<RecordingException>()
                .WithMessage("The method of the call did not match the method of the recorded call, the recorded sequence is no longer valid.*");
        }

        [Test]
        public void RecordCall_should_add_call_to_recorded_calls()
        {
            var callToRecord = A.Fake<ICompletedFakeObjectCall>();
            A.CallTo(() => callToRecord.Method).Returns(this.TypeWithOutAndRefFooMethod);
            A.CallTo(() => callToRecord.Arguments).Returns(new ArgumentCollection(new object[] { 1, "2", 3, "4" }, this.TypeWithOutAndRefFooMethod));
            A.CallTo(() => callToRecord.ReturnValue).Returns(10);

            using (var recorder = this.CreateRecorder())
            {
                recorder.RecordCall(callToRecord);
            }

            A.CallTo(() => this.callStorage.Save(A<IEnumerable<CallData>>.That.Matches(c => this.CallDataMatchesCall(c.Single(), callToRecord))))
                .MustHaveHappened();
        }

        [Test]
        public void Dispose_should_call_save_with_empty_collection_when_no_calls_have_been_recorded_and_no_previous_recording_exists()
        {
            A.CallTo(() => this.callStorage.Load()).Returns(null);

            using (this.CreateRecorder())
            {
            }

            A.CallTo(() => this.callStorage.Save(A<IEnumerable<CallData>>.That.IsThisSequence()))
                .MustHaveHappened();
        }

        private RecordingManager CreateRecorder()
        {
            return new RecordingManager(this.callStorage);
        }

        private IInterceptedFakeObjectCall CreateFakeCall(MethodInfo method)
        {
            var result = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => result.Method).Returns(method);

            return result;
        }

        private bool CallDataMatchesCall(CallData callData, ICompletedFakeObjectCall recordedCall)
        {
            return callData.Method.Equals(recordedCall.Method)
                && callData.OutputArguments.SequenceEqual(new object[] { 3, "4" })
                && callData.ReturnValue.Equals(10);
        }
    }
}
