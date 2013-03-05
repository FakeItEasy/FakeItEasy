namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultFixtureInitializerTests
    {
        [UnderTest]
        private DefaultFixtureInitializer initializer;

        [Fake]
        private IFakeAndDummyManager fakeAndDummyManager;

        [Fake]
        private IFoo fakeReturnedFromFakeAndDummyManager;

        [Fake]
        private ISutInitializer sutInitializer;

        private FixtureType fixture;

        [SetUp]
        public void SetUp()
        {
            this.OnSetUp();
        }

        [Test]
        public void Should_set_public_property_that_is_attributed()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(this.fixture.FooProperty, Is.SameAs(this.fakeReturnedFromFakeAndDummyManager));
        }

        [Test]
        public void Should_not_set_untagged_property()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(this.fixture.NonFakedProperty, Is.Null);
        }

        [Test]
        public void Should_set_private_property_that_is_tagged()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(this.fixture.GetValueOfPrivateFakeProperty(), Is.SameAs(this.fakeReturnedFromFakeAndDummyManager));
        }

        [Test]
        public void Should_set_private_field_that_is_tagged()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(this.fixture.GetValueOfPrivateFakeField(), Is.SameAs(this.fakeReturnedFromFakeAndDummyManager));
        }

        [Test]
        public void Should_set_public_field_that_is_tagged()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(this.fixture.PublicFakeField, Is.SameAs(this.fakeReturnedFromFakeAndDummyManager));
        }

        [Test]
        public void Should_not_set_non_tagged_field()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(this.fixture.NonFakeField, Is.Null);
        }

        [Test]
        public void Should_set_sut_from_sut_initializer()
        {
            // Arrange
            var sut = A.Dummy<Sut>();

            A.CallTo(() => this.sutInitializer.CreateSut(A<Type>._, A<Action<Type, object>>._))
                .Returns(sut);

            var fixture = new SutFixture();

            // Act
            this.initializer.InitializeFakes(fixture);

            // Assert
            Assert.That(fixture.Sut, Is.SameAs(sut));
        }

        [Test]
        public void Should_set_fake_from_sut_initializer_call_back_when_available()
        {
            // Arrange
            var sut = A.Dummy<Sut>();
            var fake = A.Fake<IFoo>();

            A.CallTo(() => this.sutInitializer.CreateSut(A<Type>._, A<Action<Type, object>>._))
                .Invokes(x => x.GetArgument<Action<Type, object>>(1).Invoke(typeof(IFoo), fake));

            var fixture = new SutFixture();

            // Act
            this.initializer.InitializeFakes(fixture);

            // Assert
            Assert.That(fixture.Foo, Is.SameAs(fake));
        }

        [Test]
        public void Should_fail_when_more_than_one_member_is_marked_as_sut()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(
                () => this.initializer.InitializeFakes(new SutFixtureWithTwoSutMembers()),
                Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("A fake fixture can only contain one member marked \"under test\"."));
        }

        protected virtual void OnSetUp()
        {
            Fake.InitializeFixture(this);

            A.CallTo(() => this.fakeAndDummyManager.CreateFake(typeof(IFoo), A<FakeOptions>._)).Returns(this.fakeReturnedFromFakeAndDummyManager);

            this.fixture = new FixtureType();
        }

        public class Sut
        {
            public Sut(IFoo foo)
            {
            }
        }

        public class SutFixture
        {
            [Fake]
            public IFoo Foo { get; set; }

            [UnderTest]
            public Sut Sut { get; set; }
        }

        public class SutFixtureWithTwoSutMembers
        {
            [UnderTest]
            public Sut Sut { get; set; }

            [UnderTest]
            public Sut Sut2 { get; set; }
        }

        public class FixtureType
        {
            [Fake]
            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Testing only.")]
            public IFoo PublicFakeField;

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Testing only.")]
            public IFoo NonFakeField;

            [Fake]
            private IFoo privateFakeField;

            [Fake]
            public IFoo FooProperty { get; set; }

            public IFoo NonFakedProperty { get; set; }

            [Fake]
            private IFoo PriveFakeProperty { get; set; }

            public IFoo GetValueOfPrivateFakeProperty()
            {
                return this.PriveFakeProperty;
            }

            public IFoo GetValueOfPrivateFakeField()
            {
                return this.privateFakeField;
            }
        }
    }
}
