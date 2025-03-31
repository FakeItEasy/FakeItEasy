namespace FakeItEasy.Specs
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using LambdaTale;

    public class ValueTaskReturnValueSpecs
    {
        public interface IReturnValueTask
        {
            ValueTask<int> GetValueAsync();

            ValueTask<int> GetValueAsync(int a);

            ValueTask<int> GetValueAsync(int a, string b);

            ValueTask<int> GetValueAsync(int a, string b, bool c);
        }

        [Scenario]
        public void ValueTaskSpecificValue(IReturnValueTask fake, ValueTask<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IReturnValueTask>());

            "And a ValueTask-returning method of the fake configured to return a specific value"
                .x(() => A.CallTo(() => fake.GetValueAsync()).Returns(42));

            "When the configured method is called"
                .x(() => task = fake.GetValueAsync());

            "Then it returns a successfully completed ValueTask"
                .x(() => task.IsCompletedSuccessfully.Should().BeTrue());

            "And the ValueTask's result is the configured value"
                .x(() => task.Result.Should().Be(42));
        }

        [Scenario]
        public void ValueTaskLazilyComputed(IReturnValueTask fake, Func<int> valueProducer, ValueTask<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IReturnValueTask>());

            "And a value producer configured to return a specific value"
                .x(() => valueProducer = A.Fake<Func<int>>(o => o.ConfigureFake(f => A.CallTo(() => f()).Returns(42))));

            "And a ValueTask-returning method of the fake configured to return a value computed by the value producer"
                .x(() => A.CallTo(() => fake.GetValueAsync()).ReturnsLazily(valueProducer));

            "When the configured method is called"
                .x(() => task = fake.GetValueAsync());

            "Then it returns a successfully completed ValueTask"
                .x(() => task.IsCompletedSuccessfully.Should().BeTrue());

            "And the ValueTask's result is the configured value"
                .x(() => task.Result.Should().Be(42));

            "And the value producer has been called"
                .x(() => A.CallTo(() => valueProducer()).MustHaveHappenedOnceExactly());
        }

        [Scenario]
        public void ValueTaskFromSequence(
            IReturnValueTask fake,
            ValueTask<int> task1,
            ValueTask<int> task2,
            ValueTask<int> task3,
            ValueTask<int> task4)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IReturnValueTask>());

            "And a ValueTask-returning method of the fake configured to return values from a sequence of 3"
                .x(() => A.CallTo(() => fake.GetValueAsync()).ReturnsNextFromSequence(1, 2, 3));

            "When the configured method is called 4 times"
                .x(() =>
                {
                    task1 = fake.GetValueAsync();
                    task2 = fake.GetValueAsync();
                    task3 = fake.GetValueAsync();
                    task4 = fake.GetValueAsync();
                });

            "Then it returns 3 ValueTasks whose results are the values from the sequence"
                .x(() =>
                {
                    task1.Result.Should().Be(1);
                    task2.Result.Should().Be(2);
                    task3.Result.Should().Be(3);
                });

            "Then it returns a ValueTask whose result is a dummy"
                .x(() => task4.Result.Should().Be(0));
        }

        [Scenario]
        public void ValueTaskStronglyTypedLazilyComputed1Parameter(IReturnValueTask fake, Func<int, int> valueProducer, ValueTask<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IReturnValueTask>());

            "And a strongly typed value producer that returns a value based on the argument's value"
                .x(() => valueProducer = a => a + 1);

            "And a ValueTask-returning method of the fake configured to return a value computed by the value producer"
                .x(() => A.CallTo(() => fake.GetValueAsync(A<int>._)).ReturnsLazily(valueProducer));

            "When the configured method is called"
                .x(() => task = fake.GetValueAsync(41));

            "Then it returns a successfully completed ValueTask"
                .x(() => task.IsCompletedSuccessfully.Should().BeTrue());

            "And the ValueTask's result is the result of the value producer"
                .x(() => task.Result.Should().Be(42));
        }

        [Scenario]
        public void ValueTaskStronglyTypedLazilyComputed2Parameters(IReturnValueTask fake, Func<int, string, int> valueProducer, ValueTask<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IReturnValueTask>());

            "And a strongly typed value producer that returns a value based on the arguments' values"
                .x(() => valueProducer = (a, b) => a + b.Length);

            "And a ValueTask-returning method of the fake configured to return a value computed by the value producer"
                .x(() => A.CallTo(() => fake.GetValueAsync(A<int>._, A<string>._)).ReturnsLazily(valueProducer));

            "When the configured method is called"
                .x(() => task = fake.GetValueAsync(37, "hello"));

            "Then it returns a successfully completed ValueTask"
                .x(() => task.IsCompletedSuccessfully.Should().BeTrue());

            "And the ValueTask's result is the result of the value producer"
                .x(() => task.Result.Should().Be(42));
        }

        [Scenario]
        public void ValueTaskStronglyTypedLazilyComputed3Parameters(IReturnValueTask fake, Func<int, string, bool, int> valueProducer, ValueTask<int> task)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IReturnValueTask>());

            "And a strongly typed value producer that returns a value based on the arguments' values"
                .x(() => valueProducer = (a, b, c) => a + b.Length + (c ? 1 : 0));

            "And a ValueTask-returning method of the fake configured to return a value computed by the value producer"
                .x(() => A.CallTo(() => fake.GetValueAsync(A<int>._, A<string>._, A<bool>._)).ReturnsLazily(valueProducer));

            "When the configured method is called"
                .x(() => task = fake.GetValueAsync(36, "hello", true));

            "Then it returns a successfully completed ValueTask"
                .x(() => task.IsCompletedSuccessfully.Should().BeTrue());

            "And the ValueTask's result is the result of the value producer"
                .x(() => task.Result.Should().Be(42));
        }
    }
}
