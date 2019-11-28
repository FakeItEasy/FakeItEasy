namespace FakeItEasy.Tools
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class ToolHelpers
    {
        public static string GetCurrentScriptDirectory([CallerFilePath] string path = "") => Path.GetDirectoryName(path)
            ?? throw new Exception("Can't find current script directory.");
    }
}
