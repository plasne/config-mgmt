namespace CSE.ConfigMgmt;

/// <summary>
/// Interface for a configuration entity. The entity is just a helper that can chain methods to ultimately arrive
/// at a configuration value.
/// </summary>
internal interface IEntity
{
    /// <summary>
    /// Gets the key that should uniquely identify the entity.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets a value indicating whether the entity holds at least one value.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// Gets a value indicating whether the entity requires a value.
    /// </summary>
    public bool IsRequired { get; }
}