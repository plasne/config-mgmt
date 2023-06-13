namespace CSE.ConfigMgmt.Getters;

using FluentResults;

/// <summary>
/// Interface for a configuration value getter.
/// </summary>
public interface IGetter
{
    /// <summary>
    /// Attempts to get the value for the specified key.
    /// </summary>
    /// <param name="key">The key to get.</param>
    /// <returns>The value for the specified key wrapped in a Result.</returns>
    public Result<string> TryGet(string key);
}