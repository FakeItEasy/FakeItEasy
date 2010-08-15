using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Core;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;
using FakeItEasy.SelfInitializedFakes;
using FakeItEasy.Tests.TestHelpers;

namespace FakeItEasy.Tests.SelfInitializedFakes
{
    [TestFixture]
    public class RecordingManagerTests
    {
        private ICallStorage callStorage;
        private List<CallData> recordedCalls;

        [SetUp]
        public void SetUp()
        {
            this.recordedCalls = new List<CallData>();

            this.callStorage = A.Fake<ICallStorage>();
            A.CallTo(() => this.callStorage.Load()).Returns(this.recordedCalls);
        }

        private RecordingManager CreateRecorder()
        {
            return new RecordingManager(this.callStorage);
        }

        [Test]
        public void ApplyCall_should_apply_values_from_recorded_call()
        {
            this.recordedCalls.Add(new CallData(TypeWithOutAndRefFooMethod, new object[] {10, "20" }, 10));

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Method).Returns(TypeWithOutAndRefFooMethod);
            A.CallTo(() => call.Arguments).Returns(new ArgumentCollection(new object[] { 1, "2", null, null }, TypeWithOutAndRefFooMethod));
            
            var recorder = this.CreateRecorder();

            recorder.ApplyNext(call);

            A.CallTo(() => call.SetReturnValue(10)).MustHaveHappened(Repeated.Once);
            A.CallTo(() => call.SetArgumentValue(2, 10)).MustHaveHappened(Repeated.Once);
            A.CallTo(() => call.SetArgumentValue(3, "20")).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void ApplyCall_should_apply_each_recorded_call_once_only_then_use_next_existing_call()
        {
            this.recordedCalls.Add(new CallData(TypeWithOutAndRefFooMethod, new object[] { 10, "20" }, 10));
            this.recordedCalls.Add(new CallData(TypeWithOutAndRefFooMethod, new object[] { 100, "200" }, 100));

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Method).Returns(TypeWithOutAndRefFooMethod);
            A.CallTo(() => call.Arguments).Returns(new ArgumentCollection(new object[] { 1, "2", null, null }, TypeWithOutAndRefFooMethod));

            var recorder = this.CreateRecorder();

            recorder.ApplyNext(call);
            recorder.ApplyNext(call);

            A.CallTo(() => call.SetReturnValue(100)).MustHaveHappened(Repeated.Once.Exactly);
            A.CallTo(() => call.SetArgumentValue(2, 100)).MustHaveHappened(Repeated.Once.Exactly);
            A.CallTo(() => call.SetArgumentValue(3, "200")).MustHaveHappened(Repeated.Once.Exactly);
        }

        [Test]
        public void IsRecording_should_return_true_when_storage_returns_null_from_load()
        {
            // Arrange
            A.CallTo(() => this.callStorage.Load()).Returns(null);

            // Act
            var recorder = this.CreateRecorder();

            // Assert
            Assert.That(recorder.IsRecording);
        }

        [Test]
        public void IsRecording_should_return_false_when_calls_are_loaded_from_storage()
        {
            // Arrange
            A.CallTo(() => this.callStorage.Load()).Returns(this.recordedCalls);

            // Act
            var recorder = this.CreateRecorder();

            // Assert
            Assert.That(recorder.IsRecording, Is.False);
        }

        [Test]
        [SetCulture("en-US")]
        public void ApplyNext_should_throw_when_all_calls_has_been_applied()
        {
            // Arrange
            var method = ExpressionHelper.GetMethod<IFoo>(x => x.Bar());
            this.recordedCalls.Add(new CallData(method, Enumerable.Empty<object>(), null));
            
            var call = this.CreateFakeCall(method);

            // Act
            var recorder = this.CreateRecorder();
            recorder.ApplyNext(call);

            // Assert
            var thrown = Assert.Throws<RecordingException>(() =>
                recorder.ApplyNext(call));

            Assert.That(thrown.Message, Is.EqualTo("All the recorded calls has been applied, the recorded sequence is no longer valid."));
        }

        [Test]
        [SetCulture("en-US")]
        public void ApplyNext_should_throw_when_method_of_call_being_applied_does_not_match_the_next_non_applied_recorded_call()
        {
            // Arrange
            this.recordedCalls.Add(new CallData(ExpressionHelper.GetMethod<IFoo>(x => x.Baz()), new object[] { }, null));
            var call = this.CreateFakeCall(ExpressionHelper.GetMethod<IFoo>(x => x.Bar()));

            // Act
            var recorder = this.CreateRecorder();
            
            // Assert
            var thrown = Assert.Throws<RecordingException>(() =>
                recorder.ApplyNext(call));
            Assert.That(thrown.Message, Text.StartsWith("The method of the call did not match the method of the recorded call, the recorded sequence is no longer valid."));
        }

        [Test]
        public void RecordCall_should_add_call_to_recorded_calls()
        {
            var callToRecord = A.Fake<ICompletedFakeObjectCall>();
            A.CallTo(() => callToRecord.Method).Returns(this.TypeWithOutAndRefFooMethod);
            A.CallTo(() => callToRecord.Arguments).Returns(new ArgumentCollection(new object[] { 1, "2", 3, "4" }, TypeWithOutAndRefFooMethod));
            A.CallTo(() => callToRecord.ReturnValue).Returns(10);

            using (var recorder = this.CreateRecorder())
            {
                recorder.RecordCall(callToRecord);
            }

            A.CallTo(() => this.callStorage.Save(A<IEnumerable<CallData>>.That.Matches(c => this.CallDataMatchesCall(c.Single(), callToRecord)).Argument))
                .MustHaveHappened();
        }

        [Test]
        public void Dispose_should_call_save_with_empty_collection_when_no_calls_has_been_recorded_and_no_previous_recording_exists()
        {
            A.CallTo(() => this.callStorage.Load()).Returns(null);

            using (var recorder = this.CreateRecorder())
            { 
            
            }

            A.CallTo(() => this.callStorage.Save(A<IEnumerable<CallData>>.That.IsThisSequence().Argument))
                .MustHaveHappened();
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

        private MethodInfo TypeWithOutAndRefFooMethod
        {
            get
            {
                return typeof(ITypeWithOutAndRef).GetMethod("Foo");
            }
        }

        private MethodInfo TypeWithOutAndRefBarMethod
        {
            get
            {
                return typeof(ITypeWithOutAndRef).GetMethod("Bar");
            }
        }

        public interface ITypeWithOutAndRef
        {
            int Foo(int a, string b, out int c, ref string d);
            int Bar(int a, string b, out int c, ref string d);
        }

        public abstract class TypeThatTakesCollectionArgument
        {
            public abstract int Foo(IEnumerable<object> argument);
        }
    }
}
