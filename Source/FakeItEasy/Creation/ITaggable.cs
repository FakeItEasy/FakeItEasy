namespace FakeItEasy.Creation
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents an object that can be tagged with another object. When implemented
    /// by a proxy returned from an <see cref="IProxyGenerator" /> FakeItEasy uses the tag
    /// to store a reference to the <see cref="FakeItEasy.Core.FakeManager"/> that handles that proxy.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Taggable", Justification = "This is the correct spelling as far as I can tell.")]
    public interface ITaggable
    {
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        object Tag { get; set; }
    }
}