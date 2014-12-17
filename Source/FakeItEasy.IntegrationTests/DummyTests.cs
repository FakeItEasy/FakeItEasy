namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FakeItEasy.Tests;
    using NUnit.Framework;

    [TestFixture]
    public class DummyTests
    {
        [Test]
        public void Type_registered_in_container_should_be_returned_when_a_dummy_is_requested()
        {
            var container = new DelegateFakeObjectContainer();
            container.Register<string>(() => "dummy");

            using (Fake.CreateScope(container))
            {
                Assert.That(A.Dummy<string>(), Is.EqualTo("dummy"));
            }
        }

        [Test]
        public void Proxy_should_be_returned_when_nothing_is_registered_in_the_container_for_the_type()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                Assert.That(A.Dummy<IFoo>(), new IsFakeConstraint());
            }
        }

        [Test]
        public void Correct_exception_should_be_thrown_when_dummy_is_requested_for_non_fakeable_type_not_in_container()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                Assert.Throws<FakeCreationException>(() => A.Dummy<NonInstance>());
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CanCreateDummyOfType",
            Justification = "That's really the name of the method.")]
        [Test]
        public void Definition_for_type_should_be_cached_after_first_use()
        {
            var firstDummy = A.Dummy<Dummy>();

            var secondDummy = A.Dummy<Dummy>();

            Assert.That(secondDummy.Count, Is.EqualTo(firstDummy.Count), "CanCreateDummyOfType was called the wrong number of times.");
        }

        public class Dummy
        {
            private readonly int count;

            public Dummy(int count)
            {
                this.count = count;
            }

            public int Count
            {
                get { return this.count; }
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
    public class DummyTestsDummyDefinition : IDummyDefinition
    {
        private static int timesCanCreateDummyOfTypeWasCalled = 0;

        public int Priority
        {
            get { return 0; }
        }

        public bool CanCreateDummyOfType(Type type)
        {
            ++timesCanCreateDummyOfTypeWasCalled;
            return type == typeof(DummyTests.Dummy);
        }

        public object CreateDummyOfType(Type type)
        {
            return new DummyTests.Dummy(timesCanCreateDummyOfTypeWasCalled);
        }
    }
}
