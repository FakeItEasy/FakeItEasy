namespace FakeItEasy
{
    using System;
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

            container.RegisterSingleton<IExpressionCallMatcherFactory>(c => new ExpressionCallMatcherFactory(expressionArgumentConstraintFactory, methodInfoManager));

            container.RegisterSingleton(c => expressionArgumentConstraintFactory);

            container.RegisterSingleton<FakeAsserter.Factory>(c => (calls, lastSequenceNumber) => new FakeAsserter(calls, lastSequenceNumber, c.Resolve<CallWriter>(), c.Resolve<StringBuilderOutputWriter.Factory>()));

            container.RegisterSingleton<IFakeObjectCallFormatter>(c =>
                new DefaultFakeObjectCallFormatter(c.Resolve<ArgumentValueFormatter>(), fakeManagerAccessor));

            container.RegisterSingleton(c =>
                new ArgumentValueFormatter(argumentValueFormatters, c.Resolve<StringBuilderOutputWriter.Factory>()));

            container.RegisterSingleton(c =>
                new CallWriter(c.Resolve<IFakeObjectCallFormatter>(), new FakeCallEqualityComparer()));

            container.RegisterSingleton<ICallExpressionParser>(c =>
                new CallExpressionParser());

            container.RegisterSingleton<IFakeAndDummyManager>(c =>
                new DefaultFakeAndDummyManager(
                    new DummyValueResolver(new DynamicDummyFactory(dummyFactories), fakeObjectCreator),
                    fakeObjectCreator,
                    implicitOptionsBuilderCatalogue));

            container.RegisterSingleton<IInterceptionAsserter>(c => new DefaultInterceptionAsserter(fakeObjectCreator));

            container.RegisterSingleton<IArgumentConstraintManagerFactory>(c => new ArgumentConstraintManagerFactory());

            container.RegisterSingleton<IOutputWriter>(c => new DefaultOutputWriter(Console.Write, c.Resolve<ArgumentValueFormatter>()));

            container.RegisterSingleton<StringBuilderOutputWriter.Factory>(c => () => new StringBuilderOutputWriter(new StringBuilder(), c.Resolve<ArgumentValueFormatter>()));

            container.RegisterSingleton(c => new EventHandlerArgumentProviderMap());

            container.RegisterSingleton<SequentialCallContext.Factory>(c => () => new SequentialCallContext(c.Resolve<CallWriter>(), c.Resolve<StringBuilderOutputWriter.Factory>()));

            container.RegisterSingleton<IConfigurationFactory>(c =>
                new ConfigurationFactory(c));
        
            container.RegisterSingleton<IStartConfigurationFactory>(c =>
                new StartConfigurationFactory(c, ExpressionCallRuleFactory));

            container.RegisterSingleton<RuleBuilder.Factory>(c =>
                (rule, fake) => new RuleBuilder(rule, fake, c.Resolve<FakeAsserter.Factory>()));

            container.RegisterSingleton<IFakeConfigurationManager>(c =>
                new FakeConfigurationManager(c.Resolve<IConfigurationFactory>(), ExpressionCallRuleFactory, c.Resolve<ICallExpressionParser>(), c.Resolve<IInterceptionAsserter>()));

            container.RegisterSingleton<IFakeManagerAccessor>(c => fakeManagerAccessor);

            FakeManager FakeManagerFactory(Type fakeObjectType, object proxy) =>
                new FakeManager(fakeObjectType, proxy, fakeManagerAccessor);

            IFakeCallProcessorProvider FakeCallProcessorProviderFactory(Type typeOfFake, IProxyOptions proxyOptions) =>
                new FakeManagerProvider(FakeManagerFactory, fakeManagerAccessor, typeOfFake, proxyOptions);

            ExpressionCallRule ExpressionCallRuleFactory(ParsedCallExpression callSpecification) =>
                new ExpressionCallRule(new ExpressionCallMatcher(callSpecification, expressionArgumentConstraintFactory, methodInfoManager));
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
            public ConfigurationFactory(DictionaryContainer container)
            {
                this.Container = container;
            }

            private DictionaryContainer Container { get; }

            private RuleBuilder.Factory BuilderFactory => this.Container.Resolve<RuleBuilder.Factory>();

            public IAnyCallConfigurationWithVoidReturnType CreateConfiguration(FakeManager fakeObject, BuildableCallRule callRule)
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
            private readonly ExpressionCallRule.Factory expressionCallRuleFactory;

            public StartConfigurationFactory(DictionaryContainer container, ExpressionCallRule.Factory expressionCallRuleFactory)
            {
                this.Container = container;
                this.expressionCallRuleFactory = expressionCallRuleFactory;
            }

            private DictionaryContainer Container { get; }

            public IStartConfiguration<TFake> CreateConfiguration<TFake>(FakeManager fakeObject)
            {
                return new StartConfiguration<TFake>(
                    fakeObject,
                    this.expressionCallRuleFactory,
                    this.Container.Resolve<IConfigurationFactory>(),
                    this.Container.Resolve<ICallExpressionParser>(),
                    this.Container.Resolve<IInterceptionAsserter>());
            }
        }
    }
}
