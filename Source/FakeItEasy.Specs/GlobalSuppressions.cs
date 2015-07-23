[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Reliability",
    "CA2001:AvoidCallingProblematicMethods",
    MessageId = "System.GC.Collect",
    Scope = "member",
    Target = "FakeItEasy.Specs.when_faking_a_disposable_class.#.ctor()",
    Justification = "Required for testing.")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Design",
    "CA1045:DoNotPassTypesByReference",
    MessageId = "0#",
    Scope = "member",
    Target = "FakeItEasy.Specs.when_matching_a_call_with_a_ref_parameter+IHaveInterestingParameters.#CheckYourReferences(System.String&)",
    Justification = "Required for testing.")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Design",
    "CA1021:AvoidOutParameters",
    MessageId = "0#",
    Scope = "member",
    Target = "FakeItEasy.Specs.when_matching_a_call_with_a_parameter_having_an_out_attribute+IHaveInterestingParameters.#Validate(System.String)",
    Justification = "Required for testing.")]

// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.