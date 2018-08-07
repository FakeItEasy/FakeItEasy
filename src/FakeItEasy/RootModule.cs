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
            container.RegisterSingleton(c =>
                new DynamicOptionsBuilder(
                    c.Resolve<IEnumerable<IFakeOptionsBuilder>>()));

            container.RegisterSingleton<IExpressionCallMatcherFactory>(c => new ExpressionCallMatcherFactory(c));

            container.RegisterSingleton(c =>
                new ExpressionArgumentConstraintFactory(c.Resolve<IArgumentConstraintTrapper>()));

            container.RegisterSingleton<ExpressionCallRule.Factory>(c =>
                callSpecification => new ExpressionCallRule(new ExpressionCallMatcher(callSpecification, c.Resolve<ExpressionArgumentConstraintFactory>(), c.Resolve<MethodInfoManager>())));

            container.RegisterSingleton(c =>
                new MethodInfoManager());

            container.Register<FakeAsserter.Factory>(c => calls => new FakeAsserter(calls, c.Resolve<CallWriter>(), c.Resolve<StringBuilderOutputWriter.Factory>()));

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
                var fakeConfigurator = c.Resolve<DynamicOptionsBuilder>();

                var dynamicDummyFactory = new DynamicDummyFactory(c.Resolve<IEnumerable<IDummyFactory>>());
                var dummyValueResolver = new DummyValueResolver(dynamicDummyFactory, fakeCreator);

                return new DefaultFakeAndDummyManager(
                    dummyValueResolver,
                    fakeCreator,
                    fakeConfigurator);
            });

            container.RegisterSingleton(c => new CastleDynamicProxyInterceptionValidator(c.Resolve<MethodInfoManager>()));

            container.RegisterSingleton(c => new DelegateProxyInterceptionValidator());

            container.RegisterSingleton<IFakeManagerAccessor>(c => new DefaultFakeManagerAccessor());

            container.Register(c => new FakeFacade(c.Resolve<IFakeManagerAccessor>()));

            container.RegisterSingleton<IEqualityComparer<IFakeObjectCall>>(c => new FakeCallEqualityComparer());

            container.Register<IInterceptionAsserter>(c => new DefaultInterceptionAsserter(c.Resolve<IMethodInterceptionValidator>()));

            container.Register<IArgumentConstraintTrapper>(c => new ArgumentConstraintTrap());

            container.Register<IArgumentConstraintManagerFactory>(c => new ArgumentConstraintManagerFactory());

            container.RegisterSingleton<IOutputWriter>(c => new DefaultOutputWriter(Console.Write, c.Resolve<ArgumentValueFormatter>()));

            container.RegisterSingleton<StringBuilderOutputWriter.Factory>(c => builder => new StringBuilderOutputWriter(builder, c.Resolve<ArgumentValueFormatter>()));

            container.Register(c => c.Resolve<StringBuilderOutputWriter.Factory>().Invoke(new StringBuilder()));

            container.RegisterSingleton(c => new EventHandlerArgumentProviderMap());

            container.Register(c => new SequentialCallContext(c.Resolve<CallWriter>(), c.Resolve<StringBuilderOutputWriter.Factory>()));
        }

        private class ExpressionCallMatcherFactory
            : IExpressionCallMatcherFactory
        {
            private readonly ServiceLocator serviceLocator;

            public ExpressionCallMatcherFactory(ServiceLocator serviceLocator)
            {
                this.serviceLocator = serviceLocator;
            }

            public ICallMatcher CreateCallMatcher(ParsedCallExpression callSpecification) => new ExpressionCallMatcher(
                    callSpecification,
                    this.serviceLocator.Resolve<ExpressionArgumentConstraintFactory>(),
                    this.serviceLocator.Resolve<MethodInfoManager>());
        }

        private class ArgumentConstraintManagerFactory
            : IArgumentConstraintManagerFactory
        {
            public INegatableArgumentConstraintManager<T> Create<T>()
            {
                return new DefaultArgumentConstraintManager<T>(ArgumentConstraintTrap.ReportTrappedConstraint);
            }
        }
    }
}
