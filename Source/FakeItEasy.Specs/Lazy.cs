namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FluentAssertions;
    using Xbehave;

    public class Lazy
    {
        [Scenario]
        public void LazyReturnValue(
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
        
        public interface ILazyFactory
        {
            Lazy<IFoo> Create();
        }

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Required for testing.")]
        public interface IFoo
        {
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
    }
}
