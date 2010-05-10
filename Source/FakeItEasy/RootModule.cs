namespace FakeItEasy
{
    using System.IO;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using FakeItEasy.Expressions;
    using FakeItEasy.IoC;
    using FakeItEasy.SelfInitializedFakes;
    using FakeItEasy.Configuration;
    using FakeItEasy.DynamicProxy;
    using System.Linq;
    using System;
    
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
                new DynamicProxyProxyGenerator(c.Resolve<IConstructorResolver>()));

            container.Register<IFakeWrapperConfigurator>(c =>
                new DefaultFakeWrapperConfigurator());

            container.Register<ITypeAccessor>(c =>
                new ApplicationDirectoryAssembliesTypeAccessor());

            container.Register<IConstructorResolver>(c =>
                new DefaultConstructorResolver(c.Resolve<IFakeObjectContainer>()));
        }

        #region FactoryImplementations

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
        #endregion
    }
}
