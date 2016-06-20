namespace FakeItEasy.Tests.IoC
{
    using FakeItEasy.IoC;
    using FluentAssertions;
    using Xunit;

    public class DictionaryContainerTests
    {
        [Fact]
        public void Register_registers_resolver_to_resolve_each_time_resolve_is_called()
        {
            int calledNumberOfTimes = 0;

            var container = this.CreateContainer();
            container.Register(x =>
                {
                    calledNumberOfTimes++;
                    return A.Fake<IFoo>();
                });

            container.Resolve<IFoo>();
            container.Resolve<IFoo>();

            calledNumberOfTimes.Should().Be(2);
        }

        [Fact]
        public void RegisterSingleton_registers_resolver_to_be_invoked_once_only()
        {
            int calledNumberOfTimes = 0;

            var container = this.CreateContainer();
            container.RegisterSingleton(x =>
                {
                    calledNumberOfTimes++;
                    return A.Fake<IFoo>();
                });

            container.Resolve<IFoo>();
            container.Resolve<IFoo>();

            calledNumberOfTimes.Should().Be(1);
        }

        private DictionaryContainer CreateContainer()
        {
            return new DictionaryContainer();
        }
    }
}
