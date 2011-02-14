namespace FakeItEasy.Tests.Creation
{
    using System;
    using System.Linq;
    using FakeItEasy.Creation;
    using FakeItEasy.SelfInitializedFakes;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultFakeCreatorFacadeTests
    {
        private IFakeAndDummyManager fakeAndDummyManager;
        private DefaultFakeCreatorFacade creator;

        [SetUp]
        public void SetUp()
        {
            this.fakeAndDummyManager = A.Fake<IFakeAndDummyManager>();

            this.creator = new DefaultFakeCreatorFacade(this.fakeAndDummyManager);

            ConfigureDefaultValuesForFakeAndDummyManager();
        }

        [Test]
        public void CreateFake_should_pass_arguments_for_constructor_to_fake_and_dummy_manager()
        {
            // Arrange
            var serviceProvider = A.Fake<IServiceProvider>();
            
            // Act
            this.creator.CreateFake<Foo>(x => x.WithArgumentsForConstructor(() => new Foo(serviceProvider)));
            
            // Assert
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(A<Type>.Ignored, A<FakeOptions>.That.HasArgumentsForConstructor(new object[] { serviceProvider }))).MustHaveHappened();
        }
        
        [Test]
        public void CreateFake_should_pass_all_interfaces_from_implements_to_fake_and_dummy_manager()
        {
            // Arrange
            
            // Act
            this.creator.CreateFake<IFoo>(x => x.Implements(typeof(IFormatProvider)).Implements(typeof(IFormattable)));
            
            // Assert
            var optionsWithAllInterfaces = A<FakeOptions>.That.Matches(x => 
                                                                       x.AdditionalInterfacesToImplement.Contains(typeof(IFormatProvider)) && 
                                                                       x.AdditionalInterfacesToImplement.Contains(typeof(IFormattable)));
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(A<Type>.Ignored, optionsWithAllInterfaces)).MustHaveHappened();
        }

        [Test]
        public void CreateFake_should_throw_when_arguments_for_constructor_is_set_by_expression_that_is_not_a_constructor_call()
        {
            // Arrange
            
            // Act, Assert
            Assert.Throws<ArgumentException>(() =>
                                             this.creator.CreateFake<Foo>(x => x.WithArgumentsForConstructor(() => CreateFoo())));
        }

        [Test]
        public void CreateFake_should_pass_wrapped_instance_to_manager()
        {
            // Arrange
            var wrapped = A.Fake<IFoo>();

            // Act
            this.creator.CreateFake<IFoo>(x => x.Wrapping(wrapped));

            // Assert
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(A<Type>.Ignored, A<FakeOptions>.That.Wraps(wrapped))).MustHaveHappened();
        }

        [Test]
        public void CreateFake_should_pass_recorder_to_manager()
        {
            // Arrange
            var wrapped = A.Fake<IFoo>();
            var recorder = A.Fake<ISelfInitializingFakeRecorder>();

            // Act
            this.creator.CreateFake<IFoo>(x => x.Wrapping(wrapped).RecordedBy(recorder));

            // Assert
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(A<Type>.Ignored, A<FakeOptions>.That.HasRecorder(recorder))).MustHaveHappened();
        }

        [Test]
        public void CreateFake_should_return_instance_from_manager()
        {
            // Arrange
            var instanceFromManager = A.Fake<IFoo>();
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(A<Type>.Ignored, A<FakeOptions>.Ignored)).Returns(instanceFromManager);

            // Act
            var result = this.creator.CreateFake<IFoo>(x => { });

            // Assert
            Assert.That(result, Is.SameAs(instanceFromManager));
        }

        [Test]
        public void CreateFake_should_pass_type_to_manager()
        {
            // Arrange

            // Act
            this.creator.CreateFake<IFoo>(x => { });

            // Assert
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(typeof(IFoo), A<FakeOptions>.Ignored)).MustHaveHappened();
        }

        private static Foo CreateFoo()
        {
            return A.Fake<Foo>();
        }

        private object[] optionBuilderCalls = TestCases.Create<Func<IFakeOptionsBuilder<Foo>, IFakeOptionsBuilder<Foo>>>(
            x => x.Wrapping(A.Fake<Foo>()),
            x => x.Implements(typeof(Foo)),
            x => x.WithArgumentsForConstructor(() => new Foo()),
            x => x.WithArgumentsForConstructor(new object[] { A.Fake<IServiceProvider>() }),
            x => x.Wrapping(A.Fake<Foo>()).RecordedBy(A.Fake<ISelfInitializingFakeRecorder>()),
            x => x.AsStrictMock()
            ).AsTestCaseSource(x => x);
            
        [TestCaseSource("optionBuilderCalls")]
        public void CreateFake_should_pass_options_builder_that_returns_itself_for_any_call(Func<IFakeOptionsBuilder<Foo>, IFakeOptionsBuilder<Foo>> call)
        {
            // Arrange
            IFakeOptionsBuilder<Foo> builderPassedToAction = null;

            // Act
            this.creator.CreateFake<Foo>(x => { builderPassedToAction = x; });

            // Assert
            Assert.That(call.Invoke(builderPassedToAction), Is.SameAs(builderPassedToAction));
        }

        [Test]
        public void CreateDummy_should_delegate_to_fake_and_dummy_manager()
        {
            // Arrange
            var dummy = A.Fake<IFoo>();
            A.CallTo(() => this.fakeAndDummyManager.CreateDummy(typeof(IFoo))).Returns(dummy);

            // Act
            var result = this.creator.CreateDummy<IFoo>();

            // Assert
            Assert.That(result, Is.SameAs(dummy));
        }

        [Test]
        public void CreateFake_should_be_null_guarded()
        {
            // Arrange, Act, Assert
            NullGuardedConstraint.Assert(() => 
                                         this.creator.CreateFake<IFoo>(x => x.Wrapping(A.Fake<IFoo>())));
        }

        [Test]
        public void CreateFake_with_arguments_for_constructor_should_pass_arguments_for_constructor_to_fake_and_dummy_manager()
        {
            // Arrange
            var constructorArguments = new object[] { A.Fake<IServiceProvider>() };

            // Act
            this.creator.CreateFake<Foo>(x => x.WithArgumentsForConstructor(constructorArguments));

            // Assert
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(A<Type>.Ignored, A<FakeOptions>.That.HasArgumentsForConstructor(constructorArguments))).MustHaveHappened();
        }

        [Test]
        public void CollectionOfFake_should_return_collection_where_all_items_are_fakes()
        {
            // Arrange

            // Act
            var result = this.creator.CollectionOfFake<IFoo>(10);
            
            // Assert
            Assert.That(result, Has.All.InstanceOf<IFoo>().And.All.InstanceOf<ITaggable>());
        }

        [TestCase(2)]
        [TestCase(10)]
        public void CollectionOfFake_should_return_collection_with_the_number_of_items_specified(int numberOfFakes)
        {
            // Arrange

            // Act
            var result = this.creator.CollectionOfFake<IFoo>(numberOfFakes);

            // Assert
            Assert.That(result, Has.Count.EqualTo(numberOfFakes));
        }

        private void ConfigureDefaultValuesForFakeAndDummyManager()
        {
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(typeof(IFoo), A<FakeOptions>.Ignored))
                .Returns(A.Fake<IFoo>());
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(typeof(Foo), A<FakeOptions>.Ignored))
                .Returns(A.Fake<Foo>());
        }
    }
}