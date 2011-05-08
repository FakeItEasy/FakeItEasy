namespace FakeItEasy.IntegrationTests
{
    using System;
    using NUnit.Framework;
    using Tests;

    [TestFixture]
    public class FixtureInitializationTests
    {
        [Fake] public IFoo Foo;
        [UnderTest] public SutExample Sut;

        [SetUp]
        public void SetUp()
        {
            Fake.InitializeFixture(this);
        }

        [Test]
        public void Should_set_sut()
        {
            Assert.That(this.Sut, Is.Not.Null);
        }

        [Test]
        public void Should_inject_with_same_fake_as_fixture_is_initialized_with()
        {
            Assert.That(this.Foo, Is.SameAs(this.Sut.Foo));
        }

        [Test]
        public void Should_use_same_instance_when_more_than_one_dependency_is_of_same_type()
        {
            Assert.That(this.Sut.Foo, Is.SameAs(this.Sut.Foo2));
        }

        [Test]
        public void Should_inject_fake_even_though_such_fake_is_not_available_in_fixture()
        {
            Assert.That(Fake.GetFakeManager(this.Sut.ServiceProvider), Is.Not.Null);
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