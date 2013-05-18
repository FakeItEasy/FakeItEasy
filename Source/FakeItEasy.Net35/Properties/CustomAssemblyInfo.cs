using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage(
    "Microsoft.Design",
    "CA1020:AvoidNamespacesWithFewTypes",
    Scope = "namespace",
    Target = "System",
    Justification = "Only emulating the minimum net40 types required.")]

[module: SuppressMessage(
    "Microsoft.Design",
    "CA1020:AvoidNamespacesWithFewTypes",
    Scope = "namespace",
    Target = "System.ComponentModel.Composition",
    Justification = "Only emulating the minimum net40 types required.")]

[module: SuppressMessage(
    "Microsoft.Design",
    "CA1020:AvoidNamespacesWithFewTypes",
    Scope = "namespace",
    Target = "System.Linq",
    Justification = "Only emulating the minimum net40 types required.")]
