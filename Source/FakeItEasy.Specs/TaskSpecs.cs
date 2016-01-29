namespace FakeItEasy.Specs
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave;

    public static class TaskSpecs
    {
        public interface IFooAsyncAwaitService
        {
            Task CommandAsync();

            Task<int> QueryAsync();

            Task<Bar> GetBarAsync();
        }

        [Scenario]
        public static void DefinedMethodWithReturnValue(
            FooAsyncAwait foo,
            Task<int> task)
        {
            "establish"
                .x(() =>
                    {
                        var service = A.Fake<IFooAsyncAwaitService>();
                        A.CallTo(() => service.QueryAsync()).Returns(Task.FromResult(9));
                        foo = new FooAsyncAwait(service);
                    });

            "when calling defined method with return value"
                .x(() => task = foo.QueryAsync());

            "it should return"
                .x(() => task.IsCompleted.Should().BeTrue());

            "the configured value"
                .x(() => task.Result.Should().Be(9));
        }

        [Scenario]
        public static void DefinedVoidMethod(
            FooAsyncAwait foo,
            Task task)
        {
            "establish"
                .x(() =>
                    {
                        var service = A.Fake<IFooAsyncAwaitService>();
                        A.CallTo(() => service.CommandAsync()).Returns(Task.FromResult<object>(null));
                        foo = new FooAsyncAwait(service);
                    });

            "when calling defined void method"
                .x(() => task = foo.CommandAsync());

            "it should return"
                .x(() => task.IsCompleted.Should().BeTrue());
        }

        [Scenario]
        public static void DefinedMethodWithReturnValueOfNonFakeableType(
            FooAsyncAwait foo,
            Bar bar,
            Task<Bar> task)
        {
            "establish"
                .x(() =>
                {
                        bar = new Bar();
                        var service = A.Fake<IFooAsyncAwaitService>();
                        A.CallTo(() => service.GetBarAsync()).Returns(Task.FromResult(bar));
                        foo = new FooAsyncAwait(service);
                    });

            "when calling defined method with return value"
                .x(() => task = foo.GetBarAsync());

            "it should return"
                .x(() => task.IsCompleted.Should().BeTrue());

            "the configured value"
                .x(() => task.Result.Should().Be(bar));
        }

        [Scenario]
        public static void UndefinedMethodWithReturnValue(
            FooAsyncAwait foo,
            Task<int> task)
        {
            "establish"
                .x(() => foo = new FooAsyncAwait(A.Fake<IFooAsyncAwaitService>()));

            "when calling undefined method with return value"
                .x(() => task = foo.QueryAsync());

            "it should return"
                .x(() => task.IsCompleted.Should().BeTrue());

            "a dummy value"
                .x(() => task.Result.Should().Be(0));
        }

        [Scenario]
        public static void UndefinedVoidMethod(
            FooAsyncAwait foo,
            Task task)
        {
            "establish"
                .x(() => foo = new FooAsyncAwait(A.Fake<IFooAsyncAwaitService>()));

            "when calling undefined void method"
                .x(() => task = foo.CommandAsync());

            "it should return"
                .x(() => task.IsCompleted.Should().BeTrue());
        }

        [Scenario]
        public static void UndefinedMethodReturnValueOfNonFakeableType(
            FooAsyncAwait foo,
            Task<Bar> task)
        {
            "establish"
                .x(() => foo = new FooAsyncAwait(A.Fake<IFooAsyncAwaitService>()));

            "when calling undefined method with return value"
                .x(() => task = foo.GetBarAsync());

            "it should return"
                .x(() => task.IsCompleted.Should().BeTrue());

            "and the task result should be null"
                .x(() => task.Result.Should().BeNull());
        }

        public class Bar
        {
            internal Bar()
            {
            }
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

            public async Task<Bar> GetBarAsync()
            {
                return await this.service.GetBarAsync();
            }
        }
    }
}
