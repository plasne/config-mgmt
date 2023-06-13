namespace CSE.ConfigMgmt.Getters;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using FluentResults;
using Microsoft.Extensions.Logging;

// TODO: add docs
// TODO: add unit tests

/// <summary>
/// Gets settings from Azure App Configuration.
/// </summary>
public class AzureAppConfigGetter : IGetter
{
    private readonly string endpoint;
    private readonly SettingSelector selector;
    private readonly DefaultAzureCredential credential;
    private readonly ILogger<AzureAppConfigGetter> logger;
    private Dictionary<string, string> settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigGetter"/> class.
    /// </summary>
    /// <param name="endpoint">The URI of the AppConfiguration endpoint.</param>
    /// <param name="selector">An optional selector to limit the settings returned. If not specified, all will be read.</param>
    /// <param name="credential">An optional DefaultAzureCredential object. If not specified, a DefaultAzureCredential object
    /// will be created supporting all credential options. As this can be quite slow, you are encouraged to inject your own
    /// DefaultAzureCredential object into DI.</param>
    /// <param name="logger">An optional ILogger.</param>
    public AzureAppConfigGetter(
        string endpoint,
        SettingSelector selector = null,
        DefaultAzureCredential credential = null,
        ILogger<AzureAppConfigGetter> logger = null)
    {
        this.endpoint = endpoint;
        this.selector = selector ?? new SettingSelector();
        this.credential = credential ?? new DefaultAzureCredential(
            new DefaultAzureCredentialOptions()
            {
                ExcludeEnvironmentCredential = false,
                ExcludeManagedIdentityCredential = false,
                ExcludeSharedTokenCacheCredential = false,
                ExcludeVisualStudioCredential = false,
                ExcludeVisualStudioCodeCredential = false,
                ExcludeAzureCliCredential = false,
                ExcludeInteractiveBrowserCredential = false,
            });
        this.logger = logger;
    }

    /// <summary>
    /// Gets a setting from Azure App Configuration. The first time this is called, all settings will be read into memory.
    /// </summary>
    /// <param name="key">The key to get.</param>
    /// <returns>The value wrapped in a Result.</returns>
    public Result<string> TryGet(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result.Fail("The key was null or empty.");
        }

        // read the settings
        if (this.settings is null)
        {
            var watch = Stopwatch.StartNew();
            this.logger?.LogDebug("Getting settings from '{uri}'...", this.endpoint);
            var client = new ConfigurationClient(new Uri(this.endpoint), this.credential);
            var retrieved = client.GetConfigurationSettings(this.selector);
            this.settings = retrieved
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .ToDictionary(
                    x => x.Key,
                    x =>
                    {
                        // see if it is a JSON object with a uri property (this is what app service returns)
                        if (x.Value.StartsWith('{') && x.Value.EndsWith('}'))
                        {
                            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(x.Value);
                            if (dict.TryGetValue("uri", out var uri))
                            {
                                return uri;
                            }
                        }

                        // return the unmodified value
                        return x.Value;
                    },
                    StringComparer.OrdinalIgnoreCase);
            watch.Stop();
            this.logger?.LogDebug(
                "Got {c} settings from '{uri}' successfully after {x} ms, including: {keys}.",
                this.settings.Count,
                this.endpoint,
                watch.ElapsedMilliseconds,
                string.Join(", ", this.settings.Keys));
        }

        // 1st try to get based on the full key name
        if (this.settings.TryGetValue(key, out var value))
        {
            return value;
        }

        // 2nd try to get based on the last part of the name
        foreach (var setting in this.settings)
        {
            var lastPart = setting.Key.Split(new char[] { '/', '\\', ':', '.', ',' }).Last();
            if (string.Equals(lastPart, key, StringComparison.OrdinalIgnoreCase))
            {
                return setting.Value;
            }
        }

        return Result.Fail("The key was not found.");
    }
}