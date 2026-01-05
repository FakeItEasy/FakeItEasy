namespace FakeItEasy;

using System;

/// <summary>
/// An exception thrown when a user-provided callback throws an exception.
/// </summary>
public class UserCallbackException
    : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCallbackException"/> class.
    /// </summary>
    public UserCallbackException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserCallbackException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public UserCallbackException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserCallbackException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public UserCallbackException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
