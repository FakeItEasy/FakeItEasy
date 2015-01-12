using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyVersion("2.0.0")]
[assembly: AssemblyFileVersion("2.0.0")]
[assembly: AssemblyInformationalVersion("2.0.0")]

[assembly: AssemblyCompany("Patrik Hägne")]
[assembly: AssemblyProduct("FakeItEasy")]
[assembly: AssemblyCopyright("Copyright (c) FakeItEasy contributors. (fakeiteasy@hagne.se)")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

[assembly: AssemblyTitle("FakeItEasy")]
[assembly: AssemblyDescription("The FakeItEasy dynamic fake framework.")]

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]
[assembly: InternalsVisibleTo("FakeItEasy.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001002f50d82092713be482c885861c984334606da0f54c78738a3dd0862fb2dfc6080f0780132cc65d88f0f0c70af74e8a53430962395bfc1a36fab08b7a2549d387e805c13cc84acd884447ec8c4dcfb6216df720f0998380f9c906b5de8141798d64661f036d47274e6ecb76c9cde5f4cf2b521040601e44b3914fbeb9f39127f9")]
[assembly: InternalsVisibleTo("FakeItEasy.IntegrationTests, PublicKey=00240000048000009400000006020000002400005253413100040000010001002f50d82092713be482c885861c984334606da0f54c78738a3dd0862fb2dfc6080f0780132cc65d88f0f0c70af74e8a53430962395bfc1a36fab08b7a2549d387e805c13cc84acd884447ec8c4dcfb6216df720f0998380f9c906b5de8141798d64661f036d47274e6ecb76c9cde5f4cf2b521040601e44b3914fbeb9f39127f9")]

// Module level suppress messages.
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Justification = "Contains several internal types.", Scope = "namespace", Target = "FakeItEasy.Creation")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Justification = "Contains several internal types.", Scope = "namespace", Target = "FakeItEasy.Expressions")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Justification = "These types are only used by VB and should not confuse other developers.", Scope = "namespace", Target = "FakeItEasy.VisualBasic")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Justification = "Is in a namespace of its own to allow for just using the extensions when they are explicitly requested.", Scope = "namespace", Target = "FakeItEasy.ExtensionSyntax")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "FakeItEasy.ExtensionSyntax.Full", Justification = "Is in a namespace of its own to allow for just using the extensions when they are explicitly requested.")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "FakeItEasy.Creation.CastleDynamicProxy", Justification = "Should not be mixed with other types.")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "type", Target = "FakeItEasy.IFakeConfigurator", MessageId = "Configurator", Justification = "This is the correct spelling.")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "type", Target = "FakeItEasy.Core.IFakeObjectConfigurator", MessageId = "Configurator", Justification = "This is the correct spelling.")]
[assembly: SuppressMessage("Microsoft.Usage", "CA2243:AttributeStringLiteralsShouldParseCorrectly", Justification = "AssemblyInformationalVersion uses SemVer.")]
