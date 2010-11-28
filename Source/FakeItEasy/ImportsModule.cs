namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Reflection;
    using IoC;
    using System.Linq;
    using Module = IoC.Module;

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
            //var catalog = new DirectoryCatalog(Path.GetDirectoryName(typeof (ImportsModule).Assembly.Location));
            
            //var container = new CompositionContainer(catalog);

            //try
            //{
            //    container.SatisfyImportsOnce(this);
            //}
            //catch
            //{
            //    this.importedArgumentValueFormatters = Enumerable.Empty<IArgumentValueFormatter>();
            //    this.importedDummyDefinitions = Enumerable.Empty<IDummyDefinition>();
            //    this.importedFakeConfigurators = Enumerable.Empty<IFakeConfigurator>();
            //}
            var assemblyCatalogs = new List<AssemblyCatalog>();

            var directory = Path.GetDirectoryName(typeof (ImportsModule).Assembly.Location);
            
            foreach (var assemblyFile in Directory.EnumerateFiles(directory, "*.dll"))
            {
                try
                {
                    var catalog = new AssemblyCatalog(Assembly.LoadFile(assemblyFile));
                    catalog.Parts.ToArray();
                    assemblyCatalogs.Add(catalog);
                }
                catch
                {
                }
            }

            var aggregateCatalog = new AggregateCatalog(assemblyCatalogs);
            var container = new CompositionContainer(aggregateCatalog);

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