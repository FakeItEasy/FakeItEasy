namespace FakeItEasy
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using IoC;

    internal class ImportsModule
        : Module
    {
        [ImportMany(typeof(IArgumentValueFormatter))]
        private IEnumerable<IArgumentValueFormatter> importedArgumentValueFormatters;
        [ImportMany(typeof(IDummyDefinition))]
        private IEnumerable<IDummyDefinition> importedDummyDefinitions;
        [ImportMany(typeof(IFakeConfigurator))]
        private IEnumerable<IFakeConfigurator> importedFakeConfigurators;

        public ImportsModule()
        {
            this.SatisfyImports();
        }

        private void SatisfyImports()
        {
            var catalog = new DirectoryCatalog(Path.GetDirectoryName(typeof (ImportsModule).Assembly.Location));
            var container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);
        }

        public override void RegisterDependencies(DictionaryContainer container)
        {
            container.RegisterSingleton(c => this.importedArgumentValueFormatters);
            container.RegisterSingleton(c => this.importedDummyDefinitions);
            container.RegisterSingleton(c => this.importedFakeConfigurators);
        }
    }
}