namespace CSE.ConfigMgmt.Getters;

using System;
using FluentResults;

/// <summary>
/// Gets a value from environment variables.
/// </summary>
public class EnvGetter : IGetter
{
    /// <inheritdoc />
    public Result<string> TryGet(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result.Fail("The key was null or empty.");
        }

        var result = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(result))
        {
            return result;
        }

        return Result.Fail("The key was not found.");
    }
}