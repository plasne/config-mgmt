namespace CSE.ConfigMgmt;

using System;

/// <summary>
/// Interface for the configuration system.
/// </summary>
public interface IConfig
{
    /// <summary>
    /// Creates a new configuration entity that will resolve to a string.
    /// </summary>
    /// <param name="key">The key to get.</param>
    /// <returns>The configuration entity.</returns>
    public Entity<string> AsString(string key);

    /// <summary>
    /// Creates a new configuration entity that will resolve to an integer.
    /// </summary>
    /// <param name="key">The key to get.</param>
    /// <returns>The configuration entity.</returns>
    public Entity<int> AsInteger(string key);

    /// <summary>
    /// Creates a new configuration entity that will resolve to a rational number.
    /// </summary>
    /// <param name="key">The key to get.</param>
    /// <returns>The configuration entity.</returns>
    public Entity<decimal> AsRational(string key);

    /// <summary>
    /// Creates a new configuration entity that will resolve to a boolean.
    /// </summary>
    /// <param name="key">The key to get.</param>
    /// <returns>The configuration entity.</returns>
    public Entity<bool> AsBoolean(string key);

    /// <summary>
    /// Creates a new configuration entity that will resolve to an array of strings.
    /// </summary>
    /// <param name="key">The key to get.</param>
    /// <returns>The configuration entity.</returns>
    public Entity<string[]> AsStrings(string key);

    /// <summary>
    /// Creates a new configuration entity that will resolve to an enumerable value.
    /// </summary>
    /// <typeparam name="T">The type of enumerable to resolve.</typeparam>
    /// <param name="key">The key to get.</param>
    /// <returns>The configuration entity.</returns>
    public Entity<T> AsEnum<T>(string key)
    where T : Enum;

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    public void Validate();
}