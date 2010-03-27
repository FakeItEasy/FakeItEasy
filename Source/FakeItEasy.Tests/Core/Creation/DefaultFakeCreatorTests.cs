namespace FakeItEasy.Tests.Core.Creation
{
    using System;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using FakeItEasy.SelfInitializedFakes;
    using NUnit.Framework;
    
    [TestFixture]
    public class DefaultFakeCreatorTests
    {
        private IFakeAndDummyManager fakeAndDummyManager;
        private DefaultFakeCreator creator;

        [SetUp]
        public void SetUp()
        {
            this.fakeAndDummyManager = A.Fake<IFakeAndDummyManager>();

            this.creator = new DefaultFakeCreator(this.fakeAndDummyManager);

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
            var optionsWithCorrectArguments = A<FakeOptions>.That.Matches(x => x.ArgumentsForConstructor.SequenceEqual(new object[] { serviceProvider }));
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(A<Type>.Ignored, optionsWithCorrectArguments)).MustHaveHappened();
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
            var optionsWithWrappedInstance = A<FakeOptions>.That.Matches(x => x.WrappedInstance == wrapped);
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(A<Type>.Ignored, optionsWithWrappedInstance)).MustHaveHappened();
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
            x => x.Wrapping(A.Fake<Foo>()).RecordedBy(A.Fake<ISelfInitializingFakeRecorder>())
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
     
        //[Test]
        //public void Fake_with_wrapped_instance_will_override_behavior_of_wrapped_object_on_configured_methods()
        //{
        //    var wrapped = A.Fake<IFoo>();
        //    var wrapper = this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(wrapped));

        //    A.CallTo(() => wrapped.Biz()).Returns("wrapped");
        //    A.CallTo(() => wrapper.Biz()).Returns("wrapper");

        //    Assert.That(wrapper.Biz(), Is.EqualTo("wrapper"));
        //}

        //[Test]
        //public void Fake_with_wrapped_instance_should_add_WrappedFakeObjectRule_to_fake_object()
        //{
        //    var wrapped = A.Fake<IFoo>();

        //    var foo = this.CreateFakeObject<IFoo>();
        //    A.CallTo(() => ((IFoo)foo.Object).ToString()).Returns("Tjena");

        //    A.CallTo(() => this.factory.CreateFake(A<Type>.Ignored, A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(foo.Object);

        //    this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(wrapped));

        //    Assert.That(foo.Rules.ToArray(), Has.Some.InstanceOf<WrappedObjectRule>());
        //}

        //[Test]
        //public void Generic_Fake_with_constructor_call_expression_should_pass_values_from_constructor_expression_to_fake_factory()
        //{
        //    var serviceProvider = A.Fake<IServiceProvider>();

        //    A.CallTo(() => this.factory.CreateFake(typeof(Foo), A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(A.Fake<Foo>());

        //    this.fakeObjectBuilder.GenerateFake<Foo>(x => x.WithArgumentsForConstructor(() => new Foo(serviceProvider)));

        //    OldFake.Assert(this.factory)
        //        .WasCalled(x => x.CreateFake(typeof(Foo), A<IEnumerable<object>>.That.IsThisSequence(new object[] { serviceProvider }).Argument, false));
        //}

        //[Test]
        //public void Generic_Fake_with_constructor_call_should_return_object_from_factory()
        //{
        //    var foo = A.Fake<Foo>();

        //    A.CallTo(() => this.factory.CreateFake(typeof(Foo), A<IEnumerable<object>>.Ignored.Argument, false)).Returns(foo);

        //    var returned = this.fakeObjectBuilder.GenerateFake<Foo>(x => x.WithArgumentsForConstructor(() => new Foo(A.Fake<IServiceProvider>())));

        //    Assert.That(returned, Is.SameAs(foo));
        //}

        [Test]
        public void CreateFake_should_be_null_guarded()
        {
            // Arrange, Act, Assert
            NullGuardedConstraint.Assert(() => 
                this.creator.CreateFake<IFoo>(x => x.Wrapping(A.Fake<IFoo>())));
        }

        //[Test]
        //public void Fake_with_arguments_for_constructor_should_throw_if_the_fake_type_is_not_abstract()
        //{
        //    Assert.Throws<InvalidOperationException>(() =>
        //        A.Fake<Foo>(x => x.WithArgumentsForConstructor(new object[] { "bar" })));
        //}


        //[Test]
        //public void Generic_Fake_with_arguments_for_constructor_should_return_fake_from_factory()
        //{
        //    var constructorArguments = new object[] { A.Fake<IServiceProvider>() };
        //    var foo = A.Fake<AbstractTypeWithNoDefaultConstructor>();

        //    A.CallTo(() => this.factory.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), A<IEnumerable<object>>.Ignored.Argument, false)).Returns(foo);

        //    var returned = this.fakeObjectBuilder.GenerateFake<AbstractTypeWithNoDefaultConstructor>(x => x.WithArgumentsForConstructor(constructorArguments));

        //    Assert.That(returned, Is.SameAs(foo));
        //}



        //[Test]
        //public void Generic_Fake_with_arguments_for_constructor_should_pass_arguments_to_factory()
        //{
        //    A.CallTo(() => this.factory.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(A.Fake<AbstractTypeWithNoDefaultConstructor>());

        //    var constructorArguments = new object[] { A.Fake<IServiceProvider>() };
        //    IServiceProvider p = (IServiceProvider)constructorArguments[0];

        //    this.fakeObjectBuilder.GenerateFake<AbstractTypeWithNoDefaultConstructor>(x => x.WithArgumentsForConstructor(constructorArguments));

        //    A.CallTo(() => this.factory.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), A<IEnumerable<object>>.That.IsThisSequence(constructorArguments).Argument, false)).Assert(Happened.Once);
        //}

        //[Test]
        //public void Fake_with_arguments_for_constructor_should_be_properly_guarded()
        //{
        //    NullGuardedConstraint.Assert(() =>
        //        this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.WithArgumentsForConstructor(new object[] { "foo", 1 })));
        //}

        //[Test]
        //public void Fake_with_wrapped_instance_and_recorder_should_add_SelfInitializationRule_to_fake_object()
        //{
        //    var recorder = A.Fake<ISelfInitializingFakeRecorder>();
        //    var wrapped = A.Fake<IFoo>();

        //    var wrapper = this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(wrapped).RecordedBy(recorder));
        //    var fake = Fake.GetFakeObject(wrapper);

        //    Assert.That(fake.Rules.First(), Is.InstanceOf<SelfInitializationRule>());
        //}

        //private FakeObject CreateFakeObject<T>()
        //{
        //    return Fake.GetFakeObject(A.Fake<T>());
        //}

        //public abstract class AbstractTypeWithNoDefaultConstructor
        //{
        //    protected AbstractTypeWithNoDefaultConstructor(IServiceProvider serviceProvider)
        //    {

        //    }
        //}

        private void ConfigureDefaultValuesForFakeAndDummyManager()
        {
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(typeof(IFoo), A<FakeOptions>.Ignored))
                .Returns(A.Fake<IFoo>());
            A.CallTo(() => this.fakeAndDummyManager.CreateFake(typeof(Foo), A<FakeOptions>.Ignored))
                .Returns(A.Fake<Foo>());
        }
    }
}
