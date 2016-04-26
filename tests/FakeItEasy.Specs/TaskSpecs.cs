namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave;

    public static class TaskSpecs
    {
        public interface IFooBarService
        {
            Task CommandAsync();

            Task<int> QueryAsync();

            [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not appropriate in this case")]
            Task<Foo> GetFooAsync();

            [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not appropriate in this case")]
            Task<Bar> GetBarAsync();
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
        public static void UnconfiguredAsyncMethodReturnValueOfDummyableType(
            IFooBarService fake,
            Task<Foo> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFooBarService>());

            "When calling an unconfigured async method with a result of a dummyable type"
                .x(() => task = fake.GetFooAsync());

            "Then it should return a completed task"
                .x(() => task.IsCompleted.Should().BeTrue());

            "And the task's result should be a dummy value"
                .x(() => task.Result.Should().BeAssignableTo<Foo>());
        }

        [Scenario]
        public static void UnconfiguredAsyncMethodReturnValueOfNonDummyableType(
            IFooBarService fake,
            Task<Bar> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFooBarService>());

            "When calling an unconfigured async method with a result of a non-dummyable type"
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
            // Non dummyable because the constructor is internal
            internal Bar()
            {
            }
        }
    }
}
