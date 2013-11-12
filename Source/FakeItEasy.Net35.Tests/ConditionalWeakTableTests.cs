namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public class ConditionalWeakTableTests
    {
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Required for testing.")]
        [Test]
        public void Should_release_value_when_there_are_no_more_references()
        {
            var table = new ConditionalWeakTable<TypeWithStrongReferenceThroughTable, TypeWithWeakReference>();

            var strong = new TypeWithStrongReferenceThroughTable();
            var weak = new TypeWithWeakReference()
                {
                    WeakReference = new WeakReference(strong)
                };

            table.Add(strong, weak);

            GC.Collect();

            TypeWithWeakReference result = null;
            Assert.That(table.TryGetValue(strong, out result), Is.True);
            Assert.That(result, Is.SameAs(weak));

            var weakHandleToStrong = new WeakReference(strong);

            strong = null;

            GC.Collect();

            Assert.That(weakHandleToStrong.IsAlive, Is.False);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Required for testing.")]
        [Test]
        public void Should_not_leak_memory_when_creating_delegate_fakes()
        {
            // Arrange
            var fake = new WeakReference(A.Fake<Action>());

            // Act
            GC.Collect();

            // Assert
            Assert.That(fake.Target, Is.Null);
        }

        public class TypeWithStrongReferenceThroughTable
        {
        }

        public class TypeWithWeakReference
        {
            public WeakReference WeakReference { get; set; }
        }
    }
}
