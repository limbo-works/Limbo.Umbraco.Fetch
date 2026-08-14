using System;

namespace Limbo.Umbraco.Fetch.Exceptions;

/// <summary>
/// Represents an error that occurs in the <strong>Fetch</strong> package.
/// </summary>
public class FetchException : Exception {

    /// <summary>
    /// Initializes a new instance of the <see cref="FetchException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public FetchException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FetchException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    public FetchException(string message, Exception? innerException) : base(message, innerException) { }

}