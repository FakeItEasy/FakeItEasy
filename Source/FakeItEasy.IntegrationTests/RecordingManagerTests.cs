using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FakeItEasy.SelfInitializedFakes;
using NUnit.Framework;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class RecordingManagerTests
    {
        [Test, Explicit]
        public void Second_pass_should_use_recorded_values_from_previous_pass()
        {
            var storage = new Storage();

            using (var recorder = new RecordingManager(storage))
            {
                var memoryStream = new MemoryStream();

                using (var stream = A.Fake<Stream>(x => x.Wrapping(memoryStream).RecordedBy(recorder)))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write("Hello world!");
                }

                Assert.That(memoryStream.GetBuffer().Length, Is.Not.LessThanOrEqualTo(0));
            }

            using (var recorder = new RecordingManager(storage))
            {
                var memoryStream = new MemoryStream();
                using (var stream = A.Fake<Stream>(x => x.Wrapping(memoryStream).RecordedBy(recorder)))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write("Hello world!");
                }

                Assert.That(memoryStream.Length, Is.EqualTo(0));
            }

            foreach (var call in storage.RecordedCalls)
            {
                Console.WriteLine(call.Method.ToString() + " returns: " + call.ReturnValue);
            }
        }

        [Test, Explicit]
        public void FileRecorder_tests()
        {
            using (var recorder = Recorders.FileRecorder(@"C:\Users\Patrik\Documents\recorded_calls.dat"))
            {
                var realReader = new WebReader();
                var fakeReader = A.Fake<WebReader>(x => x.Wrapping(realReader).RecordedBy(recorder));
                
                
                for (int i = 0; i < 30; i++)
                {
                    fakeReader.Download(new Uri("http://www.sembo.se/"));
                }

                Console.WriteLine(fakeReader.Download(new Uri("http://www.sembo.se/")));
            }
        }

        public class WebReader
        {
            public virtual string Download(Uri url)
            {
                Console.WriteLine("Downloading " + url.AbsoluteUri);
                using (var stream = new WebClient().OpenRead(url))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private class Storage
            : ICallStorage
        {
            public List<CallData> RecordedCalls;

            public IEnumerable<CallData> Load()
            {
                return this.RecordedCalls;
            }

            public void Save(IEnumerable<CallData> calls)
            {
                this.RecordedCalls = calls.ToList();
            }
        }
    }
}
