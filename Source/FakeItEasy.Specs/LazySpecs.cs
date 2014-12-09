namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_calling_an_unconfigured_method_that_returns_a_lazy_of_ifoo
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

        It should_return_a_lazy_whose_value_is_an_ifoo_dummy = () => lazy.Value.Should().Be(FooDefinition.Instance);
        
        public interface ILazyFactory
        {
            Lazy<IFoo> Create();
        }

        public interface IFoo
        {
        }

        public class FooDefinition : DummyDefinition<IFoo>, IFoo
        {
            public static readonly IFoo Instance = new FooDefinition();

            protected override IFoo CreateDummy()
            {
                return Instance;
            }
        }
    }
}
