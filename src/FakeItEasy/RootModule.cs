namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
    internal class RootModule
        : Module
    {
        /// <summary>
        /// Registers the dependencies.
        /// </summary>
        /// <param name="container">The container to register the dependencies in.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Container configuration.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Container configuration.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Container configuration.")]
        public override void RegisterDependencies(DictionaryContainer container)
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

            container.Register<FakeAsserter.Factory>(c => calls => new FakeAsserter(calls, c.Resolve<CallWriter>()));

            container.RegisterSingleton<FakeManager.Factory>(c =>
                (fakeObjectType, proxy) => new FakeManager(fakeObjectType, proxy, c.Resolve<IFakeManagerAccessor>()));

            container.RegisterSingleton<FakeCallProcessorProvider.Factory>(c =>
                (typeOfFake, proxyOptions) =>
                    new FakeManagerProvider(c.Resolve<FakeManager.Factory>(), c.Resolve<IFakeManagerAccessor>(), typeOfFake, proxyOptions));

            container.RegisterSingleton<IFakeObjectCallFormatter>(c =>
                new DefaultFakeObjectCallFormatter(c.Resolve<ArgumentValueFormatter>(), c.Resolve<IFakeManagerAccessor>()));

            container.RegisterSingleton(c =>
                new ArgumentValueFormatter(c.Resolve<IEnumerable<IArgumentValueFormatter>>()));

            container.RegisterSingleton(c =>
                new CallWriter(c.Resolve<IFakeObjectCallFormatter>(), c.Resolve<IEqualityComparer<IFakeObjectCall>>()));

            container.RegisterSingleton<ICallExpressionParser>(c =>
                new CallExpressionParser());

            container.RegisterSingleton<IFakeAndDummyManager>(c =>
            {
                var fakeCreator = new FakeObjectCreator(
                    c.Resolve<IProxyGenerator>(),
                    c.Resolve<IExceptionThrower>(),
                    c.Resolve<FakeCallProcessorProvider.Factory>());
                var fakeConfigurator = c.Resolve<DynamicOptionsBuilder>();

                var dynamicDummyFactory = new DynamicDummyFactory(c.Resolve<IEnumerable<IDummyFactory>>());
                var objectCreator = new ResolverFakeObjectCreator(fakeCreator);
                var dummyValueResolver = new DummyValueResolver(dynamicDummyFactory, objectCreator);

                return new DefaultFakeAndDummyManager(
                    dummyValueResolver,
                    fakeCreator,
                    fakeConfigurator);
            });

            container.RegisterSingleton(c => new CastleDynamicProxyGenerator(c.Resolve<CastleDynamicProxyInterceptionValidator>()));

            container.RegisterSingleton(c => new DelegateProxyGenerator());

            container.RegisterSingleton<IProxyGenerator>(c => new ProxyGeneratorSelector(c.Resolve<DelegateProxyGenerator>(), c.Resolve<CastleDynamicProxyGenerator>()));

            container.RegisterSingleton(
                c => new CastleDynamicProxyInterceptionValidator(c.Resolve<MethodInfoManager>()));

            container.RegisterSingleton<IExceptionThrower>(c => new DefaultExceptionThrower());

            container.RegisterSingleton<IFakeManagerAccessor>(c => new DefaultFakeManagerAccessor());

            container.Register(c => new FakeFacade(c.Resolve<IFakeManagerAccessor>()));

            container.RegisterSingleton<IEqualityComparer<IFakeObjectCall>>(c => new FakeCallEqualityComparer());

            container.Register<IInterceptionAsserter>(c => new DefaultInterceptionAsserter(c.Resolve<IProxyGenerator>()));

            container.Register<IArgumentConstraintTrapper>(c => new ArgumentConstraintTrap());

            container.Register<IArgumentConstraintManagerFactory>(c => new ArgumentConstraintManagerFactory());

            container.RegisterSingleton<IOutputWriter>(c => new DefaultOutputWriter(Console.Write));

            container.RegisterSingleton(c => new EventHandlerArgumentProviderMap());

            container.Register(c => new SequentialCallContext(c.Resolve<CallWriter>()));
        }

        private class ExpressionCallMatcherFactory
            : IExpressionCallMatcherFactory
        {
            private readonly ServiceLocator serviceLocator;

            public ExpressionCallMatcherFactory(ServiceLocator serviceLocator)
            {
                this.serviceLocator = serviceLocator;
            }

            public ICallMatcher CreateCallMatcher(ParsedCallExpression callSpecification)
            {
                return new ExpressionCallMatcher(
                    callSpecification,
                    this.serviceLocator.Resolve<ExpressionArgumentConstraintFactory>(),
                    this.serviceLocator.Resolve<MethodInfoManager>());
            }
        }

        private class ResolverFakeObjectCreator
            : IFakeObjectCreator
        {
            private readonly FakeObjectCreator creator;

            public ResolverFakeObjectCreator(FakeObjectCreator creator)
            {
                this.creator = creator;
            }

            public bool TryCreateFakeObject(DummyCreationSession session, Type typeOfFake, DummyValueResolver resolver, out object result)
            {
                result = this.creator.CreateFake(typeOfFake, new ProxyOptions(), session, resolver, false);
                return result != null;
            }
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
