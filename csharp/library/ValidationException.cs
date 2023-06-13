namespace CSE.ConfigMgmt;

using System;

/// <summary>
/// An exception thrown if a partition ID is not an integer between 0 and 31.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    public ValidationException(string key)
        : base($"configuration key '{key}' is required, but missing.")
    {
    }
}