namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class DummyTests
    {
        [Test]
        public void Type_registered_in_container_should_be_returned_when_a_dummy_is_requested()
        {
            var container = new DelegateFakeObjectContainer();
            container.Register(() => "dummy");

            string dummyString;
            using (Fake.CreateScope(container))
            {
                dummyString = A.Dummy<string>();
            }

            dummyString.Should().Be("dummy");
        }

        [Test]
        public void Proxy_should_be_returned_when_nothing_is_registered_in_the_container_for_the_type()
        {
            IFoo dummyFoo;
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                dummyFoo = A.Dummy<IFoo>();
            }

            dummyFoo.Should().BeAFake();
        }

        [Test]
        public void Correct_exception_should_be_thrown_when_dummy_is_requested_for_non_fakeable_type_not_in_container()
        {
            Exception exception;
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                exception = Record.Exception(() => A.Dummy<NonInstance>());
            }

            exception.Should().BeAnExceptionOfType<FakeCreationException>();
        }

        [Test]
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
        public int Priority
        {
            get { return 0; }
        }

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
}
