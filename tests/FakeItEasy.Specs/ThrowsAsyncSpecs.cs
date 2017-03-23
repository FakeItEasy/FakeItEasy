namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
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
                .x(() => task.Exception?.InnerException.Should().BeAnExceptionOfType<MyException>());
        }

        [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Irrelevant for test purposes, and not compatible with .NET Core")]
        [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Irrelevant for test purposes")]
        public class MyException : Exception
        {
        }
    }
}
