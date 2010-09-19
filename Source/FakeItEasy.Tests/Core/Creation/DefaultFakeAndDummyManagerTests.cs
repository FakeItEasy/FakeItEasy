//namespace FakeItEasy.Tests.Core.Creation
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using FakeItEasy.Core;
//    using FakeItEasy.Core.Creation;
//    using FakeItEasy.Expressions;
//    using FakeItEasy.SelfInitializedFakes;
//    using NUnit.Framework;

//    [TestFixture]
//    public class DefaultFakeAndDummyManagerTests
//    {
//        private IFakeObjectContainer container;
//        private IProxyGenerator proxyGenerator;
//        private IDummyValueCreator dummyValueCreator;
//        private FakeManager.Factory fakeObjectFactory;
//        private FakeManager fakeManager;
//        private IFakeWrapperConfigurator fakeWrapperConfigurator;
//        private ArgumentConstraint<IEnumerable<Type>> noAdditionalInterfaces = A<IEnumerable<Type>>.Ignored;

//        private IFakeCreationSession session;
//        private DefaultFakeAndDummyManager fakeAndDummyManager;

//        [SetUp]
//        public void SetUp()
//        {
//            this.container = A.Fake<IFakeObjectContainer>();
//            this.proxyGenerator = A.Fake<IProxyGenerator>();
//            this.session = A.Fake<IFakeCreationSession>();
//            this.dummyValueCreator = A.Fake<IDummyValueCreator>();
//            this.fakeManager = A.Fake<FakeManager>();
//            this.fakeObjectFactory = () => this.fakeManager;
//            this.fakeWrapperConfigurator = A.Fake<IFakeWrapperConfigurator>();

//            A.CallTo(() => this.session.ProxyGenerator).Returns(this.proxyGenerator);
//            A.CallTo(() => this.session.DummyCreator).Returns(this.dummyValueCreator);

//            this.fakeAndDummyManager = A.Fake<DefaultFakeAndDummyManager>(x => x.WithArgumentsForConstructor(() =>
//                new DefaultFakeAndDummyManager(this.container, this.session, this.fakeObjectFactory, this.fakeWrapperConfigurator)));

//            Any.CallTo(this.fakeAndDummyManager).CallsBaseMethod();
//        }

//        private static ProxyResult CreateFakeProxyResult()
//        {
//            var proxyResult = A.Fake<ProxyResult>();
//            A.CallTo(() => proxyResult.ProxyWasSuccessfullyCreated).Returns(true);
//            A.CallTo(() => proxyResult.Proxy).Returns((IFakedProxy)A.Fake<IFoo>());
//            return proxyResult;
//        }

//        [Test]
//        public void CreateDummy_should_return_dummy_from_container_when_available()
//        {
//            // Arrange
//            var fake = A.Fake<IFoo>();

//            object output;
//            A.CallTo(() => this.container.TryCreateFakeObject(typeof(IFoo), out output))
//                .Returns(true)
//                .AssignsOutAndRefParameters(fake);

//            // Act
//            var result = this.fakeAndDummyManager.CreateDummy(typeof(IFoo));

//            // Assert
//            Assert.That(result, Is.SameAs(fake));
//        }

//        [Test]
//        public void CreateDummy_should_create_dummy_with_dummy_creator_when_container_doesnt_contain_type()
//        {
//            // Arrange
//            object output;
//            A.CallTo(() => this.dummyValueCreator.TryCreateDummyValue(typeof(int), out output))
//                .Returns(true).AssignsOutAndRefParameters(10);

//            // Act
//            var returned = this.fakeAndDummyManager.CreateDummy(typeof(int));

//            // Assert
//            Assert.That(returned, Is.EqualTo(10));
//        }

//        [Test]
//        public void CreateDummy_should_create_proxy_when_container_doesnt_contain_type()
//        {
//            // Arrange
//            var result = A.Fake<IFoo>();

//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), noAdditionalInterfaces.Argument, null, false)).Returns(result);

//            // Act
//            var returned = this.fakeAndDummyManager.CreateDummy(typeof(IFoo));

//            // Assert
//            Assert.That(returned, Is.SameAs(result));
//        }

