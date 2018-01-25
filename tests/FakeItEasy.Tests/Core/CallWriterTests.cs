namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class CallWriterTests
    {
        private readonly List<IFakeObjectCall> calls;
        private readonly StringBuilderOutputWriter writer;
        private readonly IEqualityComparer<IFakeObjectCall> callComparer;
        private readonly IFakeObjectCallFormatter callFormatter;

        public CallWriterTests()
        {
            this.callComparer = A.Fake<IEqualityComparer<IFakeObjectCall>>();
            this.callFormatter = A.Fake<IFakeObjectCallFormatter>();

            A.CallTo(() => this.callFormatter.GetDescription(A<IFakeObjectCall>._))
                .Returns("Default call description");

            this.calls = new List<IFakeObjectCall>();
            this.writer = ServiceLocator.Current.Resolve<StringBuilderOutputWriter>();
        }

        [Fact]
        public void WriteCalls_should_list_the_calls_in_the_calls_collection()
        {
            this.StubCalls(10);

            int callNumber = 1;
            foreach (var call in this.calls)
            {
                var boundCallNumber = callNumber;
                A.CallTo(() => this.callFormatter.GetDescription(call)).Returns("Fake call " + boundCallNumber);
                callNumber++;
            }

            var callWriter = this.CreateWriter();
            callWriter.WriteCalls(this.calls, this.writer);

            var message = this.writer.Builder.ToString();
            var expectedMessage =
@"1:  Fake call 1
2:  Fake call 2
3:  Fake call 3
4:  Fake call 4
5:  Fake call 5
6:  Fake call 6
7:  Fake call 7
8:  Fake call 8
9:  Fake call 9
10: Fake call 10";

            message.Should().Contain(expectedMessage);
        }

        [Fact]
        public void WriteCalls_should_skip_duplicate_calls_in_row()
        {
            // Arrange
            this.StubCalls(10);

            A.CallTo(() => this.callFormatter.GetDescription(A<IFakeObjectCall>._)).Returns("Fake call");
            A.CallTo(() => this.callFormatter.GetDescription(this.calls[9])).Returns("Other call");

            A.CallTo(() => this.callComparer.Equals(A<IFakeObjectCall>.That.Not.IsEqualTo(this.calls[9]), A<IFakeObjectCall>.That.Not.IsEqualTo(this.calls[9]))).Returns(true);

            var callWriter = this.CreateWriter();

            // Act
            callWriter.WriteCalls(this.calls, this.writer);

            // Assert
            var message = this.writer.Builder.ToString();
            var expectedMessage =
@"1:  Fake call 9 times
...
10: Other call";

            message.Should().Contain(expectedMessage);
        }

        [Fact]
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

            var callWriter = this.CreateWriter();
            callWriter.WriteCalls(this.calls, this.writer);

            var message = this.writer.Builder.ToString();
            var expectedMessage =
@"1: odd
2: even
3: odd
4: even";

            message.Should().Contain(expectedMessage);
        }

        [Fact]
        public void WriteCalls_should_truncate_calls_list_when_more_than_a_hundred_call_lines_are_printed()
        {
            this.StubCalls(30);

            foreach (var call in this.calls)
            {
                A.CallTo(() => this.callFormatter.GetDescription(call)).Returns("Fake call " + Guid.NewGuid());
            }

            A.CallTo(() => this.callFormatter.GetDescription(this.calls[18])).Returns("Last call");

            var callWriter = this.CreateWriter();
            callWriter.WriteCalls(this.calls, this.writer);

            var message = this.writer.Builder.ToString();
            var expectedMessage =
@"19: Last call
... Found 11 more calls not displayed here.";

            message.Should().Contain(expectedMessage);
        }

        [Fact]
        public void WriteCalls_should_indent_values_with_new_lines_correctly()
        {
            // Arrange
            this.StubCalls(10);

            var text =
@"first line
second line";

            var callIndex = 0;
            A.CallTo(() => this.callFormatter.GetDescription(A<IFakeObjectCall>._)).ReturnsLazily(() => text + ++callIndex);

            var callWriter = this.CreateWriter();

            // Act
            using (this.writer.Indent())
            {
                callWriter.WriteCalls(this.calls, this.writer);
            }

            // Assert
            var message = this.writer.Builder.ToString();

            var expectedText1 =
@"1:  first line
    second line";

            var expectedText2 =
@"10: first line
    second line";

            message.Should().Contain(expectedText1).And.Contain(expectedText2);
        }

        [Fact]
        public void WriteCalls_should_write_new_line_at_end()
        {
            // Arrange
            this.StubCalls(1);

            var callWriter = this.CreateWriter();

            // Act
            callWriter.WriteCalls(this.calls, this.writer);

            // Assert
            this.writer.Builder.ToString().Should().EndWith(Environment.NewLine);
        }

        [Fact]
        public void Should_write_nothing_if_call_list_is_empty()
        {
            // Arrange
            var callWriter = this.CreateWriter();

            // Act
            callWriter.WriteCalls(Enumerable.Empty<IFakeObjectCall>(), this.writer);

            // Assert
            this.writer.Builder.ToString().Should().BeEmpty();
        }

        private CallWriter CreateWriter()
        {
            return new CallWriter(this.callFormatter, this.callComparer);
        }

        private void StubCalls(int numberOfCalls)
        {
            for (int i = 0; i < numberOfCalls; i++)
            {
                this.calls.Add(A.Fake<IFakeObjectCall>());
            }
        }
    }
}
