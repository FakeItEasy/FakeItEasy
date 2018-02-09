# Analyzer

FakeItEasy provides a Roslyn analyzer to detect incorrect usages of the library
that cannot be prevented by the API or the compiler but will typically result in
bugs at runtime.

## Installation

The analyzer works in Visual Studio 2015 Update 1 or later. It can be installed
[from NuGet](https://www.nuget.org/packages/FakeItEasy.Analyzer) for each project
that needs it:

```
PM> Install-Package FakeItEasy.Analyzer
```

## Diagnostics

The analyzer currently provides the following diagnostics:

| Id             | Summary                                  | Code Fix? | Description                                                                                                                                           |
|----------------|------------------------------------------|-----------|-------------------------------------------------------------------------------------------------------------------------------------------------------|
| FakeItEasy0001 | Unused call specification                | no        | Triggered when you specify a call but don't configure or assert it, making it a no-op.                                                                |
| FakeItEasy0002 | Non-virtual member configuration         | no        | Triggered when you try to configure a non-virtual member, which cannot be faked.                                                                      |
| FakeItEasy0003 | Argument constraint outside call spec    | no        | Triggered when you try to use an [argument constraint](argument-constraints.md) outside of a [call specification](specifying-a-call-to-configure.md). |
| FakeItEasy0004 | Argument constraint nullability mismatch | yes       | Triggered when you use a non-nullable [argument constraint](argument-constraints.md) for a nullable parameter. Calls where the argument is null won't be matched. If this is intentional, consider using `A<T?>.That.IsNotNull()` instead. If it's not, make the argument constraint nullable (`A<T?>`). |
| FakeItEasy0005 | Argument constraint type mismatch        | yes       | Triggered when you use an [argument constraint](argument-constraints.md) whose type doesn't match the parameter. No such calls can be matched.        |
| FakeItEasy0006 | Assertion uses legacy Repeated class     | yes       | Triggered when you make an assertion using the [legacy `MustHaveHappened(Repeated)` overload](assertion.md#using-the-legacy-musthavehappenedrepeated-overload).|                                                         |
