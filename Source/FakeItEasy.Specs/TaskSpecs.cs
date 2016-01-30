namespace FakeItEasy.Specs
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave;

    public static class TaskSpecs
    {
        public interface IFooBarService
        {
            Task CommandAsync();

            Task<int> QueryAsync();

            Task<Foo> GetFooAsync();

            Task<Bar> GetBarAsync();
        }

        [Scenario]
        public static void ConfiguredAsyncMethodWithReturnValueOfTypeInt(
            IFooBarService fake,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFooBarService>());

            "And a fake async method that is configured to return a completed task with a result of type int"
                .x(() => A.CallTo(() => fake.QueryAsync()).Returns(Task.FromResult(9)));

            "When calling the configured method"
                .x(() => task = fake.QueryAsync());

            "Then it should return a completed task"
                .x(() => task.IsCompleted.Should().BeTrue());

            "And the task's result should be the configured value"
                .x(() => task.Result.Should().Be(9));
        }

        [Scenario]
        public static void ConfiguredAsyncMethod(
            IFooBarService fake,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFooBarService>());

            "And a fake async method that is configured to return a completed task with no result"
                .x(() => A.CallTo(() => fake.CommandAsync()).Returns(Task.FromResult<object>(null)));

            "When calling the configured method"
                .x(() => task = fake.CommandAsync());

            "Then it should return a completed task"
                .x(() => task.IsCompleted.Should().BeTrue());
        }

        [Scenario]
        public static void ConfiguredAsyncMethodWithReturnValueOfFakeableType(
            IFooBarService fake,
            Foo result,
            Task<Foo> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFooBarService>());

            "And a fake async method that is configured to return a completed task with a result of a fakeable type"
                .x(() => A.CallTo(() => fake.GetFooAsync()).Returns(Task.FromResult(result)));

            "When calling the configured method"
                .x(() => task = fake.GetFooAsync());

            "Then it should return a completed task"
                .x(() => task.IsCompleted.Should().BeTrue());

            "And the task's result should be the configured value"
                .x(() => task.Result.Should().Be(result));
        }

        [Scenario]
        public static void ConfiguredAsyncMethodWithReturnValueOfNonFakeableType(
            IFooBarService fake,
            Bar result,
            Task<Bar> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFooBarService>());

            "And a fake async method that is configured to return a completed task with a result of a non-fakeable type"
                .x(() => A.CallTo(() => fake.GetBarAsync()).Returns(Task.FromResult(result)));

            "When calling the configured method"
                .x(() => task = fake.GetBarAsync());

            "Then it should return a completed task"
                .x(() => task.IsCompleted.Should().BeTrue());

            "And the task's result should be the configured value"
                .x(() => task.Result.Should().Be(result));
        }

        [Scenario]
        public static void UnconfiguredAsyncMethodWithReturnValueOfTypeInt(
            IFooBarService fake,
            Task<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFooBarService>());

            "When calling an unconfigured async method with a result of type int"
                .x(() => task = fake.QueryAsync());

            "Then it should return a completed task"
                .x(() => task.IsCompleted.Should().BeTrue());

            "And the task's result should be a dummy value"
                .x(() => task.Result.Should().Be(0));
        }

        [Scenario]
        public static void UnconfiguredAsyncMethod(
            IFooBarService fake,
            Task task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFooBarService>());

            "When calling an unconfigured async method with no result"
                .x(() => task = fake.CommandAsync());

            "Then it should return a completed task"
                .x(() => task.IsCompleted.Should().BeTrue());
        }

        [Scenario]
        public static void UnconfiguredAsyncMethodReturnValueOfFakeableType(
            IFooBarService fake,
            Task<Foo> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFooBarService>());

            "When calling an unconfigured async method with a result of a fakeable type"
                .x(() => task = fake.GetFooAsync());

            "Then it should return a completed task"
                .x(() => task.IsCompleted.Should().BeTrue());

            "And the task's result should be a dummy value"
                .x(() => task.Result.Should().BeAssignableTo<Foo>());
        }

        [Scenario]
        public static void UnconfiguredAsyncMethodReturnValueOfNonFakeableType(
            IFooBarService fake,
            Task<Bar> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFooBarService>());

            "When calling an unconfigured async method with a result of a non-fakeable type"
                .x(() => task = fake.GetBarAsync());

            "Then it should return a completed task"
                .x(() => task.IsCompleted.Should().BeTrue());

            "And the task's result should be null"
                .x(() => task.Result.Should().BeNull());
        }

        public class Foo
        {
        }

        public class Bar
        {
            // Non fakeable because the constructor is internal
            internal Bar()
            {
            }
        }
    }
}
