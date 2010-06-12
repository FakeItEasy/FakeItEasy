using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Fake it easy")]
[assembly: AssemblyDescription("The FakeItEasy dynamic fake framework.")]


// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("2622059a-d73a-4066-9d7f-eb545a79ca5e")]

// Module level suppress messages.
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Justification = "Contains several internal types.", Scope = "namespace", Target = "FakeItEasy.Expressions")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Justification = "These types are only used by VB and should not confuse other developers.", Scope = "namespace", Target = "FakeItEasy.VisualBasic")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Justification = "Is in a namespace of its own to allow for just using the extensions when they are explicitly requested.", Scope = "namespace", Target = "FakeItEasy.ExtensionSyntax")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "FakeItEasy.ExtensionSyntax.Full", Justification = "Is in a namespace of its own to allow for just using the extensions when they are explicitly requested.")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "FakeItEasy.DynamicProxy", Justification = "Should not be mixed with other types.")]