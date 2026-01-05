namespace FakeItEasy.Core;

using System;

/// <summary>
/// An exception that is thrown when there was an error creating a fake object.
/// </summary>
public class FakeCreationException
    : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeCreationException"/> class.
    /// </summary>
    public FakeCreationException()
        : base(ExceptionMessages.FakeCreationExceptionDefault)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeCreationException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public FakeCreationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeCreationException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public FakeCreationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
