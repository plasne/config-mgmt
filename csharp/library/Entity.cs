namespace CSE.ConfigMgmt;

using System;
using System.Collections.Generic;
using System.Linq;
using CSE.ConfigMgmt.Getters;
using CSE.ConfigMgmt.Resolvers;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// An entity is a wrapper that resolves to a configuration value.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public class Entity<T> : IEntity
{
    private readonly string key;
    private readonly IServiceProvider serviceProvider;
    private List<T> values;
    private ILogger<Config> logger;
    private T dflt = default;
    private bool isRequired = false;
    private Func<string, Result<T>> transform;

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{T}"/> class. This is provided to force an exception;
    /// the entity must be created from Config.
    /// </summary>
    public Entity()
    {
        throw new Exception("Entity must be created from Config.");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{T}"/> class.
    /// </summary>
    /// <param name="key">The unique key identifying the entity.</param>
    /// <param name="serviceProvider">The service provider.</param>
    internal Entity(string key, IServiceProvider serviceProvider)
    {
        this.key = key;
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public string Key => this.key;

    /// <inheritdoc />
    public bool HasValue => this.values is not null && this.values.Count > 0;

    /// <inheritdoc />
    public bool IsRequired => this.isRequired;

    /// <summary>
    /// Gets the value of the entity or its default.
    /// </summary>
    public T Value
    {
        get
        {
            return this.HasValue
                ? this.values.First()
                : this.dflt;
        }
    }

    /// <summary>
    /// Gets or sets the values of the entity.
    /// </summary>
    internal List<T> Values
    {
        get => this.values;
        set => this.values = value;
    }

    /// <summary>
    /// This method attempts to get a value for the entity by checking all the IGetter objects in the service collection.
    /// </summary>
    /// <param name="key">The optional key to use; otherwise the entity key is used.</param>
    /// <returns>The entity.</returns>
    public Entity<T> TryGet(string key = null)
    {
        foreach (var getter in this.serviceProvider.GetServices<IGetter>())
        {
            var result = getter.TryGet(key ?? this.key);
            if (result.IsSuccess)
            {
                this.TrySetValue(result.Value);
            }
        }

        return this;
    }

    /// <summary>
    /// This method attempts to set a value for the entity given string input.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <returns>The entity.</returns>
    public Entity<T> TrySet(string value)
    {
        this.TrySetValue(value);
        return this;
    }

    /// <summary>
    /// This method sets a default value to use if one is not found via sets or gets.
    /// </summary>
    /// <param name="value">The default value.</param>
    /// <returns>The entity.</returns>
    public Entity<T> Default(T value)
    {
        this.dflt = value;
        return this;
    }

    /// <summary>
    /// This method prints the value of the entity to the console.
    /// </summary>
    /// <param name="isSecret">True if the value is a secret; otherwise false.</param>
    /// <returns>The entity.</returns>
    public Entity<T> Print(bool isSecret = false)
    {
        Console.WriteLine("{0} = '{1}'", this.key, this.GetValueAsString(isSecret));
        return this;
    }

    /// <summary>
    /// This method logs the value of the entity to the logger.
    /// </summary>
    /// <param name="isSecret">True if the value is a secret; otherwise false.</param>
    /// <returns>The entity.</returns>
    public Entity<T> Log(bool isSecret = false)
    {
        this.logger ??= this.serviceProvider.GetService<ILogger<Config>>();
        if (this.logger is not null)
        {
            this.logger.LogInformation("{key} = '{value}'", this.key, this.GetValueAsString(isSecret));
        }
        else
        {
            this.Print(isSecret);
        }

        return this;
    }

    /// <summary>
    /// This method sets the entity as required. If Validate() is called on the Config, an exception will be thrown if
    /// the entity does not have a value. Defaults are ignored, so there is no legitimate case for setting Default() and
    /// Require() on the same entity.
    /// </summary>
    /// <returns>The entity.</returns>
    public Entity<T> Require()
    {
        this.isRequired = true;
        return this;
    }

    /// <summary>
    /// This method defines the function that determines how the value is transformed from a string to the proper datatype.
    /// This can also be used for mapping or validation.
    /// </summary>
    /// <param name="func">The function to add.</param>
    /// <returns>The entity.</returns>
    public Entity<T> SetTransform(Func<string, Result<T>> func)
    {
        this.transform = func;
        return this;
    }

    /// <summary>
    /// This method uses IResolver{T} objects in the service collection to resolve the value of the entity. This can be
    /// used to resolve a value from a secret store, for example.
    /// </summary>
    /// <returns>The entity.</returns>
    public Entity<T> Resolve()
    {
        foreach (var resolver in this.serviceProvider.GetServices<IResolver<T>>())
        {
            this.values = this.values?
                .Select(resolver.Resolve)
                .Where(result => result.IsSuccess)
                .Select(result => result.Value)
                .ToList();
        }

        return this;
    }

    private string GetValueAsString(bool isSecret)
    {
        if (isSecret && this.HasValue)
        {
            return "(set)";
        }
        else if (isSecret)
        {
            return "(not set)";
        }
        else if (typeof(T) == typeof(string[]) && this.HasValue)
        {
            return string.Join(", ", this.Value as string[]);
        }
        else if (typeof(T).IsEnum && !this.HasValue)
        {
            return "(not set)";
        }
        else if (this.HasValue)
        {
            return this.Value.ToString();
        }
        else
        {
            return this.dflt?.ToString();
        }
    }

    private bool TrySetValue(string value)
    {
        // use the transform function if provided
        if (this.transform is not null)
        {
            var result = this.transform(value);
            if (result.IsSuccess)
            {
                this.values ??= new();
                this.values.Add(result.Value);
                return true;
            }

            return false;
        }

        // try to convert to the desired type using standard functionality
        if (typeof(T) == typeof(string) && !string.IsNullOrWhiteSpace(value))
        {
            this.values ??= new();
            this.values.Add((T)(object)value);
            return true;
        }
        else if (typeof(T) == typeof(int) && int.TryParse(value, out var intVal))
        {
            this.values ??= new();
            this.values.Add((T)(object)intVal);
            return true;
        }
        else if (typeof(T) == typeof(decimal) && decimal.TryParse(value, out var decVal))
        {
            this.values ??= new();
            this.values.Add((T)(object)decVal);
            return true;
        }
        else if (typeof(T) == typeof(bool) && value.TryParseAsBool(out var boolVal))
        {
            this.values ??= new();
            this.values.Add((T)(object)boolVal);
            return true;
        }
        else if (typeof(T) == typeof(string[]) && !string.IsNullOrWhiteSpace(value))
        {
            var values = value
                .Split(',', options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToArray();

            if (values.Any())
            {
                this.values ??= new();
                this.values.Add((T)(object)values);
            }

            return true;
        }
        else if (typeof(T).IsEnum && Enum.TryParse(typeof(T), value, true, out var enumVal))
        {
            this.values ??= new();
            this.values.Add((T)(object)enumVal);
            return true;
        }

        return false;
    }
}
