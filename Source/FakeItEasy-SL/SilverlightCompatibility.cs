namespace FakeItEasy
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using IoC;

    [Conditional("Near_a_tree_by_a_river_theres_a_hole_in_the_ground")]
    public class SerializableAttribute
        : Attribute
    {
    }

    [Conditional("Near_a_tree_by_a_river_theres_a_hole_in_the_ground")]
    public class NonSerializedAttribute
        : Attribute
    {
    }

    internal class ImportsModule : Module
    {
        public override void RegisterDependencies(DictionaryContainer container)
        {
            container.RegisterSingleton(c => Enumerable.Empty<IFakeConfigurator>());
            container.RegisterSingleton(c => Enumerable.Empty<IDummyDefinition>());
            container.RegisterSingleton(c => Enumerable.Empty<IArgumentValueFormatter>());
        }
    }
}

namespace System.ComponentModel.Composition
{
    public class InheritedExportAttribute
        : Attribute
    {
    }

    public class ImportManyAttribute
        : Attribute
    {
    }
}