//        [Test]
//        public void CreateDummy_should_create_proxy_when_both_proxy_generator_and_dummy_creator_can_create_dummy()
//        {
//            // Arrange
//            var expectedResult = A.Fake<IFoo>();
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), noAdditionalInterfaces.Argument, null, false)).Returns(expectedResult);

//            object output;
//            A.CallTo(() => this.dummyValueCreator.TryCreateDummyValue(typeof(IFoo), out output))
//                .Returns(true).AssignsOutAndRefParameters(A.Fake<IFoo>());

//            // Act
//            var returned = this.fakeAndDummyManager.CreateDummy(typeof(IFoo));

//            // Assert
//            Assert.That(returned, Is.SameAs(expectedResult));
//        }

//        [Test]
//        [SetCulture("en-US")]
//        public void CreateDummy_should_throw_when_dummy_cant_be_created()
//        {
//            // Arrange
//            object output;
//            A.CallTo(() => this.dummyValueCreator.TryCreateDummyValue(A<Type>.Ignored, out output)).Returns(false);
//            object output2;
//            A.CallTo(() => this.container.TryCreateFakeObject(A<Type>.Ignored, out output2)).Returns(false);

//            // Act

//            // Assert
//            var thrown = Assert.Throws<FakeCreationException>(() => this.fakeAndDummyManager.CreateDummy(typeof(int)));
//            Assert.That(thrown.Message, Text.Contains("FakeItEasy was unable to create dummy of type \"System.Int32\", register it in the current IFakeObjectContainer to enable this."));
//        }

//        [Test]
//        public void CreateProxy_should_pass_fake_object_from_factory_to_proxy_generator()
//        {
//            // Arrange

//            // Act
//            this.fakeAndDummyManager.CreateProxy(typeof(IFoo), Enumerable.Empty<Type>(), null, false);

//            // Assert
//            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), noAdditionalInterfaces.Argument, this.fakeManager, null)).MustHaveHappened();
//        }

//        [Test]
//        public void CreateProxy_should_set_proxy_from_generator_to_fake_object()
//        {
//            // Arrange
//            var returned = CreateFakeProxyResult();

//            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, A<FakeManager>.Ignored, A<IEnumerable<object>>.Ignored.Argument)).Returns(returned);

//            // Act
//            this.fakeAndDummyManager.CreateProxy(typeof(IFoo), null, null, false);

//            // Assert
//            A.CallTo(() => this.fakeManager.SetProxy(returned)).MustHaveHappened();
//        }

//        [Test]
//        public void CreateProxy_should_pass_arguments_for_constructor_to_proxy_generator()
//        {
//            // Arrange

//            // Act
//            this.fakeAndDummyManager.CreateProxy(typeof(IFoo), null, new[] { "constructor", "arguments" }, false);

//            // Assert
//            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>.Ignored, A<IEnumerable<Type>>.Ignored.Argument, A<FakeManager>.Ignored, A<IEnumerable<object>>.That.IsThisSequence("constructor", "arguments").Argument)).MustHaveHappened();
//        }

//        [Test]
//        public void CreateProxy_should_pass_additional_interfaces_to_proxy_generator()
//        {
//            // Arrange

//            // Act
//            this.fakeAndDummyManager.CreateProxy(typeof(IFoo), new[] { typeof(IFormatProvider) }, Enumerable.Empty<object>(), false);

//            // Assert
//            A.CallTo(() => this.proxyGenerator.GenerateProxy(
//                A<Type>.Ignored,
//                A<IEnumerable<Type>>.That.IsThisSequence(typeof(IFormatProvider)).Argument,
//                A<FakeManager>.Ignored,
//                A<IEnumerable<object>>.Ignored.Argument
//            )).MustHaveHappened();
//        }

//        [Test]
//        public void CreateFake_should_return_value_from_create_proxy()
//        {
//            // Arrange
//            var foo = A.Fake<IFoo>();
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(foo);

//            // Act
//            var returned = this.fakeAndDummyManager.CreateFake(typeof(IFoo), A.Dummy<FakeOptions>());

//            // Assert
//            Assert.That(returned, Is.SameAs(foo));
//        }

