namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using IoC;
    using System.Linq;
    using Module = IoC.Module;

    internal class ImportsModule
        : Module
    {
        [ImportMany(typeof (IArgumentValueFormatter))]
        public IEnumerable<IArgumentValueFormatter> ImportedArgumentValueFormatters { get; set; }

        [ImportMany(typeof(IDummyDefinition))]
        public IEnumerable<IDummyDefinition> ImportedDummyDefinitions { get; set; }

        [ImportMany(typeof(IFakeConfigurator))]
        public IEnumerable<IFakeConfigurator> ImportedFakeConfigurators { get; set; }

        public ImportsModule()
        {
            this.SatisfyImports();
        }


        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should just ignore importing of add-ins that explode.")]
        private void SatisfyImports()
        {
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