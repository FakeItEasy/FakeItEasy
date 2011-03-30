namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class CallWriterTests
    {
        private List<IFakeObjectCall> calls;
        private StringWriter writer;
        
        [Fake] internal IFakeObjectCallFormatter callFormatter;
        [Fake] public IEqualityComparer<IFakeObjectCall> CallComparer;

        [SetUp]
        public void SetUp()
        {
            Fake.InitializeFixture(this);

            A.CallTo(() => this.callFormatter.GetDescription(A<IFakeObjectCall>.Ignored))
                .Returns("Default call description");

            this.calls = new List<IFakeObjectCall>();
            this.writer = new StringWriter();
        }

        private CallWriter CreateWriter()
        {
            return new CallWriter(this.callFormatter, this.CallComparer);
        }

        private void StubCalls(int numberOfCalls)
        {
            for (int i = 0; i < numberOfCalls; i++)
            {
                this.calls.Add(A.Fake<IFakeObjectCall>());
            }
        }

        [Test]
        public void WriteCalls_should_list_the_calls_in_the_calls_collection()
        {
            this.StubCalls(10);

            int callNumber = 1;
            foreach (var call in this.calls)
            {
                var boundCallNumber = callNumber;
                A.CallTo(() => this.callFormatter.GetDescription(call)).Returns("Fake call " + boundCallNumber.ToString());
                callNumber++;
            }

            var writer = this.CreateWriter();
            writer.WriteCalls(0, this.calls, this.writer);

            var message = this.writer.GetStringBuilder().ToString();
            
            Assert.That(message,  Is.StringContaining(@"1:  Fake call 1
2:  Fake call 2
3:  Fake call 3
4:  Fake call 4
5:  Fake call 5
6:  Fake call 6
7:  Fake call 7
8:  Fake call 8
9:  Fake call 9
10: Fake call 10"));
        }

        [Test]
        public void WriteCalls_should_skip_duplicate_calls_in_row()
        {
            // Arrange
            this.StubCalls(10);

            A.CallTo(() => this.callFormatter.GetDescription(A<IFakeObjectCall>.Ignored)).Returns("Fake call");
            A.CallTo(() => this.callFormatter.GetDescription(this.calls[9])).Returns("Other call");

            A.CallTo(() => this.CallComparer.Equals(A<IFakeObjectCall>.That.Not.IsEqualTo(this.calls[9]), A<IFakeObjectCall>.That.Not.IsEqualTo(this.calls[9]))).Returns(true);
            
            var writer = this.CreateWriter();
            
            // Act
            writer.WriteCalls(0, this.calls, this.writer);

            // Assert
            var message = this.writer.GetStringBuilder().ToString();
            Assert.That(message, Is.StringContaining(@"1:  Fake call repeated 9 times
...
10: Other call"));
        }

        [Test]
        public void WriteCalls_should_not_skip_duplicate_messages_that_are_not_in_row()
        {
            this.StubCalls(4);

            foreach (var call in this.calls.Where((x, i) => i % 2 == 0))
            {
                A.CallTo(() => this.callFormatter.GetDescription(call)).Returns("odd");
            }

            foreach (var call in this.calls.Where((x, i) => i % 2 != 0))
            {
                A.CallTo(() => this.callFormatter.GetDescription(call)).Returns("even");
            }

            var writer = this.CreateWriter();
            writer.WriteCalls(0, this.calls, this.writer);
            
            var message =this.writer.GetStringBuilder().ToString();

            Assert.That(message, Is.StringContaining(@"1: odd
2: even
3: odd
4: even"));
        }

        [Test]
        public void WriteCalls_should_truncate_calls_list_when_more_than_a_hundred_call_lines_are_printed()
        {
            this.StubCalls(30);

            foreach (var call in this.calls)
            {
                A.CallTo(() => this.callFormatter.GetDescription(call)).Returns(string.Format(CultureInfo.InvariantCulture, "Fake call {0}", Guid.NewGuid()));
            }

            A.CallTo(() => this.callFormatter.GetDescription(this.calls[18])).Returns("Last call");

            var writer = this.CreateWriter();
            writer.WriteCalls(0, this.calls, this.writer);

            var message = this.writer.GetStringBuilder().ToString();

            Assert.That(message, Is.StringContaining(@"19: Last call
... Found 11 more calls not displayed here."));
        }

        [Test]
        public void WriteCalls_should_indent_calls_by_specified_number_of_characters()
        {
            this.StubCalls(10);

            int callNumber = 1;
            foreach (var call in this.calls)
            {
                A.CallTo(() => this.callFormatter.GetDescription(call)).Returns("Fake call " + callNumber.ToString());
                callNumber++;
            }

            var writer = this.CreateWriter();
            writer.WriteCalls(4, this.calls, this.writer);

            var message = this.writer.GetStringBuilder().ToString();

            Assert.That(message, Is.StringContaining(@"    1:  Fake call 1
    2:  Fake call 2
    3:  Fake call 3
    4:  Fake call 4
    5:  Fake call 5
    6:  Fake call 6
    7:  Fake call 7
    8:  Fake call 8
    9:  Fake call 9
    10: Fake call 10"));
        }

        [Test]
        public void WriteCalls_should_indent_values_with_newlines_correctly()
        {
            // Arrange
            this.StubCalls(10);

            var callIndex = 0;
            A.CallTo(() => this.callFormatter.GetDescription(A<IFakeObjectCall>.Ignored)).ReturnsLazily(() =>
@"first line
second line" + ++callIndex);
            
            var writer = this.CreateWriter();

            // Act
            
            writer.WriteCalls(0, this.calls, this.writer);

            // Assert
            var message = this.writer.GetStringBuilder().ToString();

            Assert.That(message, Is.StringContaining(@"1:  first line
    second line").And.StringContaining(@"10: first line
    second line"));
        }

        [Test]
        public void WriteCalls_should_write_new_line_at_end()
        {
            // Arrange
            this.StubCalls(1);

            var writer = this.CreateWriter();

            // Act
            writer.WriteCalls(0, this.calls, this.writer);
            
            // Assert
            Assert.That(this.writer.GetStringBuilder().ToString(), Is.StringEnding(Environment.NewLine));
        }

        [Test]
        public void Should_write_nothing_if_call_list_is_empty()
        {
            // Arrange
            var writer = this.CreateWriter();

            // Act
            writer.WriteCalls(0, Enumerable.Empty<IFakeObjectCall>(), this.writer);

            // Assert
            Assert.That(this.writer.GetStringBuilder().ToString(), Is.Empty);
        }
    }
}