//        [Test]
//        public void CreateFake_should_send_arguments_for_constructor_from_options_to_CreateProxy()
//        {
//            // Arrange
//            var foo = A.Fake<IFoo>();
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(foo);

//            // Act
//            var returned = this.fakeAndDummyManager.CreateFake(typeof(IFoo), new FakeOptions { ArgumentsForConstructor = new[] { "a", "b" } });

//            // Assert
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, A<IEnumerable<object>>.That.IsThisSequence("a", "b").Argument, A<bool>.Ignored)).MustHaveHappened();
//        }

//        [Test]
//        public void CreateFake_should_send_additional_interfaces_to_implement_from_options_to_CreateProxy()
//        {
//            // Arrange
//            var foo = A.Fake<IFoo>();
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(foo);

//            // Act
//            var returned = this.fakeAndDummyManager.CreateFake(typeof(IFoo), new FakeOptions { AdditionalInterfacesToImplement = new[] { typeof(IFormatProvider) } });

//            // Assert
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.That.IsThisSequence(typeof(IFormatProvider)).Argument, A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).MustHaveHappened();
//        }

//        [Test]
//        [SetCulture("en-US")]
//        public void CreateProxy_should_fail_when_proxy_can_not_be_generated_and_throwOnFailure_is_set_to_true()
//        {
//            // Arrange
//            var proxyResult = CreateFakeProxyResult();
//            A.CallTo(() => proxyResult.ProxyWasSuccessfullyCreated).Returns(false);
//            A.CallTo(() => proxyResult.ErrorMessage).Returns(@"Error message
//with two lines.");

//            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, this.fakeManager, null)).Returns(proxyResult);

//            // Act

//            // Assert
//            var ex = Assert.Throws<FakeCreationException>(() =>
//                this.fakeAndDummyManager.CreateProxy(typeof(IFoo), Enumerable.Empty<Type>(), null, true));

//            Assert.That(ex.Message, Is.EqualTo(@"
//
//   FakeItEasy failed to create fake object of type ""FakeItEasy.Tests.IFoo"".
//
//   1. The type is not registered in the current IFakeObjectContainer.
//   2. The current IProxyGenerator failed to generate a proxy for the following reason:
//
//      Error message
//      with two lines.
//
//"));
//        }

//        [Test]
//        public void CreateProxy_should_return_null_when_throwOnFailure_is_false_and_proxy_is_not_successfully_created()
//        {
//            // Arrange
//            var result = A.Fake<ProxyResult>();
//            A.CallTo(() => result.ProxyWasSuccessfullyCreated).Returns(false);
//            A.CallTo(() => this.proxyGenerator.GenerateProxy(null, null, null, null)).WithAnyArguments().Returns(result);

//            // Act
//            var proxy = this.fakeAndDummyManager.CreateProxy(typeof(string), Enumerable.Empty<Type>(), null, false);

//            // Assert
//            Assert.That(proxy, Is.Null);
//        }

//        [Test]
//        public void TryCreateDummy_should_return_true_when_container_contains_dummy_type()
//        {
//            // Arrange
//            var foo = A.Fake<IFoo>();
//            object result = null;

//            A.CallTo(() => this.container.TryCreateFakeObject(typeof(IFoo), out result)).Returns(true).AssignsOutAndRefParameters(foo);
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, null, false)).Returns(null);

//            // Act
//            var success = this.fakeAndDummyManager.TryCreateDummy(typeof(IFoo), out result);

//            // Assert
//            Assert.That(success, Is.True);
//        }

//        [Test]
//        public void TryCreateDummy_should_assign_dummy_to_out_parameter_when_exists_in_container()
//        {
//            // Arrange
//            var foo = A.Fake<IFoo>();
//            object result = null;

//            A.CallTo(() => this.container.TryCreateFakeObject(typeof(IFoo), out result)).Returns(true).AssignsOutAndRefParameters(foo);
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), Enumerable.Empty<Type>(), null, false)).Returns(null);

//            // Act
//            var success = this.fakeAndDummyManager.TryCreateDummy(typeof(IFoo), out result);

//            // Assert
//            Assert.That(result, Is.SameAs(foo));
//        }

