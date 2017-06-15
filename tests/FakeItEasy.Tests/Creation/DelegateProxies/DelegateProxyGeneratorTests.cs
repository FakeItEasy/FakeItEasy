namespace FakeItEasy.Tests.Creation.DelegateProxies
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation.DelegateProxies;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class DelegateProxyGeneratorTests
    {
        private readonly DelegateProxyGenerator generator;

        public DelegateProxyGeneratorTests()
        {
            this.generator = new DelegateProxyGenerator();
        }

        private delegate void VoidDelegateWithOutputValue(out string result);

        private delegate string NonVoidDelegateWithOutputValue(out int result);

        private delegate void VoidDelegateWithRefValue(ref string result);

        private delegate string NonVoidDelegateWithRefValue(ref int result);

        [Theory]
        [InlineData(typeof(Func<int>))]
        [InlineData(typeof(Action))]
        [InlineData(typeof(EventHandler<EventArgs>))]
        [InlineData(typeof(Predicate<object>))]
        public void Should_be_successful_with_delegate_types(Type delegateType)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(
                delegateType, Enumerable.Empty<Type>(), Enumerable.Empty<object>(), A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(string))]
        [InlineData(typeof(IServiceProvider))]
        [UsingCulture("en-US")]
        public void Should_fail_for_non_delegate_types(Type nonDelegateType)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(
                nonDelegateType, Enumerable.Empty<Type>(), Enumerable.Empty<object>(), A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeFalse();
            result.ReasonForFailure.Should().Be("The delegate proxy generator can only create proxies for delegate types.");
        }

        [Theory]
        [InlineData(typeof(Func<int>))]
        [InlineData(typeof(Action))]
        [InlineData(typeof(EventHandler<EventArgs>))]
        [InlineData(typeof(Predicate<object>))]
        public void Should_create_proxy_of_the_specified_type(Type delegateType)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(
                delegateType, Enumerable.Empty<Type>(), Enumerable.Empty<object>(), A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.GeneratedProxy.GetType().Should().Be(delegateType);
        }

        [Fact]
        public void Should_ensure_fake_call_processor_is_initialized_but_not_fetched_when_no_method_on_fake_is_called()
        {
            // Arrange
            var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();

            // Act
            var proxyGeneratorResult = this.generator.GenerateProxy(
                typeof(Action), Enumerable.Empty<Type>(), null, fakeCallProcessorProvider);

            // Assert
            A.CallTo(() => fakeCallProcessorProvider.Fetch(A<object>._)).MustNotHaveHappened();
            A.CallTo(() => fakeCallProcessorProvider.EnsureInitialized(proxyGeneratorResult.GeneratedProxy)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_proxy_where_return_value_can_be_set()
        {
            // Arrange
            var proxy = this.GenerateProxy<Func<int>>(x => x.SetReturnValue(10));

            // Act
            var result = proxy.Invoke();

            // Assert
            result.Should().Be(10);
        }

        [Fact]
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
            firstArgument.Should().Be(10);
            secondArgument.Should().Be("foo");
        }

        [Fact]
        public void Should_return_proxy_that_cannot_be_configured_to_call_base_method()
        {
            // Arrange
            var proxy = this.GenerateProxy<Action>(x =>
            {
                var exception = Record.Exception(() => x.CallBaseMethod());
                exception.Should()
                    .BeAnExceptionOfType<NotSupportedException>()
                    .WithMessage("Can not configure a delegate proxy to call base method.");
            });

            // Act
            proxy.Invoke();
        }

        [Fact]
        public void Should_create_calls_that_exposes_return_value_when_converted_to_read_only()
        {
            // Arrange
            var proxy = this.GenerateProxy<Func<string>>(x =>
            {
                x.SetReturnValue("foo");
                x.AsReadOnly().ReturnValue.Should().Be("foo");
            });

            // Act
            proxy.Invoke();
        }

        [Fact]
        public void Should_return_proxy_where_out_parameter_can_be_set_void()
        {
            // Arrange
            var proxy = this.GenerateProxy<VoidDelegateWithOutputValue>(x => { x.SetArgumentValue(0, "Foo"); });
            string output;

            // Act
            proxy.Invoke(out output);

            // Assert
            output.Should().Be("Foo");
        }

        [Fact]
        public void Should_return_proxy_where_out_parameter_can_be_set_non_void()
        {
            // Arrange
            var proxy = this.GenerateProxy<NonVoidDelegateWithOutputValue>(
                x =>
                {
                    x.SetArgumentValue(0, 42);
                    x.SetReturnValue("Foo");
                });

            int output;

            // Act
            string result = proxy.Invoke(out output);

            // Assert
            result.Should().Be("Foo");
            output.Should().Be(42);
        }

        [Fact]
        public void Should_return_proxy_where_ref_parameter_can_be_set_void()
        {
            // Arrange
            var proxy = this.GenerateProxy<VoidDelegateWithRefValue>(
                x =>
                {
                    var arg = x.GetArgument<string>(0);
                    x.SetArgumentValue(0, arg + arg);
                });

            string output = "Foo";

            // Act
            proxy.Invoke(ref output);

            // Assert
            output.Should().Be("FooFoo");
        }

        [Fact]
        public void Should_return_proxy_where_ref_parameter_can_be_set_non_void()
        {
            // Arrange
            var proxy = this.GenerateProxy<NonVoidDelegateWithRefValue>(
                x =>
                {
                    var arg = x.GetArgument<int>(0);
                    x.SetArgumentValue(0, arg + arg);
                    x.SetReturnValue("Foo");
                });

            int output = 21;

            // Act
            string result = proxy.Invoke(ref output);

            // Assert
            result.Should().Be("Foo");
            output.Should().Be(42);
        }

        [Fact]
        public void Should_return_proxy_where_calls_has_the_correct_method_set()
        {
            // Arrange
            var expectedMethod = typeof(Action).GetMethod("Invoke");
            MethodInfo actualMethod = null;
            var proxy = this.GenerateProxy<Action>(x => { actualMethod = x.Method; });

            // Act
            proxy.Invoke();

            // Assert
            expectedMethod.Should().BeSameAs(actualMethod);
        }

        [Fact]
        public void Should_return_proxy_where_calls_has_the_correct_faked_object_set()
        {
            // Arrange
            object fakedObject = null;
            var proxy = this.GenerateProxy<Action>(x => { fakedObject = x.FakedObject; });

            // Act
            proxy.Invoke();

            // Assert
            fakedObject.Should().BeSameAs(proxy);
        }

        [Fact]
        [UsingCulture("en-US")]
        public void Should_return_false_for_non_invoke_method_when_asking_if_it_can_be_intercepted()
        {
            // Arrange
            var d = new Func<int>(() => 10);
            var getHashCode = d.GetType().GetMethod("GetHashCode");
            string reason;

            // Act
            var result = this.generator.MethodCanBeInterceptedOnInstance(getHashCode, d, out reason);

            // Assert
            result.Should().BeFalse();
            reason.Should().Be("Only the Invoke method can be intercepted on delegates.");
        }

        [Fact]
        public void Should_return_true_for_invoke_method_when_asking_if_it_can_be_intercepted()
        {
            // Arrange
            var d = new Func<int>(() => 10);
            var invoke = d.GetType().GetMethod("Invoke");
            string reason;

            // Act
            var result = this.generator.MethodCanBeInterceptedOnInstance(invoke, d, out reason);

            // Assert
            result.Should().BeTrue();
            reason.Should().BeNull();
        }

        private T GenerateProxy<T>(Action<IInterceptedFakeObjectCall> fakeCallProcessorAction)
        {
            var fakeCallProcessor = A.Fake<IFakeCallProcessor>();
            A.CallTo(() => fakeCallProcessor.Process(A<IInterceptedFakeObjectCall>._)).Invokes(fakeCallProcessorAction);

            var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();
            A.CallTo(() => fakeCallProcessorProvider.Fetch(A<object>._)).Returns(fakeCallProcessor);

            return (T)this.generator
                .GenerateProxy(typeof(T), Enumerable.Empty<Type>(), Enumerable.Empty<object>(), fakeCallProcessorProvider)
                .GeneratedProxy;
        }
    }
}
