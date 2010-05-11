namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using FakeItEasy.DynamicProxy;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;
    using FakeItEasy.SelfInitializedFakes;
    using System.Diagnostics;
    
    /// <summary>
    /// Handles the registration of root dependencies in an IoC-container.
    /// </summary>
    internal class RootModule 
        : Module
    {
        /// <summary>
        /// Registers the dependencies.
        /// </summary>
        /// <param name="container">The container to register the dependencies in.</param>
        public override void RegisterDependencies(DictionaryContainer container)
        {
            container.Register<FakeScope>(c => 
                FakeScope.Current);

            container.Register<IFakeObjectContainer>(c =>
                c.Resolve<FakeScope>().FakeObjectContainer);

            container.RegisterSingleton<IExpressionCallMatcherFactory>(c =>
                new ExpressionCallMatcherFactory { Container = c });

            container.RegisterSingleton<ArgumentConstraintFactory>(c => 
                new ArgumentConstraintFactory());

            container.RegisterSingleton<ExpressionCallRule.Factory>(c =>
                callSpecification => new ExpressionCallRule(new ExpressionCallMatcher(callSpecification, c.Resolve<ArgumentConstraintFactory>(), c.Resolve<MethodInfoManager>())));

            container.RegisterSingleton<MethodInfoManager>(c =>
                new MethodInfoManager());

            container.RegisterSingleton<FakeAsserter.Factory>(c =>
                x => new FakeAsserter(x, c.Resolve<CallWriter>()));

            container.RegisterSingleton<FakeManager.Factory>(c =>
                () => new FakeManager());

            container.RegisterSingleton<CallWriter>(c =>
                new CallWriter());

            container.RegisterSingleton<RecordingManager.Factory>(c =>
                x => new RecordingManager(x));

            container.RegisterSingleton<IFileSystem>(c =>
                new FileSystem());

            container.RegisterSingleton<FileStorage.Factory>(c =>
                x => new FileStorage(x, c.Resolve<IFileSystem>()));

            container.RegisterSingleton<IExpressionParser>(c =>
                new ExpressionParser());

            container.RegisterSingleton<VisualBasic.VisualBasicRuleBuilder.Factory>(c =>
                (rule, fakeObject) => new VisualBasic.VisualBasicRuleBuilder(rule, c.Resolve<RuleBuilder.Factory>().Invoke(rule, fakeObject)));

            container.Register<IFakeCreator>(c =>
                new DefaultFakeCreator(c.Resolve<IFakeAndDummyManager>()));

            container.Register<IFakeAndDummyManager>(c =>
                new DefaultFakeAndDummyManager(c.Resolve<IFakeObjectContainer>(), c.Resolve<IProxyGenerator>(), c.Resolve<FakeManager.Factory>(), c.Resolve<IFakeWrapperConfigurator>()));

            container.Register<IProxyGenerator>(c =>
                c.Resolve<IDummyResolvingSession>().ProxyGenerator);

            container.Register<IFakeWrapperConfigurator>(c =>
                new DefaultFakeWrapperConfigurator());

            container.Register<ITypeAccessor>(c =>
                new ApplicationDirectoryAssembliesTypeAccessor());

            container.Register<IDummyResolvingSession>(c =>
                new DummyResolvingSession(c));

            container.RegisterSingleton<IProxyGeneratorFactory>(c =>
                new DynamicProxyProxyGeneratorFactory());
        }

        private class DynamicProxyProxyGeneratorFactory
            : IProxyGeneratorFactory
        {
            public IProxyGenerator CreateProxyGenerator(IDummyResolvingSession session)
            {
                return new DynamicProxyProxyGenerator(session);
            }
        }

        private class DummyResolvingSession
            : IDummyResolvingSession
        {
            private Dictionary<Type, object> cachedValues = new Dictionary<Type, object>();
            private HashSet<Type> attemptedTypes = new HashSet<Type>();

            public DummyResolvingSession(DictionaryContainer container)
            {
                this.DummyCreator = new DefaultDummyValueCreator(this, container.Resolve<IFakeObjectContainer>());
                this.ConstructorResolver = new DefaultConstructorResolver(this);
                this.ProxyGenerator = container.Resolve<IProxyGeneratorFactory>().CreateProxyGenerator(this);
            }

            public bool TryGetCachedValue(System.Type type, out object value)
            {
                return this.cachedValues.TryGetValue(type, out value);
            }

            public void RegisterTriedToResolveType(System.Type type)
            {
                this.attemptedTypes.Add(type);
            }

            public bool TypeHasFailedToResolve(System.Type type)
            {
                return this.attemptedTypes.Contains(type) && !this.cachedValues.ContainsKey(type);
            }

            public void AddResolvedValueToCache(System.Type type, object dummy)
            {
                this.cachedValues.Add(type, dummy);
            }

            public IDummyValueCreator DummyCreator
            {
                get;
                set;
            }

            public IConstructorResolver ConstructorResolver
            {
                get;
                set;
            }

            public IProxyGenerator ProxyGenerator
            {
                get;
                set;
            }
        }

        private class ExpressionCallMatcherFactory
            : IExpressionCallMatcherFactory
        {
            public DictionaryContainer Container;

            public ICallMatcher CreateCallMathcer(System.Linq.Expressions.LambdaExpression callSpecification)
            {
                return new ExpressionCallMatcher(
                    callSpecification,
                    this.Container.Resolve<ArgumentConstraintFactory>(),
                    this.Container.Resolve<MethodInfoManager>());
            }
        }
       
        private class FileSystem : IFileSystem
        {
            public System.IO.Stream Open(string fileName, System.IO.FileMode mode)
            {
                return File.Open(fileName, mode);
            }

            public bool FileExists(string fileName)
            {
                return File.Exists(fileName);
            }

            public void Create(string fileName)
            {
                File.Create(fileName).Dispose();
            }
        }
    }
}
