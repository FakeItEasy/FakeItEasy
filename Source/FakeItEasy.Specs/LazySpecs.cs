namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FluentAssertions;
    using Machine.Specifications;

    public class when_calling_a_method_that_returns_a_lazy
    {
        static ILazyFactory fake;
        static Lazy<IFoo> lazy;
        
        Establish context = () =>
        {
            fake = A.Fake<ILazyFactory>();
        };

        Because of = () =>
        {
            lazy = fake.Create();
        };

        It should_return_a_lazy = () => lazy.Should().NotBeNull();

        It should_return_a_lazy_whose_value_is_a_dummy = () => lazy.Value.Should().Be(FooFactory.Instance);
        
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
