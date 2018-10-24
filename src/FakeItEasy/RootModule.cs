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
    using FakeItEasy.IoC;

    /// <summary>
    /// Handles the registration of root dependencies in an IoC-container.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Container configuration.")]
    internal static class RootModule
    {
        /// <summary>
        /// Registers the dependencies.
        /// </summary>
        /// <param name="container">The container to register the dependencies in.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Container configuration.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Container configuration.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Container configuration.")]
        public static void RegisterDependencies(DictionaryContainer container)
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

            ArgumentValueFormatter argumentValueFormatter = null;
            var stringBuilderOutputWriterFactory = new StringBuilderOutputWriterFactory(new Lazy<ArgumentValueFormatter>(() => argumentValueFormatter));
            argumentValueFormatter = new ArgumentValueFormatter(argumentValueFormatters, stringBuilderOutputWriterFactory.Create);

            var fakeObjectCallFormatter = new DefaultFakeObjectCallFormatter(argumentValueFormatter, fakeManagerAccessor);

            var callWriter = new CallWriter(fakeObjectCallFormatter, new FakeCallEqualityComparer());

            container.RegisterSingleton<IExpressionCallMatcherFactory>(c => new ExpressionCallMatcherFactory(expressionArgumentConstraintFactory, methodInfoManager));

            container.RegisterSingleton(c => expressionArgumentConstraintFactory);

            container.RegisterSingleton(c => new CallWriter(fakeObjectCallFormatter, new FakeCallEqualityComparer()));

            container.RegisterSingleton<IFakeAndDummyManager>(c =>
                new DefaultFakeAndDummyManager(
                    new DummyValueResolver(new DynamicDummyFactory(dummyFactories), fakeObjectCreator),
                    fakeObjectCreator,
                    implicitOptionsBuilderCatalogue));

            container.RegisterSingleton<IArgumentConstraintManagerFactory>(c => new ArgumentConstraintManagerFactory());

            container.RegisterSingleton(c => new EventHandlerArgumentProviderMap());

            container.RegisterSingleton<SequentialCallContext.Factory>(c => SequentialCallContextFactory);

            container.RegisterSingleton<IConfigurationFactory>(c => new ConfigurationFactory(RuleBuilderFactory));

            container.RegisterSingleton<IStartConfigurationFactory>(c =>
                new StartConfigurationFactory(c, ExpressionCallRuleFactory, callExpressionParser, interceptionAsserter));

            container.RegisterSingleton<IFakeConfigurationManager>(c =>
                new FakeConfigurationManager(c.Resolve<IConfigurationFactory>(), ExpressionCallRuleFactory, callExpressionParser, interceptionAsserter));

            container.RegisterSingleton<IFakeManagerAccessor>(c => fakeManagerAccessor);

            container.RegisterSingleton<ICallExpressionParser>(c => callExpressionParser);

            container.RegisterSingleton<StringBuilderOutputWriter.Factory>(c => stringBuilderOutputWriterFactory.Create);

            container.RegisterSingleton<IFakeObjectCallFormatter>(c => fakeObjectCallFormatter);

            FakeManager FakeManagerFactory(Type fakeObjectType, object proxy) =>
                new FakeManager(fakeObjectType, proxy, fakeManagerAccessor);

            IFakeCallProcessorProvider FakeCallProcessorProviderFactory(Type typeOfFake, IProxyOptions proxyOptions) =>
                new FakeManagerProvider(FakeManagerFactory, fakeManagerAccessor, typeOfFake, proxyOptions);

            ExpressionCallRule ExpressionCallRuleFactory(ParsedCallExpression callSpecification) =>
                new ExpressionCallRule(new ExpressionCallMatcher(callSpecification, expressionArgumentConstraintFactory, methodInfoManager));

            IFakeAsserter FakeAsserterFactory(IEnumerable<ICompletedFakeObjectCall> calls, int lastSequenceNumber) =>
                new FakeAsserter(calls, lastSequenceNumber, callWriter, stringBuilderOutputWriterFactory.Create);

            SequentialCallContext SequentialCallContextFactory() =>
                new SequentialCallContext(callWriter, stringBuilderOutputWriterFactory.Create);

            RuleBuilder RuleBuilderFactory(BuildableCallRule rule, FakeManager fake) =>
                new RuleBuilder(rule, fake, FakeAsserterFactory);
        }

        private class StringBuilderOutputWriterFactory
        {
            private Lazy<ArgumentValueFormatter> argumentValueFormatter;

            public StringBuilderOutputWriterFactory(Lazy<ArgumentValueFormatter> argumentValueFormatterService)
            {
                this.argumentValueFormatter = argumentValueFormatterService;
            }

            public StringBuilderOutputWriter Create()
            {
                return new StringBuilderOutputWriter(new StringBuilder(), this.argumentValueFormatter.Value);
            }
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
            private readonly ICallExpressionParser callExpressionParser;
            private readonly IInterceptionAsserter interceptionAsserter;

            public StartConfigurationFactory(
                DictionaryContainer container,
                ExpressionCallRule.Factory expressionCallRuleFactory,
                ICallExpressionParser callExpressionParser,
                IInterceptionAsserter interceptionAsserter)
            {
                this.Container = container;
                this.expressionCallRuleFactory = expressionCallRuleFactory;
                this.callExpressionParser = callExpressionParser;
                this.interceptionAsserter = interceptionAsserter;
            }

            private DictionaryContainer Container { get; }

            public IStartConfiguration<TFake> CreateConfiguration<TFake>(FakeManager fakeObject)
            {
                return new StartConfiguration<TFake>(
                    fakeObject,
                    this.expressionCallRuleFactory,
                    this.Container.Resolve<IConfigurationFactory>(),
                    this.callExpressionParser,
                    this.interceptionAsserter);
            }
        }
    }
}
