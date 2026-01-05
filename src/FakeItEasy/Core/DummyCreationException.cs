namespace FakeItEasy.Core;

using System;

/// <summary>
/// An exception that is thrown when there was an error creating a Dummy.
/// </summary>
public class DummyCreationException
    : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DummyCreationException"/> class.
    /// </summary>
    public DummyCreationException()
        : base(ExceptionMessages.DummyCreationExceptionDefault)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyCreationException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public DummyCreationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyCreationException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public DummyCreationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
