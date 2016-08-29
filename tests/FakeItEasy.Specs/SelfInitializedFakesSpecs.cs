namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FakeItEasy.SelfInitializedFakes;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public interface ILibraryService
    {
        int GetCount(string internationalStandardBookNumber);
    }

    public static class SelfInitializedFakesSpecs
    {
        [Scenario]
        public static void SelfInitializing(
            InMemoryStorage inMemoryStorage,
            ILibraryService realServiceWhileRecording,
            ILibraryService realServiceDuringPlayback,
            int count1ForBook1WhileRecording,
            int count1ForBook1DuringPlayback,
            int count2ForBook1DuringPlayback,
            int count1ForBook2DuringPlayback)
        {
            "establish"
                .x(() =>
                    {
                        inMemoryStorage = new InMemoryStorage();

                        realServiceWhileRecording = A.Fake<ILibraryService>();
                        realServiceDuringPlayback = A.Fake<ILibraryService>();

                        A.CallTo(() => realServiceWhileRecording.GetCount("1"))
                            .ReturnsNextFromSequence(0x1A, 0x1B);
                        A.CallTo(() => realServiceWhileRecording.GetCount("2"))
                            .Returns(0x2);
                    });

            "when self initializing a fake"
                .x(() =>
                    {
                        using (var recorder = new RecordingManager(inMemoryStorage))
                        {
                            var fakeService = A.Fake<ILibraryService>(options => options
                                .Wrapping(realServiceWhileRecording).RecordedBy(recorder));

                            count1ForBook1WhileRecording = fakeService.GetCount("1");
                            fakeService.GetCount("2");
                            fakeService.GetCount("1");
                        }

                        using (var recorder = new RecordingManager(inMemoryStorage))
                        {
                            var playbackFakeService = A.Fake<ILibraryService>(options => options
                                .Wrapping(realServiceDuringPlayback).RecordedBy(recorder));

                            count1ForBook1DuringPlayback = playbackFakeService.GetCount("1");
                            count1ForBook2DuringPlayback = playbackFakeService.GetCount("2");
                            count2ForBook1DuringPlayback = playbackFakeService.GetCount("1");
                        }
                    });

            "it should forward calls to the wrapped service while recording"
                .x(() => A.CallTo(() => realServiceWhileRecording.GetCount("1"))
                             .MustHaveHappened(Repeated.Exactly.Twice));

            "it should return the result while recording"
                .x(() => count1ForBook1WhileRecording.Should().Be(0x1A));

            "it should not forward calls to the wrapped service during playback"
                .x(() => A.CallTo(realServiceDuringPlayback).MustNotHaveHappened());

            "it should return the recorded result for the first set of arguments"
                .x(() => count1ForBook1DuringPlayback.Should().Be(0x1A));

            "it should return the recorded result for the second set of arguments"
                .x(() => count1ForBook2DuringPlayback.Should().Be(0x2));

            "it should return the second recorded result when arguments are repeated"
                .x(() => count2ForBook1DuringPlayback.Should().Be(0x1B));
        }

        [Trait("explicit", "yes")]
        [Scenario]
        public static void SelfInitializingWithFileRecorder(
            string fileRecorderPath,
            ILibraryService realServiceWhileRecording,
            ILibraryService realServiceDuringPlayback,
            int countWhileRecording,
            int countDuringPlayback)
        {
            "establish"
                .x(() =>
                    {
                        fileRecorderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                        realServiceWhileRecording = A.Fake<ILibraryService>();
                        realServiceDuringPlayback = A.Fake<ILibraryService>();

                        A.CallTo(() => realServiceWhileRecording.GetCount("8")).Returns(8);
                    });

            "when self initializing a fake with a file recorder"
                .x(() =>
                    {
                        try
                        {
                            using (var recorder = Recorders.FileRecorder(fileRecorderPath))
                            {
                                var fakeService = A.Fake<ILibraryService>(options => options
                                    .Wrapping(realServiceWhileRecording).RecordedBy(recorder));
                                countWhileRecording = fakeService.GetCount("8");
                            }

                            using (var recorder = Recorders.FileRecorder(fileRecorderPath))
                            {
                                var playbackFakeService = A.Fake<ILibraryService>(options => options
                                    .Wrapping(realServiceDuringPlayback).RecordedBy(recorder));

                                countDuringPlayback = playbackFakeService.GetCount("8");
                            }
                        }
                        finally
                        {
                            File.Delete(fileRecorderPath);
                        }
                    })
                .Teardown(() => File.Delete(fileRecorderPath));

            "it should return the expected result while recording"
                .x(() => countWhileRecording.Should().Be(8));

            "it should return the recorded result during playback"
                .x(() => countDuringPlayback.Should().Be(8));
        }

        public class InMemoryStorage : ICallStorage
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
}
