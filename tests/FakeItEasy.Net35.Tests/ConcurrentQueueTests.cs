namespace FakeItEasy
{
    extern alias FakeItEasy;

    using System.Collections;
    using System.Collections.Generic;
    using global::FakeItEasy.Tests.TestHelpers;
    using FakeItEasy::System.Collections.Concurrent;
    using FluentAssertions;
    using NUnit.Framework;

    public class ConcurrentQueueTests
    {
        [Test]
        public void Enqueue_should_add_elements_in_order()
        {
            var queue = new ConcurrentQueue<string>();

            queue.Enqueue("one");
            queue.Enqueue("two");
            queue.Enqueue("three");

            var genericEnumerable = (IEnumerable<string>)queue;

            genericEnumerable.ShouldAllBeEquivalentTo(new[] { "one", "two", "three" });
        }

        [Test]
        public void Should_return_enumerable_with_same_content_whether_generic_or_not()
        {
            var queue = new ConcurrentQueue<int>();

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            var genericEnumerable = (IEnumerable<int>)queue;
            var enumerable = (IEnumerable)queue;

            enumerable.ShouldBeEquivalentTo(genericEnumerable);
        }

        [Test]
        public void Should_not_throw_when_modified_during_enumeration()
        {
            var queue = new ConcurrentQueue<int>();

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            var enumerator = queue.GetEnumerator();

            enumerator.MoveNext();
            queue.Enqueue(4);

            var exception = Record.Exception(() => enumerator.MoveNext());

            exception.Should().BeNull();
        }
    }
}
