namespace FakeItEasy.Specs
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Machine.Specifications;

    public interface IFooAsyncAwaitService
    {
        Task CommandAsync();

        Task<int> QueryAsync();
    }

    public class when_calling_defined_method_with_return_value
    {
        static FooAsyncAwait foo;
        static Task task;

        Establish context = () =>
        {
            var service = A.Fake<IFooAsyncAwaitService>();
            A.CallTo(() => service.QueryAsync()).Returns(Task.FromResult(9));
            foo = new FooAsyncAwait(service);
        };

        Because of = () => task = foo.QueryAsync();

        It should_return = () => task.IsCompleted.Should().BeTrue();
    }

    public class when_calling_defined_void_method
    {
        static FooAsyncAwait foo;
        static Task task;

        Establish context = () =>
        {
            var service = A.Fake<IFooAsyncAwaitService>();
            A.CallTo(() => service.CommandAsync()).Returns(Task.FromResult<object>(null));
            foo = new FooAsyncAwait(service);
        };

        Because of = () => task = foo.CommandAsync();

        It should_return = () => task.IsCompleted.Should().BeTrue();
    }

    public class when_calling_undefined_method_with_return_value
    {
        static FooAsyncAwait foo;
        static Task task;

        Establish context = () => foo = new FooAsyncAwait(A.Fake<IFooAsyncAwaitService>());

        Because of = () => task = foo.QueryAsync();

        It should_return = () => task.IsCompleted.Should().BeTrue();
    }

    public class when_calling_undefined_void_method
    {
        static FooAsyncAwait foo;
        static Task task;

        Establish context = () => foo = new FooAsyncAwait(A.Fake<IFooAsyncAwaitService>());

        Because of = () => task = foo.CommandAsync();

        It should_return = () => task.IsCompleted.Should().BeTrue();
    }

    public class FooAsyncAwait
    {
        private readonly IFooAsyncAwaitService service;

        public FooAsyncAwait(IFooAsyncAwaitService service)
        {
            this.service = service;
        }

        public async Task CommandAsync()
        {
            await this.service.CommandAsync();
        }

        public async Task<int> QueryAsync()
        {
            return await this.service.QueryAsync();
        }
    }
}