//        [Test]
//        public void TryCreateDummy_should_return_true_when_dummy_creator_can_create_type()
//        {
//            // Arrange
//            object output;
//            A.CallTo(() => this.dummyValueCreator.TryCreateDummyValue(typeof(int), out output))
//                .Returns(true).AssignsOutAndRefParameters(10);

//            // Act
//            object output2;
//            var returned = this.fakeAndDummyManager.TryCreateDummy(typeof(int), out output2);

//            // Assert
//            Assert.That(returned, Is.True);
//        }

//        [Test]
//        public void TryCreateDummy_should_assign_result_when_dummy_creator_can_create_type()
//        {
//            // Arrange
//            object output;
//            A.CallTo(() => this.dummyValueCreator.TryCreateDummyValue(typeof(int), out output))
//                .Returns(true).AssignsOutAndRefParameters(10);

//            // Act
//            object result = null;
//            this.fakeAndDummyManager.TryCreateDummy(typeof(int), out result);

//            // Assert
//            Assert.That(result, Is.EqualTo(10));
//        }

//        [Test]
//        public void TryCreateDummy_should_return_false_when_create_proxy_returns_null()
//        {
//            // Arrange
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, null, false)).Returns(null);
//            object result = null;

//            // Act
//            var success = this.fakeAndDummyManager.TryCreateDummy(typeof(IFoo), out result);

//            // Assert
//            Assert.That(success, Is.False);
//        }

//        [Test]
//        public void TryCreateDummy_should_return_true_when_create_proxy_returns_result()
//        {
//            // Arrange
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, null, false)).Returns(A.Fake<IFoo>());

//            // Act
//            object result = null;
//            var success = this.fakeAndDummyManager.TryCreateDummy(typeof(IFoo), out result);

//            // Assert
//            Assert.That(success, Is.True);
//        }

//        [Test]
//        public void TryCreateDummy_should_assign_created_proxy_to_out_parameter_when_successful()
//        {
//            // Arrange
//            var foo = A.Fake<IFoo>();
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, null, false)).Returns(foo);
//            object result = null;

//            // Act
//            var success = this.fakeAndDummyManager.TryCreateDummy(typeof(IFoo), out result);

//            // Assert
//            Assert.That(result, Is.SameAs(foo));
//        }

//        [Test]
//        public void TryCreateFake_should_return_false_when_create_proxy_returns_null()
//        {
//            // Arrange
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, null, false)).Returns(null);
//            object result = null;

//            // Act
//            var success = this.fakeAndDummyManager.TryCreateFake(typeof(IFoo), A.Dummy<FakeOptions>(), out result);

//            // Assert
//            Assert.That(success, Is.False);
//        }

//        [Test]
//        public void TryCreateFake_should_return_true_when_create_proxy_returns_result()
//        {
//            // Arrange
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, null, false)).Returns(A.Fake<IFoo>());
//            object result = null;

//            // Act
//            var success = this.fakeAndDummyManager.TryCreateFake(typeof(IFoo), A.Dummy<FakeOptions>(), out result);

//            // Assert
//            Assert.That(success, Is.True);
//        }

//        [Test]
//        public void TryCreateFake_should_assign_created_proxy_to_out_parameter_when_successful()
//        {
//            // Arrange
//            var foo = A.Fake<IFoo>();
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, null, false)).Returns(foo);
//            object result = null;

//            // Act
//            var success = this.fakeAndDummyManager.TryCreateFake(typeof(IFoo), A.Dummy<FakeOptions>(), out result);

//            // Assert
//            Assert.That(result, Is.SameAs(foo));
//        }

//        [Test]
//        public void TryCreateFake_should_pass_arguments_for_constructor_from_options()
//        {
//            // Arrange
//            var options = new FakeOptions() { ArgumentsForConstructor = new[] { "a", "b" } };

//            // Act
//            object output;
//            this.fakeAndDummyManager.TryCreateFake(typeof(IFoo), options, out output);

//            // Assert
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, A<IEnumerable<object>>.That.IsThisSequence("a", "b").Argument, false)).MustHaveHappened();
//        }

//        [Test]
//        public void TryCreateFake_should_pass_additional_interfaces_to_implement_from_options()
//        {
//            // Arrange
//            var options = new FakeOptions() { AdditionalInterfacesToImplement = new[] { typeof(IFormattable) } };

