namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class DefaultFixtureInitializerTests
    {
        private readonly FixtureType fixture;

#pragma warning disable 649
        [UnderTest]
        private DefaultFixtureInitializer initializer;

        [Fake]
        private IFakeAndDummyManager fakeAndDummyManager;

        [Fake]
        private IFoo fakeReturnedFromFakeAndDummyManager;

        [Fake]
        private ISutInitializer sutInitializer;
#pragma warning restore 649

        public DefaultFixtureInitializerTests()
        {
            Fake.InitializeFixture(this);

            A.CallTo(() => this.fakeAndDummyManager.CreateFake(typeof(IFoo), A<Action<IFakeOptions>>._)).Returns(this.fakeReturnedFromFakeAndDummyManager);

            this.fixture = new FixtureType();
        }

        [Fact]
        public void Should_set_public_property_that_is_attributed()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            this.fixture.FooProperty.Should().BeSameAs(this.fakeReturnedFromFakeAndDummyManager);
        }

        [Fact]
        public void Should_not_set_untagged_property()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            this.fixture.NonFakedProperty.Should().BeNull();
        }

        [Fact]
        public void Should_set_private_property_that_is_tagged()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            this.fixture.GetValueOfPrivateFakeProperty().Should().BeSameAs(this.fakeReturnedFromFakeAndDummyManager);
        }

        [Fact]
        public void Should_set_private_field_that_is_tagged()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            this.fixture.GetValueOfPrivateFakeField().Should().BeSameAs(this.fakeReturnedFromFakeAndDummyManager);
        }

        [Fact]
        public void Should_set_public_field_that_is_tagged()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            this.fixture.PublicFakeField.Should().BeSameAs(this.fakeReturnedFromFakeAndDummyManager);
        }

        [Fact]
        public void Should_not_set_non_tagged_field()
        {
            // Arrange

            // Act
            this.initializer.InitializeFakes(this.fixture);

            // Assert
            this.fixture.NonFakeField.Should().BeNull();
        }

        [Fact]
        public void Should_set_sut_from_sut_initializer()
        {
            // Arrange
            var sut = A.Dummy<Sut>();

            A.CallTo(() => this.sutInitializer.CreateSut(A<Type>._, A<Action<Type, object>>._))
                .Returns(sut);

            var sutFixture = new SutFixture();

            // Act
            this.initializer.InitializeFakes(sutFixture);

            // Assert
            sutFixture.Sut.Should().BeSameAs(sut);
        }

        [Fact]
        public void Should_set_fake_from_sut_initializer_callback_when_available()
        {
            // Arrange
            var fake = A.Fake<IFoo>();

            A.CallTo(() => this.sutInitializer.CreateSut(A<Type>._, A<Action<Type, object>>._))
                .Invokes(x => x.GetArgument<Action<Type, object>>(1).Invoke(typeof(IFoo), fake));

            var sutFixture = new SutFixture();

            // Act
            this.initializer.InitializeFakes(sutFixture);

            // Assert
            sutFixture.Foo.Should().BeSameAs(fake);
        }

        [Fact]
        public void Should_fail_when_more_than_one_member_is_marked_as_sut()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => this.initializer.InitializeFakes(new SutFixtureWithTwoSutMembers()));

            // Assert
            exception.Should().BeAnExceptionOfType<InvalidOperationException>()
                .WithMessage("A fake fixture can only contain one member marked \"under test\".");
        }

        public class Sut
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "foo", Justification = "Required for testing.")]
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
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Required for testing.")]
            [Fake]
            public IFoo PublicFakeField;

            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Required for testing.")]
            public IFoo NonFakeField;

#pragma warning disable 649
            [Fake]
            private IFoo privateFakeField;
#pragma warning restore 649

            [Fake]
            public IFoo FooProperty { get; set; }

            public IFoo NonFakedProperty { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required for testing.")]
            [Fake]
            private IFoo PrivateFakeProperty { get; set; }

            [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required for testing.")]
            public IFoo GetValueOfPrivateFakeProperty()
            {
                return this.PrivateFakeProperty;
            }

            [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required for testing.")]
            public IFoo GetValueOfPrivateFakeField()
            {
                return this.privateFakeField;
            }
        }
    }
}
