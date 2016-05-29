# Source Stepping

In most cases, you'll never need to step through the FakeItEasy source code, but occasionally, it may help to see what FakeItEasy is doing under the hood. FakeItEasy uses [GitLink](https://github.com/GitTools/GitLink) to make it a breeze to step into the FakeItEasy source code.

To enable GitLink to work in Visual Studio:

1. Tools → Options → Debugging → General → Enable Just My Code: **Uncheck**
1. Tools → Options → Debugging → General → Enable source server support: **Check**
1. Tools → Options → Debugging → Symbols → Cache symbols in this directory: **Path to a suitable folder**
