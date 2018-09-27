namespace FakeItEasy.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;
    using Xunit.Abstractions;

    public class ConcurrentCallTests
    {
        private readonly ITestOutputHelper output;

        public ConcurrentCallTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        public interface ITestInterface
        {
            int MyMethod();
        }

        [Fact]
        public void Concurrent_calls_are_correctly_recorded()
        {
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    ITestInterface fake = A.Fake<ITestInterface>();

                    int count = 0;
                    A.CallTo(() => fake.MyMethod()).ReturnsLazily(p => Interlocked.Increment(ref count));

                    Task<int>[] tasks =
                    {
                        Task.Run(() => fake.MyMethod()),
                        Task.Run(() => fake.MyMethod()),
                    };

                    int[] result = Task.WhenAll(tasks).Result;

                    result.Should().BeEquivalentTo(1, 2);

                    A.CallTo(() => fake.MyMethod()).MustHaveHappenedTwiceExactly();
                }
                catch
                {
                    this.output.WriteLine($"Failed at iteration {i}");
                    throw;
                }
            }
        }
    }
}
