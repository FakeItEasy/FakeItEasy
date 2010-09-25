namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        private static readonly Logger logger = Log.GetLogger<RootModule>();

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

            container.Register<FakeAsserter.Factory>(c => x => OrderedAssertion.CurrentAsserterFactory.Invoke(x));

            container.RegisterSingleton<FakeManager.Factory>(c =>
                () => new FakeManager());

            container.RegisterSingleton<IFakeObjectCallFormatter>(c =>
                new DefaultFakeObjectCallFormatter());

            container.RegisterSingleton<CallWriter>(c =>
                new CallWriter(c.Resolve<IFakeObjectCallFormatter>()));

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

            container.Register<IFakeCreatorFacade>(c =>
                new DefaultFakeCreatorFacade(c.Resolve<IFakeAndDummyManager>()));

            container.Register<IFakeAndDummyManager>(c =>
                {
                    logger.Debug("Creating new fake and dummy manager.");

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

            container.Register<ITypeCatalogue>(c =>
                new ApplicationDirectoryAssembliesTypeAccessor());
        }

        private class SessionFakeObjectCreator
            : IFakeObjectCreator
        {
            public FakeObjectCreator Creator;

            public bool TryCreateFakeObject(Type typeOfFake, DummyValueCreationSession session, out object result)
            {
                result = this.Creator.CreateFake(typeOfFake, FakeOptions.Empty, session, false);
                return result != null;
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
