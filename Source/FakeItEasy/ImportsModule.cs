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
        public IEnumerable<IArgumentValueFormatter> ImportedArgumentValueFormatters;
        [ImportMany(typeof(IDummyDefinition))]
        public IEnumerable<IDummyDefinition> ImportedDummyDefinitions;
        [ImportMany(typeof(IFakeConfigurator))]
        public IEnumerable<IFakeConfigurator> ImportedFakeConfigurators;

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
            container.RegisterSingleton(c => this.ImportedArgumentValueFormatters);
            container.RegisterSingleton(c => this.ImportedDummyDefinitions);
            container.RegisterSingleton(c => this.ImportedFakeConfigurators);
        }
    }
}