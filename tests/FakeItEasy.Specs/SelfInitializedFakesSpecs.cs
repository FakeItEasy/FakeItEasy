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
            IEnumerable<int> countsWhileRecording,
            IEnumerable<int> countsDuringPlayback)
        {
            "Given a call storage object"
                .x(() => inMemoryStorage = new InMemoryStorage());

            "And a real service to wrap while recording"
                .x(() =>
                {
                    realServiceWhileRecording = A.Fake<ILibraryService>();

                    A.CallTo(() => realServiceWhileRecording.GetCount("1"))
                        .ReturnsNextFromSequence(0x1A, 0x1B);
                    A.CallTo(() => realServiceWhileRecording.GetCount("2"))
                        .Returns(0x2);
                });

            "And a real service to wrap while playing back"
                .x(() => realServiceDuringPlayback = A.Fake<ILibraryService>());

            "When I use a self-initialized fake in recording mode to get the counts for book 1, 2, and 1 again"
                .x(() =>
                {
                    using (var recorder = new RecordingManager(inMemoryStorage))
                    {
                        var fakeService = A.Fake<ILibraryService>(options => options
                            .Wrapping(realServiceWhileRecording).RecordedBy(recorder));

                        countsWhileRecording = new List<int>
                        {
                            fakeService.GetCount("1"),
                            fakeService.GetCount("2"),
                            fakeService.GetCount("1")
                        };
                    }
                });

            "And I use a self-initialized fake in playback mode to get the counts for book 1, 2, and 1 again"
                .x(() =>
                {
                    using (var recorder = new RecordingManager(inMemoryStorage))
                    {
                        var playbackFakeService = A.Fake<ILibraryService>(options => options
                            .Wrapping(realServiceDuringPlayback).RecordedBy(recorder));

                        countsDuringPlayback = new List<int>
                        {
                            playbackFakeService.GetCount("1"),
                            playbackFakeService.GetCount("2"),
                            playbackFakeService.GetCount("1")
                        };
                    }
                });

            "Then the recording fake forwards calls to the wrapped service"
                .x(() => A.CallTo(() => realServiceWhileRecording.GetCount("1"))
                    .MustHaveHappened(Repeated.Exactly.Twice));

            "And the recording fake returns the wrapped service's results"
                .x(() => countsWhileRecording.Should().Equal(0x1A, 0x2, 0x1B));

            "And the playback fake does not forward calls to the wrapped service"
                .x(() => A.CallTo(realServiceDuringPlayback).MustNotHaveHappened());

            "And the playback fake returns the recorded results"
                .x(() => countsDuringPlayback.Should().Equal(0x1A, 0x2, 0x1B));
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
            "Given a path that does not exist"
                .x(() => fileRecorderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

            "And a real service to wrap while recording"
                .x(() =>
                {
                    realServiceWhileRecording = A.Fake<ILibraryService>();

                    A.CallTo(() => realServiceWhileRecording.GetCount("8"))
                        .Returns(0x8);
                });

            "And a real service to wrap while playing back"
                .x(() => realServiceDuringPlayback = A.Fake<ILibraryService>());

            "When I use a self-initialized fake recording to the path to get the count for book 8"
                .x(() =>
                {
                    using (var recorder = Recorders.FileRecorder(fileRecorderPath))
                    {
                        var fakeService = A.Fake<ILibraryService>(options => options
                            .Wrapping(realServiceWhileRecording).RecordedBy(recorder));
                        countWhileRecording = fakeService.GetCount("8");
                    }
                })
                .Teardown(() => File.Delete(fileRecorderPath));

            "And I use a self-initialized fake playing back from the path to get the count for book 8"
                .x(() =>
                {
                    using (var recorder = Recorders.FileRecorder(fileRecorderPath))
                    {
                        var playbackFakeService = A.Fake<ILibraryService>(options => options
                            .Wrapping(realServiceDuringPlayback).RecordedBy(recorder));

                        countDuringPlayback = playbackFakeService.GetCount("8");
                    }
                });

            "Then the recording fake returns the wrapped service's result"
                .x(() => countWhileRecording.Should().Be(8));

            "And the playback fake returns the recorded result"
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
