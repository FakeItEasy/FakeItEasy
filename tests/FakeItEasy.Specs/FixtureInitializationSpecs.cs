namespace FakeItEasy.Specs
{
    using System;
    using System.Reflection;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

#pragma warning disable CS0618 // Type or member is obsolete
    public static class FixtureInitializationSpecs
    {
        [Scenario]
        public static void Initialization(ExampleFixture fixture)
        {
            "Given a test fixture"
                .x(() => fixture = new ExampleFixture());

            "When I initialize the fixture"
                .x(() => Fake.InitializeFixture(fixture));

            "Then the sut is set"
                .x(() => fixture.Sut.Should().NotBeNull());

            "And dependencies are injected into the sut from the fixture"
                .x(() => fixture.Sut.Foo.Should().BeSameAs(fixture.Foo));

            "And dependencies are injected into the sut even when not available in fixture"
                .x(() => fixture.Sut.ServiceProvider.Should().NotBeNull());

            "And dependencies of the same type are the same instance"
                .x(() => fixture.Sut.Foo.Should().BeSameAs(fixture.Sut.Foo2));

            "And public attributed fixture fields are set"
                .x(() => fixture.FooField.Should().NotBeNull());

            "And public unattributed fixture fields are not set"
                .x(() => fixture.UnattributedFooField.Should().BeNull());

            "And private attributed fixture properties are set"
                .x(() => typeof(ExampleFixture).GetProperty("PrivateFoo", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fixture).Should().NotBeNull());

            "And private attributed fixture fields are set"
                .x(() => typeof(ExampleFixture).GetField("privateFoo", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fixture).Should().NotBeNull());

            "And public unattributed fixture properties are not set"
                .x(() => fixture.UnattributedFoo.Should().BeNull());
        }

        public static void TwoSuts(TwoSutFixture fixture, Exception exception)
        {
            "Given a test fixture"
                .x(() => fixture = new TwoSutFixture());

            "And the fixture has two SUTs"
                .See<TwoSutFixture>();

            "When I initialize the fixture"
                .x(() => exception = Record.Exception(() => Fake.InitializeFixture(fixture)));

            "Then it throws an exception"
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
#pragma warning restore CS0618 // Type or member is obsolete
}
