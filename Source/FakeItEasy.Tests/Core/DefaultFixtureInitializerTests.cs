namespace FakeItEasy.Tests.Core
{
    using FakeItEasy.Core;
    using NUnit.Framework;
    using FakeItEasy.Creation;

    [TestFixture]
    public class DefaultFixtureInitializerTests
    {
        private DefaultFixtureInitializer initializer;
        private IFakeAndDummyManager fakeAndDummyManager;
        private IFoo fakeReturnedFromFakeAndDummyManager;
        private FixtureType fixture;

        [SetUp]
        public void SetUp()
        {
            this.OnSetUp();
        }

        protected virtual void OnSetUp()
        {
            this.fakeReturnedFromFakeAndDummyManager = A.Fake<IFoo>();
            
            this.fakeAndDummyManager = A.Fake<IFakeAndDummyManager>();
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(typeof(IFoo), A<FakeOptions>.Ignored)).Returns(this.fakeReturnedFromFakeAndDummyManager);

            this.fixture = new FixtureType();

            this.initializer = new DefaultFixtureInitializer(this.fakeAndDummyManager);
        }

        [Test]
        public void Should_set_public_property_that_is_attributed()
        {
            // Arrange
            
            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(fixture.FooProperty, Is.SameAs(this.fakeReturnedFromFakeAndDummyManager));
        }

        [Test]
        public void Should_not_set_untagged_property()
        {
            // Arrange
            
            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(fixture.NonFakedProperty, Is.Null);
        }

        [Test]
        public void Should_set_private_property_that_is_tagged()
        {
            // Arrange
            
            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(fixture.GetValueOfPrivateFakeProperty(), Is.SameAs(this.fakeReturnedFromFakeAndDummyManager));
        }

        [Test]
        public void Should_set_private_field_that_is_tagged()
        {
            // Arrange
            
            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(fixture.GetValueOfPrivateFakeField(), Is.SameAs(this.fakeReturnedFromFakeAndDummyManager));
        }

        [Test]
        public void Should_set_public_field_that_is_tagged()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(fixture.PublicFakeField, Is.SameAs(this.fakeReturnedFromFakeAndDummyManager));
        }

        [Test]
        public void Should_not_set_non_tagged_field()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            Assert.That(fixture.NonFakeField, Is.Null);
        }

        public class FixtureType
        {
            [Fake]
            public IFoo FooProperty { get; set; }

            public IFoo NonFakedProperty { get; set; }

            [Fake]
            private IFoo PriveFakeProperty { get; set; }

            [Fake]
            private IFoo privateFakeField;

            [Fake]
            public IFoo PublicFakeField;

            public IFoo NonFakeField;

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
