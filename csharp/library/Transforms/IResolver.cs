namespace CSE.ConfigMgmt.Resolvers;

using FluentResults;

/// <summary>
/// Interface for a configuration value resolver. A resolver takes a value and converts it into another value. This is most
/// commonly used to resolve a secret name into a secret value.
/// </summary>
/// <typeparam name="T">The type of value to resolve.</typeparam>
public interface IResolver<T>
{
    /// <summary>
    /// Attempts to resolve the specified value.
    /// </summary>
    /// <param name="value">The value to resolve.</param>
    /// <returns>The resolved value wrapped in a Result.</returns>
    public Result<T> Resolve(T value);
}