//            // Act
//            object output;
//            this.fakeAndDummyManager.TryCreateFake(typeof(IFoo), options, out output);

//            // Assert
//            A.CallTo(() => this.fakeAndDummyManager.CreateProxy(typeof(IFoo), A<IEnumerable<Type>>.That.IsThisSequence(typeof(IFormattable)).Argument, A<IEnumerable<object>>.Ignored.Argument, false)).MustHaveHappened();
//        }

//        [Test]
//        public void TryCreateFake_should_pass_created_object_to_wrapper_configurator_when_wrapped_object_is_set()
//        {
//            // Arrange
//            var proxyResult = CreateFakeProxyResult();
//            var wrapped = A.Fake<IFoo>();
//            var recorder = A.Fake<ISelfInitializingFakeRecorder>();

//            var options = new FakeOptions()
//            {
//                WrappedInstance = wrapped,
//                SelfInitializedFakeRecorder = recorder
//            };


//            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<IEnumerable<Type>>.That.IsThisSequence().Argument, this.fakeManager, null))
//                .Returns(proxyResult);

//            // Act
//            object output;
//            this.fakeAndDummyManager.TryCreateFake(typeof(IFoo), options, out output);

//            // Assert
//            A.CallTo(() => this.fakeWrapperConfigurator.ConfigureFakeToWrap(proxyResult.Proxy, wrapped, recorder)).MustHaveHappened();
//        }

//        [Test]
//        public void TryCreateFake_should_not_call_wrapper_configurator_when_wrapped_object_is_not_set()
//        {
//            // Arrange
//            var options = new FakeOptions()
//            {
//                WrappedInstance = null
//            };

//            var proxyResult = CreateFakeProxyResult();

//            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>.Ignored, A<IEnumerable<Type>>.Ignored.Argument, A<FakeManager>.Ignored, A<IEnumerable<object>>.Ignored.Argument)).Returns(proxyResult);

//            // Act
//            object output;
//            this.fakeAndDummyManager.TryCreateFake(typeof(IFoo), options, out output);

//            // Assert
//            A.CallTo(() => this.fakeWrapperConfigurator.ConfigureFakeToWrap(A<object>.Ignored, A<object>.Ignored, A<ISelfInitializingFakeRecorder>.Ignored.Argument)).MustNotHaveHappened();
//        }

//        [Test]
//        public void CreateFake_should_pass_created_object_to_wrapper_configurator_when_wrapped_object_is_set()
//        {
//            // Arrange
//            var proxyResult = CreateFakeProxyResult();
//            var wrapped = A.Fake<IFoo>();
//            var recorder = A.Fake<ISelfInitializingFakeRecorder>();

//            var options = new FakeOptions()
//            {
//                WrappedInstance = wrapped,
//                SelfInitializedFakeRecorder = recorder
//            };

//            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, this.fakeManager, null)).Returns(proxyResult);

//            // Act
//            this.fakeAndDummyManager.CreateFake(typeof(IFoo), options);

//            // Assert
//            A.CallTo(() => this.fakeWrapperConfigurator.ConfigureFakeToWrap(proxyResult.Proxy, wrapped, recorder)).MustHaveHappened();
//        }

//        [Test]
//        public void CreateFake_should_not_call_wrapper_configurator_when_wrapped_object_is_not_set()
//        {
//            // Arrange
//            var options = new FakeOptions()
//            {
//                WrappedInstance = null
//            };

//            var proxyResult = CreateFakeProxyResult();

//            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>.Ignored, A<IEnumerable<Type>>.Ignored.Argument, A<FakeManager>.Ignored, A<IEnumerable<object>>.Ignored.Argument)).Returns(proxyResult);

//            // Act
//            this.fakeAndDummyManager.CreateFake(typeof(IFoo), options);

//            // Assert
//            A.CallTo(() => this.fakeWrapperConfigurator.ConfigureFakeToWrap(A<object>.Ignored, A<object>.Ignored, A<ISelfInitializingFakeRecorder>.Ignored.Argument)).MustNotHaveHappened();
//        }
//    }
//}
