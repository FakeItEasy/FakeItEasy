namespace FakeItEasy.IntegrationTests
{
    using NUnit.Framework;
    using FakeItEasy.Core;

    [TestFixture]
    public class ServiceLocationIntegrationTests
    {
        [Test]
        public void Should_be_able_to_register_service_that_can_be_resolved()
        {
            var scope = A.Fake<IDependencyScope>();

            A.CallTo(() => scope.Resolve(typeof(string))).Returns("a value");

            var factory = A.Fake<IDependencyResolver>();
            A.CallTo(() => factory.CreateScope()).Returns(scope);

            ComponentStore.RegisterDependencyResolver(factory);

            using (var resolveScope = ComponentStore.BeginResolve())
            {
                Assert.That(resolveScope.Resolve(typeof(string)), Is.EqualTo("a value"));
            }
        }

        [Test]
        public void Should_be_able_to_resolve_fake_it_easy_default_registrations()
        {
            using (var resolveScope = ComponentStore.BeginResolve())
            {
                Assert.That(resolveScope.Resolve(typeof(FakeManager.Factory)), Is.Not.Null);
            }
        }
    }
}