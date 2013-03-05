namespace FakeItEasy.Tests.IoC
{
    using FakeItEasy.IoC;
    using NUnit.Framework;

    [TestFixture]
    public class DictionaryContainerTests
    {
        [Test]
        public void Register_registers_resolver_to_resolve_each_time_resolve_is_called()
        {
            int calledNumberOfTimes = 0;

            var container = this.CreateContainer();
            container.Register<IFoo>(x =>
                {
                    calledNumberOfTimes++;
                    return A.Fake<IFoo>();
                });

            container.Resolve<IFoo>();
            container.Resolve<IFoo>();

            Assert.That(calledNumberOfTimes, Is.EqualTo(2));
        }

        [Test]
        public void RegisterSingleton_registers_resolver_to_be_invoked_once_only()
        {
            int calledNumberOfTimes = 0;

            var container = this.CreateContainer();
            container.RegisterSingleton<IFoo>(x =>
                {
                    calledNumberOfTimes++;
                    return A.Fake<IFoo>();
                });

            container.Resolve<IFoo>();
            container.Resolve<IFoo>();

            Assert.That(calledNumberOfTimes, Is.EqualTo(1));
        }

        private DictionaryContainer CreateContainer()
        {
            return new DictionaryContainer();
        }
    }
}
