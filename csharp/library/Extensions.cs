namespace CSE.ConfigMgmt;

using System;
using System.Linq;
using CSE.ConfigMgmt.Getters;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds configuration management to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="types">The getter types to add.</param>
    /// <exception cref="ArgumentException">Thrown when a type does not implement IGetter.</exception>
    public static void AddConfigMgmt(this IServiceCollection services, params Type[] types)
    {
        // add getters
        foreach (var type in types)
        {
            if (typeof(IGetter).IsAssignableFrom(type))
            {
                services.AddSingleton(typeof(IGetter), type);
            }
            else
            {
                throw new ArgumentException($"Type {type.Name} does not implement IGetter interface.");
            }
        }

        // add config
        services.AddSingleton<IConfig, Config>();
    }

    /// <summary>
    /// Adds configuration management to the service collection.
    /// </summary>
    /// <typeparam name="TGetter0">The type of the 1st getter to add.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <exception cref="ArgumentException">Thrown when a type does not implement IGetter.</exception>
    public static void AddConfigMgmt<TGetter0>(this IServiceCollection services)
        where TGetter0 : IGetter
    {
        services.AddConfigMgmt(typeof(TGetter0));
    }

    /// <summary>
    /// Adds configuration management to the service collection.
    /// </summary>
    /// <typeparam name="TGetter0">The type of the 1st getter to add.</typeparam>
    /// <typeparam name="TGetter1">The type of the 2nd getter to add.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <exception cref="ArgumentException">Thrown when a type does not implement IGetter.</exception>
    public static void AddConfigMgmt<TGetter0, TGetter1>(this IServiceCollection services)
    where TGetter0 : IGetter
    where TGetter1 : IGetter
    {
        services.AddConfigMgmt(typeof(TGetter0), typeof(TGetter1));
    }

    /// <summary>
    /// Adds configuration management to the service collection.
    /// </summary>
    /// <typeparam name="TGetter0">The type of the 1st getter to add.</typeparam>
    /// <typeparam name="TGetter1">The type of the 2nd getter to add.</typeparam>
    /// <typeparam name="TGetter2">The type of the 3rd getter to add.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <exception cref="ArgumentException">Thrown when a type does not implement IGetter.</exception>
    public static void AddConfigMgmt<TGetter0, TGetter1, TGetter2>(this IServiceCollection services)
    where TGetter0 : IGetter
    where TGetter1 : IGetter
    where TGetter2 : IGetter
    {
        services.AddConfigMgmt(typeof(TGetter0), typeof(TGetter1), typeof(TGetter2));
    }

    /// <summary>
    /// Adds configuration management to the service collection.
    /// </summary>
    /// <typeparam name="TGetter0">The type of the 1st getter to add.</typeparam>
    /// <typeparam name="TGetter1">The type of the 2nd getter to add.</typeparam>
    /// <typeparam name="TGetter2">The type of the 3rd getter to add.</typeparam>
    /// <typeparam name="TGetter3">The type of the 4th getter to add.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <exception cref="ArgumentException">Thrown when a type does not implement IGetter.</exception>
    public static void AddConfigMgmt<TGetter0, TGetter1, TGetter2, TGetter3>(this IServiceCollection services)
    where TGetter0 : IGetter
    where TGetter1 : IGetter
    where TGetter2 : IGetter
    where TGetter3 : IGetter
    {
        services.AddConfigMgmt(typeof(TGetter0), typeof(TGetter1), typeof(TGetter2), typeof(TGetter3));
    }

    /// <summary>
    /// Fits the value(s) to the specified min and max values.
    /// </summary>
    /// <param name="entity">The entity to fit.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The entity.</returns>
    public static Entity<int> Fit(this Entity<int> entity, int min, int max)
    {
        if (max < min)
        {
            throw new ArgumentException("The min value cannot be greater than the max value.");
        }

        entity.Transform(val =>
        {
            if (val < min)
            {
                val = min;
            }
            else if (val > max)
            {
                val = max;
            }

            return val;
        });

        return entity;
    }

    /// <summary>
    /// Fits the value(s) to the specified min and max values.
    /// </summary>
    /// <param name="entity">The entity to fit.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The entity.</returns>
    public static Entity<decimal> Fit(this Entity<decimal> entity, decimal min, decimal max)
    {
        if (max < min)
        {
            throw new ArgumentException("The min value cannot be greater than the max value.");
        }

        entity.Transform(val =>
        {
            if (val < min)
            {
                val = min;
            }
            else if (val > max)
            {
                val = max;
            }

            return val;
        });

        return entity;
    }

    /// <summary>
    /// This method attempts to parse a string to a boolean value.
    /// </summary>
    /// <param name="strVal">The string value to parse.</param>
    /// <param name="boolVal">The boolean value.</param>
    /// <returns>True if the string was parsed successfully; otherwise, false.</returns>
    internal static bool TryParseAsBool(this string strVal, out bool boolVal)
    {
        boolVal = false;

        if (string.IsNullOrWhiteSpace(strVal))
        {
            return false;
        }

        if (bool.TryParse(strVal, out boolVal))
        {
            return true;
        }

        if (strVal.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("y", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("true", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("t", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("1", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("active", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("enabled", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("activated", StringComparison.OrdinalIgnoreCase))
        {
            boolVal = true;
            return true;
        }

        if (strVal.Equals("no", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("n", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("false", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("f", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("0", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("inactive", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("disabled", StringComparison.OrdinalIgnoreCase) ||
            strVal.Equals("deactivated", StringComparison.OrdinalIgnoreCase))
        {
            boolVal = false;
            return true;
        }

        return false;
    }
}