namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FluentAssertions;
    using Xbehave;

    public static class LazySpecs
    {
        public interface ILazyFactory
        {
            Lazy<IFoo> Create();

            Lazy<Bar> CreateBar();
        }

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Required for testing.")]
        public interface IFoo
        {
        }

        [Scenario]
        public static void LazyReturnValueOfDummyableType(
            ILazyFactory fake,
            Lazy<IFoo> lazy)
        {
            "Given a fake"
                .x(() => fake = A.Fake<ILazyFactory>());

            "When calling an unconfigured method that returns a lazy of a dummyable type"
                .x(() => lazy = fake.Create());

            "Then it should return a lazy"
                .x(() => lazy.Should().NotBeNull());

            "And the value of the lazy should be a dummy"
                .x(() => lazy.Value.Should().Be(FooFactory.Instance));
        }

        [Scenario]
        public static void LazyReturnValueOfNonDummyableType(
            ILazyFactory fake,
            Lazy<Bar> lazy)
        {
            "Given a fake"
                .x(() => fake = A.Fake<ILazyFactory>());

            "When calling a method that returns a lazy of a non-dummyable type"
                .x(() => lazy = fake.CreateBar());

            "Then it should return a lazy"
                .x(() => lazy.Should().NotBeNull());

            "And the value of the lazy should be null"
                .x(() => lazy.Value.Should().BeNull());
        }

        public class Foo : IFoo
        {
        }

        public class FooFactory : DummyFactory<IFoo>
        {
            public static IFoo Instance { get; } = new Foo();

            protected override IFoo Create()
            {
                return Instance;
            }
        }

        public class Bar
        {
            private Bar()
            {
            }
        }
    }
}
