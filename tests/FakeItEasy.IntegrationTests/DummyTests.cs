namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
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

        [Fact]
        public void Short_constructor_is_always_used_when_simultaneously_creating_two_dummies_with_self_referential_constructors()
        {
            // Implemented to ensure #1639 is fixed. Essentially a very fancy version of just making two Dummies on separate threads,
            // but with a whole lot of extra synchronization built in to ensure that different phases of the creation in the two threads
            // line up to recreate the bug:
            // - task1 should start resolving long constructor before task2 starts (this is mostly to help establish which task is which)
            // - task2 should start resolving long constructor before task1 has finished the short constructor and cached it
            // - task1 should finish resolving its Dummy before task2 tries to resolve the ClassWithLongSelfReferentialConstructor argument
            //   for the long constructor
            var task1 = Task.Run(() =>
                {
                    var dummy = A.Dummy<ClassWithLongSelfReferentialConstructor>();
                    ClassWithLongSelfReferentialConstructor.Argument1OfLongConstructor.Task1IsFinishedResolvingItsDummy.Set();
                    return dummy;
                });

            var task2 = Task.Run(() =>
            {
                ClassWithLongSelfReferentialConstructor.Argument1OfLongConstructor.Task1IsResolvingLongConstructor.Wait();
                return A.Dummy<ClassWithLongSelfReferentialConstructor>();
            });

            Task.WaitAll(task1, task2);
            task1.Result.NumberOfConstructorParameters.Should().Be(1);
            task2.Result.NumberOfConstructorParameters.Should().Be(1);
        }

        public class ClassWithLongSelfReferentialConstructor
        {
            public readonly object NumberOfConstructorParameters;

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "singleArgument", Justification = "This is just a dummy argument.")]
            public ClassWithLongSelfReferentialConstructor(SingleArgument singleArgument) =>
                this.NumberOfConstructorParameters = 1;

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "firstArgument", Justification = "This is just a dummy argument.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "classWithLongSelfReferentialConstructor", Justification = "This is just a dummy argument.")]
            public ClassWithLongSelfReferentialConstructor(
                Argument1OfLongConstructor firstArgument,
                ClassWithLongSelfReferentialConstructor classWithLongSelfReferentialConstructor) =>
                this.NumberOfConstructorParameters = 2;

            public sealed class Argument1OfLongConstructor
            {
                public static readonly ManualResetEventSlim Task1IsResolvingLongConstructor = new ManualResetEventSlim(false);
                public static readonly ManualResetEventSlim Task2IsResolvingLongConstructor = new ManualResetEventSlim(false);
                public static readonly ManualResetEventSlim Task1IsFinishedResolvingItsDummy = new ManualResetEventSlim(false);
                private static bool isTask1 = true;

                public Argument1OfLongConstructor()
                {
                    if (isTask1)
                    {
                        isTask1 = false;
                        Task1IsResolvingLongConstructor.Set();
                    }
                    else
                    {
                        Task2IsResolvingLongConstructor.Set();
                        Task1IsFinishedResolvingItsDummy.Wait();
                    }
                }
            }

            public sealed class SingleArgument
            {
                private static bool isTask1 = true;

                public SingleArgument()
                {
                    if (isTask1)
                    {
                        isTask1 = false;
                        Argument1OfLongConstructor.Task2IsResolvingLongConstructor.Wait();
                    }
                }
            }
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
