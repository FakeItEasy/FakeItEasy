namespace FakeItEasy.Specs
{
    using System;
    using System.Reflection;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

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

            "and public attributed fixture fields should be set"
                .x(() => fixture.FooField.Should().NotBeNull());

            "and public unattributed fixture fields should not be set"
                .x(() => fixture.UnattributedFooField.Should().BeNull());

            "and private attributed fixture properties should be set"
                .x(() => typeof(ExampleFixture).GetProperty("PrivateFoo", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fixture).Should().NotBeNull());

            "and private attributed fixture fields should be set"
                .x(() => typeof(ExampleFixture).GetField("privateFoo", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fixture).Should().NotBeNull());

            "and public unattributed fixture properties should not be set"
                .x(() => fixture.UnattributedFoo.Should().BeNull());
        }

        public static void TwoSuts(TwoSutFixture fixture, Exception exception)
        {
            "given a test fixture"
                .x(() => fixture = new TwoSutFixture());

            "and the fixture has two SUTs"
                .See<TwoSutFixture>();

            "when the fixture is initialized"
                .x(() => exception = Record.Exception(() => Fake.InitializeFixture(fixture)));

            "then it throws an exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>()
                .WithMessage($"A fake fixture can only contain one member marked with {nameof(UnderTestAttribute)}."));
        }

        public class TwoSutFixture
        {
            [UnderTest]
            public SutExample Sut1 { get; set; }

            [UnderTest]
            public SutExample Sut2 { get; set; }
        }

        public class ExampleFixture
        {
            [Fake]
            public IFoo Foo { get; set; }

            public IFoo UnattributedFoo { get; set; }

            [UnderTest]
            public SutExample Sut { get; set; }

            [Fake]
            private IFoo PrivateFoo { get; set; }

            [Fake]
            public IFoo FooField;
            public IFoo UnattributedFooField;

            [Fake]
#pragma warning disable CS0169 // it's used by reflection
            private IFoo privateFoo;
#pragma warning restore CS0169
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
