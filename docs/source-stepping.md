# Source Stepping

In most cases, you'll never need to step through the FakeItEasy source code,
but occasionally, it may help to see what FakeItEasy is doing under the hood.
FakeItEasy uses [SourceLink](https://github.com/dotnet/sourcelink) to make it
a breeze to step into the FakeItEasy source code.

To enable SourceLink to work in Visual Studio:

1. Tools → Options → Debugging → General → Enable Just My Code: **Uncheck**
1. Tools → Options → Debugging → General → Enable Source Link support: **Check**

Then, during your debug session, when you step into a FakeItEasy method call,
the debugger will download the source code from GitHub so you can step through
it.
