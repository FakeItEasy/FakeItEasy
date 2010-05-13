namespace FakeItEasy.Configuration
{
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;
    using FakeItEasy.VisualBasic;

    internal class ConfigurationModule
        : Module
    {
        public override void RegisterDependencies(DictionaryContainer container)
        {
            container.RegisterSingleton<RecordedCallRule.Factory>(c =>
                () => new RecordedCallRule(c.Resolve<MethodInfoManager>()));

            container.RegisterSingleton<IRecordingCallRuleFactory>(c =>
                new RecordingCallRuleFactory() 
                { 
                    Container = c 
                });

            container.RegisterSingleton<IConfigurationFactory>(c =>
                new ConfigurationFactory() 
                {
                    Container = c 
                });

            container.RegisterSingleton<IStartConfigurationFactory>(c =>
                new StartConfigurationFactory() 
                {
                    Container = c 
                });

            container.RegisterSingleton<RuleBuilder.Factory>(c =>
                (rule, fake) => new RuleBuilder(rule, fake, c.Resolve<FakeAsserter.Factory>()));

            container.RegisterSingleton<IFakeConfigurationManager>(c =>
                new FakeConfigurationManager(c.Resolve<IConfigurationFactory>(), c.Resolve<IExpressionParser>(), c.Resolve<ExpressionCallRule.Factory>(), c.Resolve<IProxyGenerator>()));
        }

        private class RecordingCallRuleFactory : IRecordingCallRuleFactory
        {
            public DictionaryContainer Container { get; set; }

            public RecordingCallRule<TFake> Create<TFake>(FakeManager fakeObject, RecordedCallRule recordedRule)
            {
                return new RecordingCallRule<TFake>(fakeObject, recordedRule, this.Container.Resolve<FakeAsserter.Factory>());
            }
        }

        private class ConfigurationFactory : IConfigurationFactory
        {
            public DictionaryContainer Container { get; set; }

            private RuleBuilder.Factory BuilderFactory
            {
                get
                {
                    return this.Container.Resolve<RuleBuilder.Factory>();
                }
            }

            public IVoidArgumentValidationConfiguration CreateConfiguration(FakeManager fakeObject, BuildableCallRule callRule)
            {
                return this.BuilderFactory.Invoke(callRule, fakeObject);
            }

            public IReturnValueArgumentValidationConfiguration<TMember> CreateConfiguration<TMember>(FakeManager fakeObject, BuildableCallRule callRule)
            {
                var parent = this.BuilderFactory.Invoke(callRule, fakeObject);
                var configuration = new RuleBuilder.ReturnValueConfiguration<TMember>();
                configuration.ParentConfiguration = parent;
                return configuration;
            }

            public IAnyCallConfiguration CreateAnyCallConfiguration(FakeManager fakeObject, AnyCallCallRule callRule)
            {
                return new AnyCallConfiguration(fakeObject, callRule, this.Container.Resolve<IConfigurationFactory>());
            }
        }

        private class StartConfigurationFactory : IStartConfigurationFactory
        {
            public DictionaryContainer Container { get; set; }

            public IStartConfiguration<TFake> CreateConfiguration<TFake>(FakeManager fakeObject)
            {
                return new StartConfiguration<TFake>(fakeObject, this.Container.Resolve<ExpressionCallRule.Factory>(), this.Container.Resolve<IConfigurationFactory>(), this.Container.Resolve<IProxyGenerator>());
            }
        }
    }
}
