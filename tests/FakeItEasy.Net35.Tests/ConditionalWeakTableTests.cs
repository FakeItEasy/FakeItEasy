namespace FakeItEasy
{
    extern alias FakeItEasy;

    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy::System.Runtime.CompilerServices;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class ConditionalWeakTableTests
    {
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Required for testing.")]
        [Test]
        public void Should_retain_value_when_references_exist()
        {
            // Arrange
            var table = new ConditionalWeakTable<TypeWithStrongReferenceThroughTable, TypeWithWeakReference>();

            var strong = new TypeWithStrongReferenceThroughTable();
            var weak = new TypeWithWeakReference()
                {
                    WeakReference = new WeakReference(strong)
                };

            table.Add(strong, weak);

            // Act
            GC.Collect();

            // Assert
            TypeWithWeakReference result = null;
            table.TryGetValue(strong, out result).Should().BeTrue();
            result.Should().BeSameAs(weak);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Required for testing.")]
        [Test]
        public void Should_release_value_when_there_are_no_more_references()
        {
            // Arrange
            var table = new ConditionalWeakTable<TypeWithStrongReferenceThroughTable, TypeWithWeakReference>();

            var strong = new TypeWithStrongReferenceThroughTable();
            var weak = new TypeWithWeakReference()
                {
                    WeakReference = new WeakReference(strong)
                };

            table.Add(strong, weak);

            var weakHandleToStrong = new WeakReference(strong);

            // Act
            strong = null;
            GC.Collect();

            // Assert
            weakHandleToStrong.IsAlive.Should().BeFalse();
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
            fake.Target.Should().BeNull();
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
