namespace CSE.ConfigMgmt;

using System;
using System.Collections.Generic;
using System.Linq;
using CSE.ConfigMgmt.Getters;
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
    private ILogger<Config> logger;
    private List<string> values;
    private bool isDirty = true;
    private T cached = default;
    private T dflt = default;
    private bool isRequired = false;
    private Func<string, Result<T>> convert;
    private List<Func<T, Result>> validate;
    private List<Func<T, Result<T>>> transform;

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
            if (!this.isDirty)
            {
                return this.cached;
            }

            var converted = this.Converted();
            var validated = this.Validated(converted);
            var transformed = this.Transformed(validated);
            this.cached = transformed
                .DefaultIfEmpty(this.dflt)
                .First();

            this.isDirty = false;
            return this.cached;
        }
    }

    /// <summary>
    /// This method gets values for the key from all the IGetter objects in the service collection.
    /// </summary>
    /// <param name="key">The optional key to use; otherwise the entity key is used.</param>
    /// <returns>The entity.</returns>
    public Entity<T> Get(string key = null)
    {
        this.isDirty = true;

        foreach (var getter in this.serviceProvider.GetServices<IGetter>())
        {
            var result = getter.TryGet(key ?? this.key);
            if (result.IsSuccess)
            {
                this.values ??= new();
                this.values.Add(result.Value);
            }
        }

        return this;
    }

    /// <summary>
    /// This method sets a value for the entity given string input. As with all values, it will be used only if it passes
    /// transformation, validation, etc.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <returns>The entity.</returns>
    public Entity<T> Set(string value)
    {
        this.isDirty = true;
        this.values ??= new();
        this.values.Add(value);
        return this;
    }

    /// <summary>
    /// This method sets a default value to use if one is not found via sets or gets.
    /// </summary>
    /// <param name="value">The default value.</param>
    /// <returns>The entity.</returns>
    public Entity<T> Default(T value)
    {
        this.isDirty = true;
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
    /// This method defines the function that converts the string value to the entity type.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The entity.</returns>
    public Entity<T> Convert(Func<string, Result<T>> func)
    {
        this.isDirty = true;
        this.convert = func;
        return this;
    }

    /// <summary>
    /// This method adds a function that validates the entity value. All validation functions must pass for a value
    /// to be considered valid.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The entity.</returns>
    public Entity<T> Validate(Func<T, Result> func)
    {
        this.isDirty = true;
        this.validate ??= new();
        this.validate.Add(func);
        return this;
    }

    /// <summary>
    /// This method adds a function that transforms the entity value. All transform functions are applied in the order
    /// they are added. No validation functions have to pass for a value to be valid.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The entity.</returns>
    public Entity<T> Transform(Func<T, Result<T>> func)
    {
        this.isDirty = true;
        this.transform ??= new();
        this.transform.Add(func);
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

    private List<T> Converted()
    {
        var output = new List<T>();

        // consider all string values
        foreach (var value in this.values)
        {
            // use the function if provided
            if (this.convert is not null)
            {
                var result = this.convert(value);
                if (result.IsSuccess)
                {
                    output.Add(result.Value);
                }

                continue;
            }

            // try to convert to the desired type using standard functionality
            if (typeof(T) == typeof(string) && !string.IsNullOrWhiteSpace(value))
            {
                output.Add((T)(object)value);
            }
            else if (typeof(T) == typeof(int) && int.TryParse(value, out var intVal))
            {
                output.Add((T)(object)intVal);
            }
            else if (typeof(T) == typeof(decimal) && decimal.TryParse(value, out var decVal))
            {
                output.Add((T)(object)decVal);
            }
            else if (typeof(T) == typeof(bool) && value.TryParseAsBool(out var boolVal))
            {
                output.Add((T)(object)boolVal);
            }
            else if (typeof(T) == typeof(string[]) && !string.IsNullOrWhiteSpace(value))
            {
                var values = value
                    .Split(',', options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToArray();

                if (values.Any())
                {
                    output.Add((T)(object)values);
                }
            }
            else if (typeof(T).IsEnum && Enum.TryParse(typeof(T), value, true, out var enumVal))
            {
                output.Add((T)(object)enumVal);
            }
        }

        return output;
    }

    private List<T> Validated(List<T> values)
    {
        if (this.validate is null)
        {
            return values;
        }

        // all validations must succeed
        return values.Where(x => this.validate.All(y => y(x).IsSuccess)).ToList();
    }

    private List<T> Transformed(List<T> values)
    {
        if (this.transform is null)
        {
            return values;
        }

        // none of the transforms are required to succeed
        return values.Select(x =>
        {
            foreach (var transform in this.transform)
            {
                var result = transform(x);
                if (result.IsSuccess)
                {
                    x = result.Value;
                }
            }

            return x;
        }).ToList();
    }
}
