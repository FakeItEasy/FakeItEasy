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
        public static void LazyReturnValue(
            ILazyFactory fake,
            Lazy<IFoo> lazy)
        {
            "establish"
                .x(() => fake = A.Fake<ILazyFactory>());

            "when calling a method that returns a lazy"
                .x(() => lazy = fake.Create());

            "it should return a lazy"
                .x(() => lazy.Should().NotBeNull());

            "it should return a lazy whose value is a dummy"
                .x(() => lazy.Value.Should().Be(FooFactory.Instance));
        }

        [Scenario]
        public static void LazyReturnValueOfNonFakeableType(
            ILazyFactory fake,
            Lazy<Bar> lazy)
        {
            "Given a fake"
                .x(() => fake = A.Fake<ILazyFactory>());

            "When calling a method that returns a lazy"
                .x(() => lazy = fake.CreateBar());

            "Then it should return a lazy"
                .x(() => lazy.Should().NotBeNull());

            "And the value of the lazy should be null"
                .x(() => lazy.Value.Should().BeNull());
        }

        public class FooFactory : DummyFactory<IFoo>, IFoo
        {
            private static IFoo instance = new FooFactory();

            public static IFoo Instance
            {
                get { return instance; }
            }

            protected override IFoo Create()
            {
                return instance;
            }
        }

        public class Bar
        {
            internal Bar()
            {
            }
        }
    }
}
