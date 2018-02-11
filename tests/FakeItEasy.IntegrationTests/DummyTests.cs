namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Required for testing.")]
    public interface ITypeWithNoDummyFactory
    {
    }

    public class DummyTests
    {
        [Fact]
        public void Proxy_should_be_returned_for_dummy_interface_when_no_dummy_factory_is_registered()
        {
            var dummy = A.Dummy<ITypeWithNoDummyFactory>();
            dummy.Should().BeAFake();
        }

        [Fact]
        public void Correct_exception_should_be_thrown_when_dummy_is_requested_for_unfakeable_type_with_no_dummy_factory()
        {
            var exception = Record.Exception(() => A.Dummy<NonInstance>());
            exception.Should().BeAnExceptionOfType<FakeCreationException>();
        }

        [Fact]
        public void Correct_exception_should_be_thrown_when_dummy_collection_is_requested_for_unfakeable_type_with_no_dummy_factory()
        {
            var exception = Record.Exception(() => A.CollectionOfDummy<NonInstance>(1));
            exception.Should().BeAnExceptionOfType<FakeCreationException>();
        }

        [Fact]
        public void Dummy_factories_are_polled_only_once_per_dummy_type()
        {
            A.Dummy<Dummy>();
            A.Dummy<Dummy>();

            DummyTestsDummyFactory.CanCreateDummyCallCount.Should()
                .Be(1, "the factory should've been cached after creating the first Dummy");
        }

        [Fact]
        public void Same_dummy_type_can_be_created_concurrently_on_different_threads()
        {
            // Simulate concurrent dummy type creation using manual reset events.
            // Ensures that one ConcurrentCreationDummy dummy is created while
            // the other is still being created.
            // Guards against the concurrent creations being detected as a
            // circular dependency and throwing an exception.
            var makeOne = Task.Run(() => A.Dummy<ConcurrentCreationDummy>());
            var makeTwo = Task.Run(() =>
            {
                ConcurrentCreationDummy.CreatingFirstInstance.Wait();
                try
                {
                    A.Dummy<ConcurrentCreationDummy>();
                }
                finally
                {
                    ConcurrentCreationDummy.CreatedSecondInstance.Set();
                }
            });

            var exception = Record.Exception(() => Task.WaitAll(makeOne, makeTwo));
            exception.Should().BeNull("all creations should have succeeded");
        }

        public class Dummy
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Must not be static to make a Dummy.")]
        public sealed class ConcurrentCreationDummy
        {
            private static int creationAttempts;

            public ConcurrentCreationDummy()
            {
                if (IsCreatingFirstInstance())
                {
                    CreatingFirstInstance.Set();
                    CreatedSecondInstance.Wait();
                }
            }

            public static ManualResetEventSlim CreatingFirstInstance { get; } = new ManualResetEventSlim();

            public static ManualResetEventSlim CreatedSecondInstance { get; } = new ManualResetEventSlim();

            private static bool IsCreatingFirstInstance()
            {
                return Interlocked.Increment(ref creationAttempts) == 1;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Required for testing.")]
        private class NonInstance
        {
            private NonInstance()
            {
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Tidier.")]
    public class DummyTestsDummyFactory : IDummyFactory
    {
        public Priority Priority => Priority.Default;

        internal static int CanCreateDummyCallCount { get; private set; }

        public bool CanCreate(Type type)
        {
            if (type != typeof(DummyTests.Dummy))
            {
                return false;
            }

            ++CanCreateDummyCallCount;
            return true;
        }

        public object Create(Type type)
        {
            return new DummyTests.Dummy();
        }
    }

    public sealed class UnfakeableTypeWithNoDummyFactory
    {
    }
}
