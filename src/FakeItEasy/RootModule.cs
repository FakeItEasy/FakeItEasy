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

            container.RegisterSingleton(c =>
                new TypeCatalogueInstanceProvider(c.Resolve<ITypeCatalogue>()));
            container.RegisterSingleton<ITypeCatalogue>(c =>
            {
                var typeCatalogue = new TypeCatalogue();
                typeCatalogue.Load(bootstrapper.GetAssemblyFileNamesToScanForExtensions());
                return typeCatalogue;
            });

            RegisterEnumerableInstantiatedFromTypeCatalogue<IArgumentValueFormatter>(container);
            RegisterEnumerableInstantiatedFromTypeCatalogue<IDummyFactory>(container);
            RegisterEnumerableInstantiatedFromTypeCatalogue<IFakeOptionsBuilder>(container);

            var methodInfoManager = new MethodInfoManager();
            var argumentConstraintTrap = new ArgumentConstraintTrap();
            var expressionArgumentConstraintFactory = new ExpressionArgumentConstraintFactory(argumentConstraintTrap);

            container.RegisterSingleton(c =>
                new ImplicitOptionsBuilderCatalogue(
                    c.Resolve<IEnumerable<IFakeOptionsBuilder>>()));

            container.RegisterSingleton<IExpressionCallMatcherFactory>(c => new ExpressionCallMatcherFactory(expressionArgumentConstraintFactory, methodInfoManager));

            container.RegisterSingleton(c => expressionArgumentConstraintFactory);

            container.RegisterSingleton<ExpressionCallRule.Factory>(c =>
                callSpecification => new ExpressionCallRule(new ExpressionCallMatcher(callSpecification, expressionArgumentConstraintFactory, methodInfoManager)));

            container.RegisterSingleton<FakeAsserter.Factory>(c => (calls, lastSequenceNumber) => new FakeAsserter(calls, lastSequenceNumber, c.Resolve<CallWriter>(), c.Resolve<StringBuilderOutputWriter.Factory>()));

            container.RegisterSingleton<FakeManager.Factory>(c =>
                (fakeObjectType, proxy) => new FakeManager(fakeObjectType, proxy, c.Resolve<IFakeManagerAccessor>()));

            container.RegisterSingleton<FakeCallProcessorProvider.Factory>(c =>
                (typeOfFake, proxyOptions) =>
                    new FakeManagerProvider(c.Resolve<FakeManager.Factory>(), c.Resolve<IFakeManagerAccessor>(), typeOfFake, proxyOptions));

            container.RegisterSingleton<IFakeObjectCallFormatter>(c =>
                new DefaultFakeObjectCallFormatter(c.Resolve<ArgumentValueFormatter>(), c.Resolve<IFakeManagerAccessor>()));

            container.RegisterSingleton(c =>
                new ArgumentValueFormatter(c.Resolve<IEnumerable<IArgumentValueFormatter>>(), c.Resolve<StringBuilderOutputWriter.Factory>()));

            container.RegisterSingleton(c =>
                new CallWriter(c.Resolve<IFakeObjectCallFormatter>(), c.Resolve<IEqualityComparer<IFakeObjectCall>>()));

            container.RegisterSingleton<ICallExpressionParser>(c =>
                new CallExpressionParser());

            container.RegisterSingleton(c => new FakeObjectCreator(
                container.Resolve<FakeCallProcessorProvider.Factory>(),
                container.Resolve<CastleDynamicProxyInterceptionValidator>(),
                container.Resolve<DelegateProxyInterceptionValidator>()));
            container.RegisterSingleton<IFakeObjectCreator>(c => c.Resolve<FakeObjectCreator>());
            container.RegisterSingleton<IMethodInterceptionValidator>(c => c.Resolve<FakeObjectCreator>());

            container.RegisterSingleton<IFakeAndDummyManager>(c =>
            {
                var fakeCreator = c.Resolve<IFakeObjectCreator>();
                var fakeConfigurator = c.Resolve<ImplicitOptionsBuilderCatalogue>();

                var dynamicDummyFactory = new DynamicDummyFactory(c.Resolve<IEnumerable<IDummyFactory>>());
                var dummyValueResolver = new DummyValueResolver(dynamicDummyFactory, fakeCreator);

                return new DefaultFakeAndDummyManager(
                    dummyValueResolver,
                    fakeCreator,
                    fakeConfigurator);
            });

            container.RegisterSingleton(c => new CastleDynamicProxyInterceptionValidator(methodInfoManager));

            container.RegisterSingleton(c => new DelegateProxyInterceptionValidator());

            container.RegisterSingleton<IFakeManagerAccessor>(c => new DefaultFakeManagerAccessor());

            container.RegisterSingleton<IEqualityComparer<IFakeObjectCall>>(c => new FakeCallEqualityComparer());

            container.RegisterSingleton<IInterceptionAsserter>(c => new DefaultInterceptionAsserter(c.Resolve<IMethodInterceptionValidator>()));

            container.RegisterSingleton<IArgumentConstraintManagerFactory>(c => new ArgumentConstraintManagerFactory());

            container.RegisterSingleton<IOutputWriter>(c => new DefaultOutputWriter(Console.Write, c.Resolve<ArgumentValueFormatter>()));

            container.RegisterSingleton<StringBuilderOutputWriter.Factory>(c => () => new StringBuilderOutputWriter(new StringBuilder(), c.Resolve<ArgumentValueFormatter>()));

            container.RegisterSingleton(c => new EventHandlerArgumentProviderMap());

            container.RegisterSingleton<SequentialCallContext.Factory>(c => () => new SequentialCallContext(c.Resolve<CallWriter>(), c.Resolve<StringBuilderOutputWriter.Factory>()));

            container.RegisterSingleton<IConfigurationFactory>(c =>
                new ConfigurationFactory(c));

            container.RegisterSingleton<IStartConfigurationFactory>(c =>
                new StartConfigurationFactory(c));

            container.RegisterSingleton<RuleBuilder.Factory>(c =>
                (rule, fake) => new RuleBuilder(rule, fake, c.Resolve<FakeAsserter.Factory>()));

            container.RegisterSingleton<IFakeConfigurationManager>(c =>
                new FakeConfigurationManager(c.Resolve<IConfigurationFactory>(), c.Resolve<ExpressionCallRule.Factory>(), c.Resolve<ICallExpressionParser>(), c.Resolve<IInterceptionAsserter>()));
        }

        private static void RegisterEnumerableInstantiatedFromTypeCatalogue<T>(DictionaryContainer container)
        {
            container.RegisterSingleton(c =>
                c.Resolve<TypeCatalogueInstanceProvider>().InstantiateAllOfType<T>());
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
