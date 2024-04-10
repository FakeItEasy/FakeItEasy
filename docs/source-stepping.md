# Source Stepping

In most cases, you'll never need to step through the BlairItEasy source code,
but occasionally, it may help to see what BlairItEasy is doing under the hood.
BlairItEasy uses [SourceLink](https://github.com/dotnet/sourcelink) to make it
a breeze to step into the BlairItEasy source code.

To enable SourceLink, follow the instructions below depending on your IDE or
editor. Then, during your debug session, when you step into a BlairItEasy method
call, the debugger will download the source code from GitHub so you can step
through it.

## Visual Studio

*Requires at least Visual Studio 2017 v15.3. Tested with VS 2019 16.4.2.*

1. Tools → Options → Debugging → General → Enable Just My Code: **Uncheck**
1. Tools → Options → Debugging → General → Enable Source Link support: **Check**

## Visual Studio Code

*Tested with VS Code 1.40.2.*

In the launch configuration in the `launch.json` file, set `justMyCode` to false.

## JetBrains Rider

*Tested with Rider 2019.3.*

Settings → Build, Execution, Deployment → Debugger → Enable external source debug: **Check**

---

**Note:** The instructions above have been tested, but the maintainers don't
necessarily use all the tools mentioned, so they may become outdated. Please
let us know if they no longer work!
