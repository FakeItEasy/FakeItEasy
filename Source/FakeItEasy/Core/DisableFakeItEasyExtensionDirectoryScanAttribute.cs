namespace FakeItEasy.Core
{
    using System;

    /// <summary>
    /// Add this attribute any assembly that references FakeItEasy to disable complete directory scan on startup.
    /// This may prevent IArgumentValueFormatter, IDummyDefinition, IFakeConfigurator configurations from working.
    /// If you don't know of these interfaces, it's probably fine to disable the directory scan without ill effects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class DisableFakeItEasyExtensionDirectoryScanAttribute : Attribute
    {
    }
}