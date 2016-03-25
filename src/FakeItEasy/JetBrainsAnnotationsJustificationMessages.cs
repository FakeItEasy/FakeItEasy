namespace FakeItEasy
{
    using JetBrains.Annotations;

    /// <summary>
    /// Common justification messages for JetBrains ReSharper annotations.
    /// </summary>
    public static class JetBrainsAnnotationsJustificationMessages
    {
        /// <summary>
        /// Justification message for call specifications' <see cref="MustUseReturnValueAttribute"/> attributes.
        /// </summary>
        public const string CallSpecificationMustUseReturnValueJustification = "Unused call specification; did you forget to configure or assert the call?";
    }
}