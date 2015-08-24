namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests;
    using FluentAssertions;
    using Xbehave;

    public class FixtureInitialization
    {
        [Scenario]
        public void when_initializing_fixture()
        {
            "establish"._(() =>
            {
                Fixture = new ExampleFixture();
            });

            "when initializing fixture"._(() =>
            {
                Fake.InitializeFixture(Fixture);
            });

            "it should set sut"._(() =>
            {
                Fixture.Sut.Should().NotBeNull();
            });

            "it should use the same instance when more than one dependency is of the same type"._(() =>
            {
                Fixture.Sut.Foo.Should().BeSameAs(Fixture.Sut.Foo2);
            });

            "it should inject fake from fixture"._(() =>
            {
                Fixture.Sut.Foo.Should().BeSameAs(Fixture.Foo);
            });

            "it should inject fake when not available in fixture"._(() =>
            {
                Fixture.Sut.ServiceProvider.Should().NotBeNull();
            });
        }

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
