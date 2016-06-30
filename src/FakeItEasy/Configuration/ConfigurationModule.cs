namespace FakeItEasy.Configuration
{
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;

    internal class ConfigurationModule
        : Module
    {
        public override void RegisterDependencies(DictionaryContainer container)
        {
            container.RegisterSingleton<IConfigurationFactory>(c =>
                new ConfigurationFactory(c));

            container.RegisterSingleton<IStartConfigurationFactory>(c =>
                new StartConfigurationFactory(c));

            container.RegisterSingleton<RuleBuilder.Factory>(c =>
                (rule, fake) => new RuleBuilder(rule, fake, c.Resolve<FakeAsserter.Factory>()));

            container.RegisterSingleton<IFakeConfigurationManager>(c =>
                new FakeConfigurationManager(c.Resolve<IConfigurationFactory>(), c.Resolve<ExpressionCallRule.Factory>(), c.Resolve<ICallExpressionParser>(), c.Resolve<IInterceptionAsserter>()));
        }

        private class ConfigurationFactory : IConfigurationFactory
        {
            public ConfigurationFactory(DictionaryContainer container)
            {
                this.Container = container;
            }

            private DictionaryContainer Container { get; }

            private RuleBuilder.Factory BuilderFactory => this.Container.Resolve<RuleBuilder.Factory>();

            public IVoidArgumentValidationConfiguration CreateConfiguration(FakeManager fakeObject, BuildableCallRule callRule)
            {
                return this.BuilderFactory.Invoke(callRule, fakeObject);
            }

            public IAnyCallConfigurationWithReturnTypeSpecified<TMember> CreateConfiguration<TMember>(FakeManager fakeObject, BuildableCallRule callRule)
            {
                var parent = this.BuilderFactory.Invoke(callRule, fakeObject);
                return new RuleBuilder.ReturnValueConfiguration<TMember>(parent);
            }

            public IAnyCallConfigurationWithNoReturnTypeSpecified CreateAnyCallConfiguration(FakeManager fakeObject, AnyCallCallRule callRule)
            {
                return new AnyCallConfiguration(fakeObject, callRule, this.Container.Resolve<IConfigurationFactory>());
            }
        }

        private class StartConfigurationFactory : IStartConfigurationFactory
        {
            public StartConfigurationFactory(DictionaryContainer container)
            {
                this.Container = container;
            }

            private DictionaryContainer Container { get; }

            public IStartConfiguration<TFake> CreateConfiguration<TFake>(FakeManager fakeObject)
            {
                return new StartConfiguration<TFake>(
                    fakeObject,
                    this.Container.Resolve<ExpressionCallRule.Factory>(),
                    this.Container.Resolve<IConfigurationFactory>(),
                    this.Container.Resolve<ICallExpressionParser>(),
                    this.Container.Resolve<IInterceptionAsserter>());
            }
        }
    }
}
