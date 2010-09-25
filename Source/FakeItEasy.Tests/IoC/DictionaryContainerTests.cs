using FakeItEasy.IoC;
using NUnit.Framework;

namespace FakeItEasy.Tests.IoC
{
    [TestFixture]
    public class DictionaryContainerTests
    {
        private DictionaryContainer CreateContainer()
        {
            return new DictionaryContainer();
        }

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
    }
}
