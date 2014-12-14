namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FakeItEasy.SelfInitializedFakes;
    using FluentAssertions;
    using Machine.Specifications;

    public interface ILibraryService
    {
        int GetCount(string internationalStandardBookNumber);
    }

    public class when_self_initializing_a_fake
    {
        private static InMemoryStorage inMemoryStorage;

        private static ILibraryService realServiceWhileRecording;
        private static ILibraryService realServiceDuringPlayback;

        private static int count1ForBook1WhileRecording;
        private static int count1ForBook1DuringPlayback;
        private static int count2ForBook1DuringPlayback;
        private static int count1ForBook2DuringPlayback;
        
        Establish context = () =>
        {
            inMemoryStorage = new InMemoryStorage();

            realServiceWhileRecording = A.Fake<ILibraryService>();
            realServiceDuringPlayback = A.Fake<ILibraryService>();

            A.CallTo(() => realServiceWhileRecording.GetCount("9780345813923"))
                .ReturnsNextFromSequence(11, 10);
            A.CallTo(() => realServiceWhileRecording.GetCount("9781593078225"))
                .Returns(3);
        };

        Because of = () =>
        {
            using (var recorder = new RecordingManager(inMemoryStorage))
            {
                var fakeService = A.Fake<ILibraryService>(options => options
                    .Wrapping(realServiceWhileRecording).RecordedBy(recorder));

                count1ForBook1WhileRecording = fakeService.GetCount("9780345813923");
                fakeService.GetCount("9781593078225");
                fakeService.GetCount("9780345813923");
            }

            using (var recorder = new RecordingManager(inMemoryStorage))
            {
                var playbackFakeService = A.Fake<ILibraryService>(options => options
                    .Wrapping(realServiceDuringPlayback).RecordedBy(recorder));

                count1ForBook1DuringPlayback = playbackFakeService.GetCount("9780345813923");
                count1ForBook2DuringPlayback = playbackFakeService.GetCount("9781593078225");
                count2ForBook1DuringPlayback = playbackFakeService.GetCount("9780345813923");
            }
        };

        It should_forward_calls_to_the_wrapped_service_while_recording =
            () => A.CallTo(() => realServiceWhileRecording.GetCount("9780345813923"))
                .MustHaveHappened(Repeated.Exactly.Twice);

        It should_return_the_result_while_recording =
            () => count1ForBook1WhileRecording.Should().Be(11);

        It should_not_forward_calls_to_the_wrapped_service_during_playback =
            () => A.CallTo(realServiceDuringPlayback).MustNotHaveHappened();

        It should_return_the_recorded_result_for_the_first_set_of_arguments =
            () => count1ForBook1DuringPlayback.Should().Be(11);

        It should_return_the_recorded_result_for_the_second_set_of_arguments =
            () => count1ForBook2DuringPlayback.Should().Be(3);

        It should_return_the_second_recorded_result_when_arguments_are_repeated =
            () => count2ForBook1DuringPlayback.Should().Be(10);

        private class InMemoryStorage : ICallStorage
        {
            private IEnumerable<CallData> recordedCalls;

            public IEnumerable<CallData> Load()
            {
                return this.recordedCalls;
            }

            public void Save(IEnumerable<CallData> calls)
            {
                this.recordedCalls = calls.ToList();
            }
        }
    }
    
    [Tags("explicit")]
    public class when_self_initializing_a_fake_with_a_FileRecorder
    {
        private static string fileRecorderPath;

        private static ILibraryService realServiceWhileRecording;
        private static ILibraryService realServiceDuringPlayback;

        private static int countWhileRecording;
        private static int countDuringPlayback;

        Establish context = () =>
        {
            fileRecorderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            realServiceWhileRecording = A.Fake<ILibraryService>();
            realServiceDuringPlayback = A.Fake<ILibraryService>();

            A.CallTo(() => realServiceWhileRecording.GetCount("9780345813923")).Returns(8);
        };

        Because of = () =>
        {
            try
            {
                using (var recorder = Recorders.FileRecorder(fileRecorderPath))
                {
                    var fakeService = A.Fake<ILibraryService>(options => options
                        .Wrapping(realServiceWhileRecording).RecordedBy(recorder));
                    countWhileRecording = fakeService.GetCount("9780345813923");
                }

                using (var recorder = Recorders.FileRecorder(fileRecorderPath))
                {
                    var playbackFakeService = A.Fake<ILibraryService>(options => options
                        .Wrapping(realServiceDuringPlayback).RecordedBy(recorder));

                    countDuringPlayback = playbackFakeService.GetCount("9780345813923");
                }
            }
            finally
            {
                File.Delete(fileRecorderPath);
            }
        };

        It should_return_the_expected_result_while_recording = () => countWhileRecording.Should().Be(8);

        It should_return_the_recorded_result_during_playback = () => countDuringPlayback.Should().Be(8);

        Cleanup after = () => File.Delete(fileRecorderPath);
    }
}