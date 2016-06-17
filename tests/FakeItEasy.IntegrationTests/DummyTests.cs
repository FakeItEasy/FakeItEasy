namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
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

        public class Dummy
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Required for testing.")]
        private class NonInstance
        {
            private NonInstance()
            {
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Tidier.")]
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
