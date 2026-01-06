namespace FakeItEasy.Specs;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FakeItEasy.Tests.TestHelpers;
using FluentAssertions;
using LambdaTale;

public static class ThrowsAsyncSpecs
{
    public interface IFoo
    {
        Task BarAsync();

        Task BarAsync(int a);

        Task BarAsync(int a, string b);

        Task BarAsync(int a, string b, bool c);

        Task BarAsync(int a, string b, bool c, double d);

        Task<int> BazAsync();

        Task<int> BazAsync(int a);

        Task<int> BazAsync(int a, string b);

        Task<int> BazAsync(int a, string b, bool c);

        Task<int> BazAsync(int a, string b, bool c, double d);
    }

    public interface IValueFoo
    {
        ValueTask BarAsync();

        ValueTask BarAsync(int a);

        ValueTask BarAsync(int a, string b);

        ValueTask BarAsync(int a, string b, bool c);

        ValueTask BarAsync(int a, string b, bool c, double d);

        ValueTask<int> BazAsync();

        ValueTask<int> BazAsync(int a);

        ValueTask<int> BazAsync(int a, string b);

        ValueTask<int> BazAsync(int a, string b, bool c);

        ValueTask<int> BazAsync(int a, string b, bool c, double d);
    }

    [Scenario]
    public static void NoResultPassExceptionInstance(IFoo fake, Task task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync()).ThrowsAsync(new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void WithResultPassExceptionInstance(IFoo fake, Task<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync()).ThrowsAsync(new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void NoResultPassExceptionFactoryWithCallArg(IFoo fake, Task task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync()).ThrowsAsync(call => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void WithResultPassExceptionFactoryWithCallArg(IFoo fake, Task<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync()).ThrowsAsync(call => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void NoResultPassExceptionFactoryWithNoArgs(IFoo fake, Task task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync()).ThrowsAsync(() => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void WithResultPassExceptionFactoryWithNoArgs(IFoo fake, Task<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync()).ThrowsAsync(() => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void NoResultPassExceptionFactoryWithOneArg(IFoo fake, Task task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync(0)).ThrowsAsync((int a) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(0); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void WithResultPassExceptionFactoryWithOneArg(IFoo fake, Task<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync(0)).ThrowsAsync((int a) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(0); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void NoResultPassExceptionFactoryWithTwoArgs(IFoo fake, Task task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync(0, "x"))
                .ThrowsAsync((int a, string b) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(0, "x"); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void WithResultPassExceptionFactoryWithTwoArgs(IFoo fake, Task<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync(0, "x"))
                .ThrowsAsync((int a, string b) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(0, "x"); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void NoResultPassExceptionFactoryWithThreeArgs(IFoo fake, Task task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync(0, "x", false))
                .ThrowsAsync((int a, string b, bool c) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(0, "x", false); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void WithResultPassExceptionFactoryWithThreeArgs(IFoo fake, Task<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync(0, "x", false))
                .ThrowsAsync((int a, string b, bool c) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(0, "x", false); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void NoResultPassExceptionFactoryWithFourArgs(IFoo fake, Task task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync(0, "x", false, 0.0))
                .ThrowsAsync((int a, string b, bool c, double d) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(0, "x", false, 0.0); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void WithResultPassExceptionFactoryWithFourArgs(IFoo fake, Task<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync(0, "x", false, 0.0))
                .ThrowsAsync((int a, string b, bool c, double d) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(0, "x", false, 0.0); });

        "Then it returns a failed task"
            .x(() => task.Status.Should().Be(TaskStatus.Faulted));

        "And the task's exception is the configured exception"
            .x(() => (task.Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskNoResultPassExceptionInstance(IValueFoo fake, ValueTask task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync()).ThrowsAsync(new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskWithResultPassExceptionInstance(IValueFoo fake, ValueTask<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync()).ThrowsAsync(new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskNoResultPassExceptionFactoryWithCallArg(IValueFoo fake, ValueTask task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync()).ThrowsAsync(call => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskWithResultPassExceptionFactoryWithCallArg(IValueFoo fake, ValueTask<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync()).ThrowsAsync(call => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskNoResultPassExceptionFactoryWithNoArgs(IValueFoo fake, ValueTask task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync()).ThrowsAsync(() => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskWithResultPassExceptionFactoryWithNoArgs(IValueFoo fake, ValueTask<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync()).ThrowsAsync(() => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskNoResultPassExceptionFactoryWithOneArg(IValueFoo fake, ValueTask task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync(0)).ThrowsAsync((int a) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(0); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskWithResultPassExceptionFactoryWithOneArg(IValueFoo fake, ValueTask<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync(0)).ThrowsAsync((int a) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(0); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskNoResultPassExceptionFactoryWithTwoArgs(IValueFoo fake, ValueTask task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync(0, "x"))
                .ThrowsAsync((int a, string b) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(0, "x"); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskWithResultPassExceptionFactoryWithTwoArgs(IValueFoo fake, ValueTask<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync(0, "x"))
                .ThrowsAsync((int a, string b) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(0, "x"); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskNoResultPassExceptionFactoryWithThreeArgs(IValueFoo fake, ValueTask task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync(0, "x", false))
                .ThrowsAsync((int a, string b, bool c) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(0, "x", false); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskWithResultPassExceptionFactoryWithThreeArgs(IValueFoo fake, ValueTask<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync(0, "x", false))
                .ThrowsAsync((int a, string b, bool c) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(0, "x", false); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskNoResultPassExceptionFactoryWithFourArgs(IValueFoo fake, ValueTask task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BarAsync(0, "x", false, 0.0))
                .ThrowsAsync((int a, string b, bool c, double d) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BarAsync(0, "x", false, 0.0); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    [Scenario]
    public static void ValueTaskWithResultPassExceptionFactoryWithFourArgs(IValueFoo fake, ValueTask<int> task)
    {
        "Given a fake"
            .x(() => fake = A.Fake<IValueFoo>());

        "And an async method of the fake configured to throw asynchronously"
            .x(() => A.CallTo(() => fake.BazAsync(0, "x", false, 0.0))
                .ThrowsAsync((int a, string b, bool c, double d) => new MyException()));

        "When that method is called"
            .x(() => { task = fake.BazAsync(0, "x", false, 0.0); });

        "Then it returns a failed task"
            .x(() => task.IsFaulted.Should().BeTrue());

        "And the task's exception is the configured exception"
            .x(() => (task.AsTask().Exception?.InnerException).Should().BeAnExceptionOfType<MyException>());
    }

    public class MyException : Exception
    {
    }
}
