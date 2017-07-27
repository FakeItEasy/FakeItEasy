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

| Id             | Summary                                  | Description                                                                                                                                           |
|----------------|------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------|
| FakeItEasy0001 | Unused call specification                | Triggered when you specify a call but don't configure or assert it, making it a no-op.                                                                |
| FakeItEasy0002 | Non-virtual member configuration         | Triggered when you try to configure a non-virtual member, which cannot be faked.                                                                      |
| FakeItEasy0003 | Argument constraint outside call spec    | Triggered when you try to use an [argument constraint](argument-constraints.md) outside of a [call specification](specifying-a-call-to-configure.md). |
| FakeItEasy0004 | Argument constraint nullability mismatch | Triggered when you use a non-nullable [argument constraint](argument-constraints.md) for a nullable parameter. Calls where the argument is null won't be matched. If this is intentional, consider using `A<T?>.That.IsNotNull()` instead. If it's not, make the argument constraint nullable (`A<T?>`). |
