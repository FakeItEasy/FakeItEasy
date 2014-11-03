namespace FakeItEasy.Tests.Creation.DelegateProxies
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation.DelegateProxies;
    using NUnit.Framework;

    [TestFixture]
    public class DelegateProxyGeneratorTests
    {
#pragma warning disable 649
        [UnderTest]
        private DelegateProxyGenerator generator;
#pragma warning restore 649

        private delegate void DelegateWithOutputValue(out string result);

        [SetUp]
        public void Setup()
        {
            Fake.InitializeFixture(this);
        }

        [TestCase(typeof(Func<int>))]
        [TestCase(typeof(Action))]
        [TestCase(typeof(EventHandler<EventArgs>))]
        [TestCase(typeof(Predicate<object>))]
        public void Should_be_successful_with_delegate_types(Type delegateType)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(delegateType, Enumerable.Empty<Type>(), Enumerable.Empty<object>(), A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            Assert.That(result.ProxyWasSuccessfullyGenerated, Is.True);
        }

        [TestCase(typeof(object))]
        [TestCase(typeof(string))]
        [TestCase(typeof(IServiceProvider))]
        [SetCulture("en-US")]
        public void Should_fail_for_non_delegate_types(Type nonDelegateType)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(nonDelegateType, Enumerable.Empty<Type>(), Enumerable.Empty<object>(), A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            Assert.That(result.ProxyWasSuccessfullyGenerated, Is.False);
            Assert.That(result.ReasonForFailure, Is.EqualTo("The delegate proxy generator can only create proxies for delegate types."));
        }

        [TestCase(typeof(Func<int>))]
        [TestCase(typeof(Action))]
        [TestCase(typeof(EventHandler<EventArgs>))]
        [TestCase(typeof(Predicate<object>))]
        public void Should_create_proxy_of_the_specified_type(Type delegateType)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(delegateType, Enumerable.Empty<Type>(), Enumerable.Empty<object>(), A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            Assert.That(result.GeneratedProxy.GetType(), Is.EqualTo(delegateType));
        }

        [Test]
        public void Should_ensure_fake_call_processor_is_initialized_but_not_fetched_when_no_method_on_fake_is_called()
        {
            // Arrange
            var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();

            // Act
            var proxyGeneratorResult = this.generator.GenerateProxy(typeof(Action), Enumerable.Empty<Type>(), null, fakeCallProcessorProvider);

            // Assert
            A.CallTo(() => fakeCallProcessorProvider.Fetch(A<object>._)).MustNotHaveHappened();
            A.CallTo(() => fakeCallProcessorProvider.EnsureInitialized(proxyGeneratorResult.GeneratedProxy)).MustHaveHappened();
        }

        [Test]
        public void Should_return_proxy_where_return_value_can_be_set()
        {
            // Arrange
            var proxy = this.GenerateProxy<Func<int>>(x => x.SetReturnValue(10));

            // Act
            var result = proxy.Invoke();

            // Assert
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void Should_return_proxy_that_exposes_arguments_passed_to_proxy()
        {
            // Arrange
            int firstArgument = 0;
            string secondArgument = null;

            var proxy = this.GenerateProxy<Action<int, string>>(x =>
                {
                    firstArgument = x.Arguments.Get<int>(0);
                    secondArgument = x.Arguments.Get<string>(1);
                });

            // Act
            proxy.Invoke(10, "foo");

            // Assert
            Assert.That(firstArgument, Is.EqualTo(10));
            Assert.That(secondArgument, Is.EqualTo("foo"));
        }

        [Test]
        public void Should_return_proxy_that_cannot_be_configured_to_call_base_method()
        {
            // Arrange
            var proxy = this.GenerateProxy<Action>(x =>
                {
                    var ex = Assert.Throws<NotSupportedException>(x.CallBaseMethod);
                    Assert.That(ex.Message, Is.EqualTo("Can not configure a delegate proxy to call base method."));
                });

            // Act
            proxy.Invoke();
        }

        [Test]
        public void Should_create_calls_that_exposes_return_value_when_converted_to_read_only()
        {
            // Arrange
            var proxy = this.GenerateProxy<Func<string>>(x =>
                {
                    x.SetReturnValue("foo");

                    Assert.That(x.AsReadOnly().ReturnValue, Is.EqualTo("foo"));
                });

            // Act
            proxy.Invoke();
        }

        [Test, Ignore]
        public void Should_return_proxy_where_out_parameter_can_be_set()
        {
            // Arrange
            var proxy = this.GenerateProxy<DelegateWithOutputValue>(x =>
                {
                    x.SetArgumentValue(0, "Foo");
                });

            // Act
            string output = null;
            proxy.Invoke(out output);

            // Assert
            Assert.That(output, Is.EqualTo("Foo"));
        }

        [Test]
        public void Should_return_proxy_where_calls_has_the_correct_method_set()
        {
            // Arrange
            var expectedMethod = typeof(Action).GetMethod("Invoke");
            MethodInfo actualMethod = null;

            var proxy = this.GenerateProxy<Action>(x =>
                {
                    actualMethod = x.Method;
                });

            // Act
            proxy.Invoke();

            // Assert
            Assert.That(expectedMethod, Is.EqualTo(actualMethod));
        }

        [Test]
        public void Should_return_proxy_where_calls_has_the_correct_faked_object_set()
        {
            // Arrange
            object fakedObject = null;

            var proxy = this.GenerateProxy<Action>(x =>
                {
                    fakedObject = x.FakedObject;
                });

            // Act
            proxy.Invoke();

            // Assert
            Assert.That(fakedObject, Is.SameAs(proxy));
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_return_false_for_non_invoke_method_when_asking_if_it_can_be_intercepted()
        {
            // Arrange
            var d = new Func<int>(() => 10);
            var getHashCode = d.GetType().GetMethod("GetHashCode");

            // Act
            string reason;
            var result = this.generator.MethodCanBeInterceptedOnInstance(getHashCode, d, out reason);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(reason, Is.EqualTo("Only the Invoke method can be intercepted on delegates."));
        }

        [Test]
        public void Should_return_true_for_invoke_method_when_asking_if_it_can_be_intercepted()
        {
            // Arrange
            var d = new Func<int>(() => 10);
            var invoke = d.GetType().GetMethod("Invoke");

            // Act
            string reason;
            var result = this.generator.MethodCanBeInterceptedOnInstance(invoke, d, out reason);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(reason, Is.Null);
        }

        private T GenerateProxy<T>(Action<IWritableFakeObjectCall> fakeCallProcessorAction)
        {
            var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();

            var result = this.generator.GenerateProxy(typeof(T), Enumerable.Empty<Type>(), Enumerable.Empty<object>(), fakeCallProcessorProvider);

            var fakeCallProcessor = A.Fake<IFakeCallProcessor>();
            A.CallTo(() => fakeCallProcessorProvider.Fetch(A<object>._)).Returns(fakeCallProcessor);
            A.CallTo(() => fakeCallProcessor.Process(A<IWritableFakeObjectCall>._)).Invokes(fakeCallProcessorAction);

            return (T)result.GeneratedProxy;
        }
    }
}
