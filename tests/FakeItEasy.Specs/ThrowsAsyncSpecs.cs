namespace FakeItEasy.Specs
{
    using System;
    using System.Threading.Tasks;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

    public class ThrowsAsyncSpecs
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
        public void NoResultPassExceptionInstance(IFoo fake, Task task)
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
        public void WithResultPassExceptionInstance(IFoo fake, Task<int> task)
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
        public void NoResultPassExceptionFactoryWithCallArg(IFoo fake, Task task)
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
        public void WithResultPassExceptionFactoryWithCallArg(IFoo fake, Task<int> task)
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
        public void NoResultPassExceptionFactoryWithNoArgs(IFoo fake, Task task)
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
        public void WithResultPassExceptionFactoryWithNoArgs(IFoo fake, Task<int> task)
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
        public void NoResultPassExceptionFactoryWithOneArg(IFoo fake, Task task)
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
        public void WithResultPassExceptionFactoryWithOneArg(IFoo fake, Task<int> task)
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
        public void NoResultPassExceptionFactoryWithTwoArgs(IFoo fake, Task task)
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
        public void WithResultPassExceptionFactoryWithTwoArgs(IFoo fake, Task<int> task)
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
        public void NoResultPassExceptionFactoryWithThreeArgs(IFoo fake, Task task)
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
        public void WithResultPassExceptionFactoryWithThreeArgs(IFoo fake, Task<int> task)
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
        public void NoResultPassExceptionFactoryWithFourArgs(IFoo fake, Task task)
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
        public void WithResultPassExceptionFactoryWithFourArgs(IFoo fake, Task<int> task)
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

        public class MyException : Exception
        {
        }
    }
}
