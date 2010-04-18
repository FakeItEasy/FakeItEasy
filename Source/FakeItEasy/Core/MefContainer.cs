namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;

    /// <summary>
    /// A IFakeObjectContainer implementation that uses mef to load IFakeDefinitions and
    /// IFakeConfigurations.
    /// </summary>
    public class MefContainer
        : IFakeObjectContainer
    {
        ///// <summary>
        ///// The field for the definitions, assigned by MEF.
        ///// </summary>
        //[ImportMany(typeof(IFakeDefinition))]
        //public IEnumerable<IFakeDefinition> Definitions;

        ///// <summary>
        ///// The field for configurations, assigned by MEF.
        ///// </summary>
        //[ImportMany(typeof(IFakeConfigurator))]
        //public IEnumerable<IFakeConfigurator> ImportedConfigurations;

        //private Dictionary<Type, IFakeDefinition> definitionsByType;
        //private Dictionary<Type, IFakeConfigurator> configurationsByType;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="MefContainer"/> class.
        ///// </summary>
        //public MefContainer()
        //{
        //    this.InitializeImports();
        //    this.definitionsByType = this.Definitions.ToDictionary(x => x.ForType, x => x);
        //    this.configurationsByType = this.ImportedConfigurations.ToDictionary(x => x.ForType, x => x);
        //}

        ///// <summary>
        ///// Initializes the imports.
        ///// </summary>
        //private void InitializeImports()
        //{
        //    var catalogue = new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        //    var batch = new CompositionBatch();
        //    batch.AddPart(this);

        //    var container = new CompositionContainer(catalogue);
        //    container.Compose(batch);
        //}

        ///// <summary>
        ///// Creates a fake object of the specified type using the specified arguments if it's
        ///// supported by the container, returns a value indicating if it's supported or not.
        ///// </summary>
        ///// <param name="typeOfFakeObject">The type of fake object to create.</param>
        ///// <param name="fakeObject">The fake object that was created if the method returns true.</param>
        ///// <returns>True if a fake object can be created.</returns>
        //public bool TryCreateFakeObject(Type typeOfFakeObject, out object fakeObject)
        //{
        //    IFakeDefinition definition;
        //    if (!this.definitionsByType.TryGetValue(typeOfFakeObject, out definition))
        //    {
        //        fakeObject = null;
        //        return false;
        //    }

        //    fakeObject = definition.CreateFake();
        //    return true;
        //}

        ///// <summary>
        ///// Configures the fake.
        ///// </summary>
        ///// <param name="typeOfFakeObject">The type of fake object.</param>
        ///// <param name="fakeObject">The fake object.</param>
        //public void ConfigureFake(Type typeOfFakeObject, object fakeObject)
        //{
        //    IFakeConfigurator configuration;
        //    if (this.configurationsByType.TryGetValue(typeOfFakeObject, out configuration))
        //    {
        //        configuration.ConfigureFake(fakeObject);
        //    }
        //}
        public bool TryCreateFakeObject(Type typeOfFake, out object fakeObject)
        {
            throw new NotImplementedException();
        }

        public void ConfigureFake(Type typeOfFake, object fakeObject)
        {
            throw new NotImplementedException();
        }
    }
}
