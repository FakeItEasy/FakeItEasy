namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using FakeItEasy.Creation.DelegateProxies;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Handles the registration of root dependencies in an IoC-container.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Container configuration.")]
    internal static class RootModule
    {
        public interface IServiceRegistrar
        {
            void Register<T>(T service) where T : class;
        }

        public static void RegisterDependencies(IServiceRegistrar registrar)
        {
            var bootstrapper = BootstrapperLocator.FindBootstrapper();

            var typeCatalogue = new TypeCatalogue();
            typeCatalogue.Load(bootstrapper.GetAssemblyFileNamesToScanForExtensions());
            var typeCatalogueInstanceProvider = new TypeCatalogueInstanceProvider(typeCatalogue);

            var argumentValueFormatters = typeCatalogueInstanceProvider.InstantiateAllOfType<IArgumentValueFormatter>();
            var dummyFactories = typeCatalogueInstanceProvider.InstantiateAllOfType<IDummyFactory>();
            var fakeOptionsBuilders = typeCatalogueInstanceProvider.InstantiateAllOfType<IFakeOptionsBuilder>();

            var implicitOptionsBuilderCatalogue = new ImplicitOptionsBuilderCatalogue(fakeOptionsBuilders);

            var methodInfoManager = new MethodInfoManager();
            var argumentConstraintTrap = new ArgumentConstraintTrap();
            var expressionArgumentConstraintFactory = new ExpressionArgumentConstraintFactory(argumentConstraintTrap);

            var fakeManagerAccessor = new DefaultFakeManagerAccessor();

            var fakeObjectCreator = new FakeObjectCreator(
                FakeCallProcessorProviderFactory,
                new CastleDynamicProxyInterceptionValidator(methodInfoManager),
                new DelegateProxyInterceptionValidator());

            var callExpressionParser = new CallExpressionParser();

            var interceptionAsserter = new DefaultInterceptionAsserter(fakeObjectCreator);

            var argumentValueFormatter = new ArgumentValueFormatter(argumentValueFormatters);

            var fakeObjectCallFormatter = new DefaultFakeObjectCallFormatter(argumentValueFormatter, fakeManagerAccessor);

            var callWriter = new CallWriter(fakeObjectCallFormatter, new FakeCallEqualityComparer());

            var configurationFactory = new ConfigurationFactory(RuleBuilderFactory);

            registrar.Register<IExpressionCallMatcherFactory>(new ExpressionCallMatcherFactory(expressionArgumentConstraintFactory, methodInfoManager));

            registrar.Register(expressionArgumentConstraintFactory);

            registrar.Register<FakeAndDummyManager>(
                new FakeAndDummyManager(
                    new DummyValueResolver(new DynamicDummyFactory(dummyFactories), fakeObjectCreator),
                    fakeObjectCreator,
                    implicitOptionsBuilderCatalogue));

            registrar.Register<IArgumentConstraintManagerFactory>(new ArgumentConstraintManagerFactory());

            registrar.Register(new EventHandlerArgumentProviderMap());

            registrar.Register<SequentialCallContext.Factory>(SequentialCallContextFactory);

            registrar.Register<IStartConfigurationFactory>(
                new StartConfigurationFactory(ExpressionCallRuleFactory, configurationFactory, callExpressionParser, interceptionAsserter));

            registrar.Register<IFakeConfigurationManager>(
                new FakeConfigurationManager(configurationFactory, ExpressionCallRuleFactory, callExpressionParser, interceptionAsserter));

            registrar.Register<IFakeManagerAccessor>(fakeManagerAccessor);

            registrar.Register<ICallExpressionParser>(callExpressionParser);

            registrar.Register((StringBuilderOutputWriter.Factory)StringBuilderOutputWriterFactory);

            registrar.Register<IFakeObjectCallFormatter>(fakeObjectCallFormatter);

            StringBuilderOutputWriter StringBuilderOutputWriterFactory() =>
                new StringBuilderOutputWriter(argumentValueFormatter);

            FakeManager FakeManagerFactory(Type fakeObjectType, object proxy, string? name) =>
                new FakeManager(fakeObjectType, proxy, name);

            IFakeCallProcessorProvider FakeCallProcessorProviderFactory(Type typeOfFake, IProxyOptions proxyOptions) =>
                new FakeManagerProvider(FakeManagerFactory, fakeManagerAccessor, typeOfFake, proxyOptions);

            ExpressionCallRule ExpressionCallRuleFactory(ParsedCallExpression callSpecification) =>
                new ExpressionCallRule(new ExpressionCallMatcher(callSpecification, expressionArgumentConstraintFactory, methodInfoManager));

            IFakeAsserter FakeAsserterFactory(IEnumerable<CompletedFakeObjectCall> calls, int lastSequenceNumber) =>
                new FakeAsserter(calls, lastSequenceNumber, callWriter, StringBuilderOutputWriterFactory);

            SequentialCallContext SequentialCallContextFactory() =>
                new SequentialCallContext(callWriter, StringBuilderOutputWriterFactory);

            RuleBuilder RuleBuilderFactory(BuildableCallRule rule, FakeManager fake) =>
                new RuleBuilder(rule, fake, FakeAsserterFactory);
        }

        private class ExpressionCallMatcherFactory
            : IExpressionCallMatcherFactory
        {
            private readonly ExpressionArgumentConstraintFactory expressionArgumentConstraintFactory;
            private readonly MethodInfoManager methodInfoManager;

            public ExpressionCallMatcherFactory(ExpressionArgumentConstraintFactory expressionArgumentConstraintFactory, MethodInfoManager methodInfoManager)
            {
                this.expressionArgumentConstraintFactory = expressionArgumentConstraintFactory;
                this.methodInfoManager = methodInfoManager;
            }

            public ICallMatcher CreateCallMatcher(ParsedCallExpression callSpecification) => new ExpressionCallMatcher(
                    callSpecification,
                    this.expressionArgumentConstraintFactory,
                    this.methodInfoManager);
        }

        private class ArgumentConstraintManagerFactory
            : IArgumentConstraintManagerFactory
        {
            public INegatableArgumentConstraintManager<T> Create<T>()
            {
                return new DefaultArgumentConstraintManager<T>(ArgumentConstraintTrap.ReportTrappedConstraint);
            }
        }

        private class ConfigurationFactory : IConfigurationFactory
        {
            private readonly RuleBuilder.Factory ruleBuilderFactory;

            public ConfigurationFactory(RuleBuilder.Factory ruleBuilderFactory)
            {
                this.ruleBuilderFactory = ruleBuilderFactory;
            }

            public IAnyCallConfigurationWithVoidReturnType CreateConfiguration(FakeManager fakeObject, BuildableCallRule callRule)
            {
                return this.ruleBuilderFactory.Invoke(callRule, fakeObject);
            }

            public IAnyCallConfigurationWithReturnTypeSpecified<TMember> CreateConfiguration<TMember>(FakeManager fakeObject, BuildableCallRule callRule)
            {
                var parent = this.ruleBuilderFactory.Invoke(callRule, fakeObject);
                return new RuleBuilder.ReturnValueConfiguration<TMember>(parent);
            }

            public IAnyCallConfigurationWithNoReturnTypeSpecified CreateAnyCallConfiguration(FakeManager fakeObject, AnyCallCallRule callRule)
            {
                return new AnyCallConfiguration(fakeObject, callRule, this);
            }
        }

        private class StartConfigurationFactory : IStartConfigurationFactory
        {
            private readonly ExpressionCallRule.Factory expressionCallRuleFactory;
            private readonly IConfigurationFactory configurationFactory;
            private readonly ICallExpressionParser callExpressionParser;
            private readonly IInterceptionAsserter interceptionAsserter;

            public StartConfigurationFactory(
                ExpressionCallRule.Factory expressionCallRuleFactory,
                IConfigurationFactory configurationFactory,
                ICallExpressionParser callExpressionParser,
                IInterceptionAsserter interceptionAsserter)
            {
                this.expressionCallRuleFactory = expressionCallRuleFactory;
                this.configurationFactory = configurationFactory;
                this.callExpressionParser = callExpressionParser;
                this.interceptionAsserter = interceptionAsserter;
            }

            public IStartConfiguration<TFake> CreateConfiguration<TFake>(FakeManager fakeObject)
            {
                return new StartConfiguration<TFake>(
                    fakeObject,
                    this.expressionCallRuleFactory,
                    this.configurationFactory,
                    this.callExpressionParser,
                    this.interceptionAsserter);
            }
        }
    }
}
