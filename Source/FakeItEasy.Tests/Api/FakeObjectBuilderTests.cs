namespace FakeItEasy.Tests.Api
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using NUnit.Framework;
    using FakeItEasy.SelfInitializedFakes;
    using System.Linq;
    using System.Collections;

    [TestFixture]
    public class FakeObjectBuilderTests
    {
        private FakeObjectFactory factory;
        private FakeObjectBuilder fakeObjectBuilder;

        [SetUp]
        public void SetUp()
        {
            this.factory = A.Fake<FakeObjectFactory>();
            A.CallTo(() => this.factory.CreateFake(typeof(IFoo), A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(A.Fake<IFoo>());

            this.fakeObjectBuilder = this.CreateBuilder();
        }

        private IFoo CreateFoo()
        {
            return A.Fake<IFoo>();
        }

        private FakeObjectBuilder CreateBuilder()
        {
            return new FakeObjectBuilder(this.factory);
        }

        [Test]
        public void Options_specifying_arguments_for_constructor_by_expression_that_is_not_actual_call_to_constructor_should_throw()
        {
            
            Assert.Throws<ArgumentException>(() =>
                this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.WithArgumentsForConstructor(() => CreateFoo())));
        }

        [Test]
        public void Fake_called_with_instance_returns_wrapping_fake_that_delegates_to_wrapped_object()
        {
            var wrapped = A.Fake<IFoo>();
            var wrapper = this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(wrapped));

            A.CallTo(() => wrapped.Baz()).Returns(10);

            Assert.That(wrapper.Baz(), Is.EqualTo(10));
        }



        [Test]
        public void Fake_with_wrapped_instance_will_override_behavior_of_wrapped_object_on_configured_methods()
        {
            var wrapped = A.Fake<IFoo>();
            var wrapper = this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(wrapped));

            Configure.Fake(wrapped).CallsTo(x => x.Biz()).Returns("wrapped");
            Configure.Fake(wrapper).CallsTo(x => x.Biz()).Returns("wrapper");

            Assert.That(wrapper.Biz(), Is.EqualTo("wrapper"));
        }

        [Test]
        public void Fake_with_wrapped_instance_should_add_WrappedFakeObjectRule_to_fake_object()
        {
            var wrapped = A.Fake<IFoo>();

            var foo = this.CreateFakeObject<IFoo>();
            A.CallTo(() => ((IFoo)foo.Object).ToString()).Returns("Tjena");

            A.CallTo(() => this.factory.CreateFake(A<Type>.Ignored, A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(foo.Object);

            this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(wrapped));

            Assert.That(foo.Rules.ToArray(), Has.Some.InstanceOf<WrappedObjectRule>());
        }

        [Test]
        public void Generic_Fake_with_constructor_call_expression_should_pass_values_from_constructor_expression_to_fake_factory()
        {
            var serviceProvider = A.Fake<IServiceProvider>();

            A.CallTo(() => this.factory.CreateFake(typeof(Foo), A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(A.Fake<Foo>());

            this.fakeObjectBuilder.GenerateFake<Foo>(x => x.WithArgumentsForConstructor(() => new Foo(serviceProvider)));

            Fake.Assert(this.factory)
                .WasCalled(x => x.CreateFake(typeof(Foo), A<IEnumerable<object>>.That.IsThisSequence(new object[] { serviceProvider }).Argument, false));
        }

        [Test]
        public void Generic_Fake_with_constructor_call_should_return_object_from_factory()
        {
            var foo = A.Fake<Foo>();

            A.CallTo(() => this.factory.CreateFake(typeof(Foo), A<IEnumerable<object>>.Ignored.Argument, false)).Returns(foo);

            var returned = this.fakeObjectBuilder.GenerateFake<Foo>(x => x.WithArgumentsForConstructor(() => new Foo(A.Fake<IServiceProvider>())));

            Assert.That(returned, Is.SameAs(foo));
        }



        [Test]
        public void Generate_fake_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() => this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(A.Fake<IFoo>())));
        }


        [Test]
        public void Fake_with_arguments_for_constructor_should_throw_if_the_fake_type_is_not_abstract()
        {
            Assert.Throws<InvalidOperationException>(() =>
                A.Fake<Foo>(x => x.WithArgumentsForConstructor(new object[] { "bar" })));
        }


        [Test]
        public void Generic_Fake_with_arguments_for_constructor_should_return_fake_from_factory()
        {
            var constructorArguments = new object[] { A.Fake<IServiceProvider>() };
            var foo = A.Fake<AbstractTypeWithNoDefaultConstructor>();
            
            A.CallTo(() => this.factory.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), A<IEnumerable<object>>.Ignored.Argument, false)).Returns(foo);

            var returned = this.fakeObjectBuilder.GenerateFake<AbstractTypeWithNoDefaultConstructor>(x => x.WithArgumentsForConstructor(constructorArguments));

            Assert.That(returned, Is.SameAs(foo));
        }



        [Test]
        public void Generic_Fake_with_arguments_for_constructor_should_pass_arguments_to_factory()
        {
            A.CallTo(() => this.factory.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(A.Fake<AbstractTypeWithNoDefaultConstructor>());

            var constructorArguments = new object[] { A.Fake<IServiceProvider>() };
            IServiceProvider p = (IServiceProvider)constructorArguments[0];

            this.fakeObjectBuilder.GenerateFake<AbstractTypeWithNoDefaultConstructor>(x => x.WithArgumentsForConstructor(constructorArguments));

            A.CallTo(() => this.factory.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), A<IEnumerable<object>>.That.IsThisSequence(constructorArguments).Argument, false)).Assert(Happened.Once);
        }

        [Test]
        public void Fake_with_arguments_for_constructor_should_be_properly_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.WithArgumentsForConstructor(new object[] { "foo", 1 })));
        }

        [Test]
        public void Fake_with_wrapped_instance_and_recorder_should_add_SelfInitializationRule_to_fake_object()
        {
            var recorder = A.Fake<ISelfInitializingFakeRecorder>();
            var wrapped = A.Fake<IFoo>();

            var wrapper = this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(wrapped).RecordedBy(recorder));
            var fake = Fake.GetFakeObject(wrapper);

            Assert.That(fake.Rules.First(), Is.InstanceOf<SelfInitializationRule>());
        }

        private FakeObject CreateFakeObject<T>()
        {
            return Fake.GetFakeObject(A.Fake<T>());
        }

        public abstract class AbstractTypeWithNoDefaultConstructor
        {
            protected AbstractTypeWithNoDefaultConstructor(IServiceProvider serviceProvider)
            {

            }
        }
    }
}
