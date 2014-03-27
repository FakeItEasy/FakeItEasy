namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Tests;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_initializing_fixture
    {
        Establish context = () => Fixture = new ExampleFixture();

        Because of = () => Fake.InitializeFixture(Fixture);

        It should_set_sut = () =>
            Fixture.Sut.Should().NotBeNull();

        It should_use_the_same_instance_when_more_than_one_dependency_is_of_the_same_type = () =>
            Fixture.Sut.Foo.Should().BeSameAs(Fixture.Sut.Foo2);

        It should_inject_fake_from_fixture = () =>
            Fixture.Sut.Foo.Should().BeSameAs(Fixture.Foo);

        It should_inject_fake_when_not_available_in_fixture = () =>
            Fixture.Sut.ServiceProvider.Should().NotBeNull();

        public static ExampleFixture Fixture { get; set; }

        public class ExampleFixture
        {
            [Fake]
            public IFoo Foo { get; set; }

            [UnderTest]
            public SutExample Sut { get; set; }
        }

        public class SutExample
        {
            public SutExample(IFoo foo, IServiceProvider serviceProvider, IFoo foo2)
            {
                this.Foo = foo;
                this.ServiceProvider = serviceProvider;
                this.Foo2 = foo2;
            }

            public IFoo Foo { get; set; }

            public IServiceProvider ServiceProvider { get; set; }

            public IFoo Foo2 { get; set; }
        }
    }
}
