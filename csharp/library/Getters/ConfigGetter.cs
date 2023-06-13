namespace CSE.ConfigMgmt.Getters;

using FluentResults;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Gets a configuration value from the IConfiguration.
/// </summary>
public class ConfigGetter : IGetter
{
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigGetter"/> class.
    /// </summary>
    /// <param name="configuration">An IConfiguration object.</param>
    public ConfigGetter(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <inheritdoc />
    public Result<string> TryGet(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result.Fail("The key was null or empty.");
        }

        var result = this.configuration.GetValue<string>(key);
        if (!string.IsNullOrWhiteSpace(result))
        {
            return result;
        }

        return Result.Fail("The key was not found.");
    }
}