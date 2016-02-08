namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests;
    using FluentAssertions;
    using Xbehave;

    public static class FixtureInitializationSpecs
    {
        [Scenario]
        public static void Initialization(ExampleFixture fixture)
        {
            "given a test fixture"
                .x(() => fixture = new ExampleFixture());

            "when the fixture is initialized"
                .x(() => Fake.InitializeFixture(fixture));

            "then the sut should be set"
                .x(() => fixture.Sut.Should().NotBeNull());

            "and dependencies should be injected into the sut from the fixture"
                .x(() => fixture.Sut.Foo.Should().BeSameAs(fixture.Foo));

            "and dependencies should be injected into the sut even when not available in fixture"
                .x(() => fixture.Sut.ServiceProvider.Should().NotBeNull());

            "and dependencies of the same type should be the same instance"
                .x(() => fixture.Sut.Foo.Should().BeSameAs(fixture.Sut.Foo2));
        }

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
