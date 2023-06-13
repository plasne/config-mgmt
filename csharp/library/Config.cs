namespace CSE.ConfigMgmt;

using System;
using System.Collections.Generic;

/// <summary>
/// An implementation of the configuration system.
/// </summary>
public class Config : IConfig
{
    private readonly IServiceProvider serviceProvider;
    private readonly List<IEntity> entities = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Config"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public Config(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public Entity<string> AsString(string key)
    {
        var entity = new Entity<string>(key, this.serviceProvider);
        this.entities.Add(entity);
        return entity;
    }

    /// <inheritdoc />
    public Entity<int> AsInteger(string key)
    {
        var entity = new Entity<int>(key, this.serviceProvider);
        this.entities.Add(entity);
        return entity;
    }

    /// <inheritdoc />
    public Entity<decimal> AsRational(string key)
    {
        var entity = new Entity<decimal>(key, this.serviceProvider);
        this.entities.Add(entity);
        return entity;
    }

    /// <inheritdoc />
    public Entity<bool> AsBoolean(string key)
    {
        var entity = new Entity<bool>(key, this.serviceProvider);
        this.entities.Add(entity);
        return entity;
    }

    /// <inheritdoc />
    public Entity<string[]> AsStrings(string key)
    {
        var entity = new Entity<string[]>(key, this.serviceProvider);
        this.entities.Add(entity);
        return entity;
    }

    /// <inheritdoc />
    public Entity<T> AsEnum<T>(string key)
    where T : Enum
    {
        var entity = new Entity<T>(key, this.serviceProvider);
        this.entities.Add(entity);
        return entity;
    }

    /// <inheritdoc />
    public void Validate()
    {
        foreach (var entity in this.entities)
        {
            if (entity.IsRequired && !entity.HasValue)
            {
                throw new ValidationException(entity.Key);
            }
        }
    }
}
