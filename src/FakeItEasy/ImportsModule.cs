namespace FakeItEasy
{
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.IoC;

    internal class ImportsModule
        : Module
    {
        public override void RegisterDependencies(DictionaryContainer container)
        {
            var bootstrapper = BootstrapperLocator.FindBootstrapper();

            container.RegisterSingleton<TypeCatalogueInstanceProvider>(c =>
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
        }

        private static void RegisterEnumerableInstantiatedFromTypeCatalogue<T>(DictionaryContainer container)
        {
            container.RegisterSingleton<IEnumerable<T>>(c =>
                    c.Resolve<TypeCatalogueInstanceProvider>().InstantiateAllOfType<T>());
        }
    }
}
