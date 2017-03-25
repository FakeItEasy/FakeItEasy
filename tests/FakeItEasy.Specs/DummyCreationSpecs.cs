namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

    public abstract class DummyCreationSpecsBase
    {
        [Scenario]
        public void InterfaceCreation(
            IDisposable dummy)
        {
            "Given a fakeable interface type"
                .See<IDisposable>();

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<IDisposable>());

            "Then it returns a fake of that type"
                .x(() => dummy.Should().BeAFake());
        }

        [Scenario]
        public void AbstractClassCreation(
            TextReader dummy)
        {
            "Given a fakeable abstract class type"
                .See<TextReader>();

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<TextReader>());

            "Then it returns a fake of that type"
                .x(() => dummy.Should().BeAFake());
        }

        [Scenario]
        public void ValueTypeCreation(
            DateTime dummy)
        {
            "Given a value type"
                .See<DateTime>();

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<DateTime>());

            "Then it returns the default value for that type"
                .x(() => dummy.Should().Be(default(DateTime)));
        }

        [Scenario]
        public void TypeWithDummyFactoryCreation(
            Foo dummy)
        {
            "Given a type"
                .See<Foo>();

            "And a dummy factory for that type"
                .See<FooDummyFactory>();

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<Foo>());

            "Then it returns a dummy created by the dummy factory"
                .x(() => dummy.Bar.Should().Be(42));
        }

        [Scenario]
        public void CollectionOfDummyCreation(
            IList<DateTime> dummies)
        {
            "Given a type"
                .See<DateTime>();

            "When a collection of that type is requested"
                .x(() => dummies = this.CreateCollectionOfDummy<DateTime>(10));

            "Then it returns a collection with the specified number of dummies"
                .x(() => dummies.Should().HaveCount(10));
        }

        protected abstract T CreateDummy<T>();

        protected abstract IList<T> CreateCollectionOfDummy<T>(int count);

        public class Foo
        {
            public int Bar { get; set; }
        }

        public class FooDummyFactory : DummyFactory<Foo>
        {
            protected override Foo Create()
            {
                return new Foo { Bar = 42 };
            }
        }
    }

    public class GenericDummyCreationSpecs : DummyCreationSpecsBase
    {
        protected override T CreateDummy<T>()
        {
            return A.Dummy<T>();
        }

        protected override IList<T> CreateCollectionOfDummy<T>(int count)
        {
            return A.CollectionOfDummy<T>(count);
        }
    }

    public class NonGenericDummyCreationSpecs : DummyCreationSpecsBase
    {
        protected override T CreateDummy<T>()
        {
            return (T)Sdk.Create.Dummy(typeof(T));
        }

        protected override IList<T> CreateCollectionOfDummy<T>(int count)
        {
            return Sdk.Create.CollectionOfDummy(typeof(T), count).Cast<T>().ToList();
        }
    }
}
