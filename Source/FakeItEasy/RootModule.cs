namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;
    using FakeItEasy.SelfInitializedFakes;

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
            container.Register(c =>
                FakeScope.Current);

            container.Register(c =>
                c.Resolve<FakeScope>().FakeObjectContainer);

            container.RegisterSingleton<IExpressionCallMatcherFactory>(c =>
                new ExpressionCallMatcherFactory
                    {
                        Container = c
                    });

            container.RegisterSingleton(c =>
                new ArgumentConstraintFactory());

            container.RegisterSingleton<ExpressionCallRule.Factory>(c =>
                callSpecification => new ExpressionCallRule(new ExpressionCallMatcher(callSpecification, c.Resolve<ArgumentConstraintFactory>(), c.Resolve<MethodInfoManager>(), c.Resolve<ICallExpressionParser>())));

            container.RegisterSingleton(c =>
                new MethodInfoManager());

            container.Register<FakeAsserter.Factory>(c => x => OrderedAssertion.CurrentAsserterFactory.Invoke(x));

            container.RegisterSingleton<FakeManager.Factory>(c =>
                () => new FakeManager());

            container.RegisterSingleton<IFakeObjectCallFormatter>(c =>
                new DefaultFakeObjectCallFormatter(c.Resolve<ArgumentValueFormatter>(), c.Resolve<IFakeManagerAccessor>()));

            container.RegisterSingleton(c =>
                new ArgumentValueFormatter(c.Resolve<IEnumerable<IArgumentValueFormatter>>()));

            container.RegisterSingleton(c =>
                new CallWriter(c.Resolve<IFakeObjectCallFormatter>(), c.Resolve<IEqualityComparer<IFakeObjectCall>>()));

            container.RegisterSingleton<RecordingManager.Factory>(c =>
                x => new RecordingManager(x));

            container.RegisterSingleton<IFileSystem>(c =>
                new FileSystem());

#if !SILVERLIGHT
            container.RegisterSingleton<FileStorage.Factory>(c =>
                x => new FileStorage(x, c.Resolve<IFileSystem>()));
#endif

            container.RegisterSingleton<ICallExpressionParser>(c =>
                new CallExpressionParser());

            container.RegisterSingleton<IExpressionParser>(c =>
                new ExpressionParser(c.Resolve<ICallExpressionParser>()));

            container.RegisterSingleton<RecordingRuleBuilder.Factory>(c =>
                (rule, fakeObject) => new RecordingRuleBuilder(rule, c.Resolve<RuleBuilder.Factory>().Invoke(rule, fakeObject)));

            container.Register<IFakeCreatorFacade>(c =>
                new DefaultFakeCreatorFacade(c.Resolve<IFakeAndDummyManager>()));

            container.Register<IFakeAndDummyManager>(c =>
                                                         {
                                                             var fakeContainer = c.Resolve<IFakeObjectContainer>();
                                                             var fakeCreator = new FakeObjectCreator(c.Resolve<IProxyGenerator>(), c.Resolve<IExceptionThrower>(), c.Resolve<IFakeManagerAccessor>(), fakeContainer);
                                                             var session = new DummyValueCreationSession(fakeContainer, new SessionFakeObjectCreator { Creator = fakeCreator });

                                                             return new DefaultFakeAndDummyManager(session, fakeCreator, c.Resolve<IFakeWrapperConfigurer>());
                                                         });

            container.RegisterSingleton<IProxyGenerator>(c => new CastleDynamicProxyGenerator());

            container.RegisterSingleton<IExceptionThrower>(c => new DefaultExceptionThrower());

            container.RegisterSingleton<IFakeManagerAccessor>(c => new DefaultFakeManagerAccessor(c.Resolve<FakeManager.Factory>()));

            container.Register<IFakeWrapperConfigurer>(c =>
                new DefaultFakeWrapperConfigurer());

            container.Register(c =>
                new FakeFacade(c.Resolve<IFakeManagerAccessor>(), c.Resolve<IFakeScopeFactory>(), c.Resolve<IFixtureInitializer>()));

            container.RegisterSingleton<IFakeScopeFactory>(c => new FakeScopeFactory());

            container.Register<IFixtureInitializer>(c => new DefaultFixtureInitializer(c.Resolve<IFakeAndDummyManager>()));

            container.RegisterSingleton<IEqualityComparer<IFakeObjectCall>>(c => new FakeCallEqualityComparer());

            container.Register<IInterceptionAsserter>(c => new DefaultInterceptionAsserter());
        }

        private class ExpressionCallMatcherFactory
            : IExpressionCallMatcherFactory
        {
            public DictionaryContainer Container { get; set; }

            public ICallMatcher CreateCallMathcer(LambdaExpression callSpecification)
            {
                return new ExpressionCallMatcher(
                    callSpecification, 
                    this.Container.Resolve<ArgumentConstraintFactory>(), 
                    this.Container.Resolve<MethodInfoManager>(),
                    this.Container.Resolve<ICallExpressionParser>());
            }
        }

        private class FileSystem : IFileSystem
        {
            public Stream Open(string fileName, FileMode mode)
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

        private class SessionFakeObjectCreator
            : IFakeObjectCreator
        {
            public FakeObjectCreator Creator { get; set; }

            public bool TryCreateFakeObject(Type typeOfFake, DummyValueCreationSession session, out object result)
            {
                result = this.Creator.CreateFake(typeOfFake, FakeOptions.Empty, session, false);
                return result != null;
            }
        }
    }
}