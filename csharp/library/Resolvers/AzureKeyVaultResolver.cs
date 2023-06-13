namespace CSE.ConfigMgmt.Resolvers;

using System;
using System.Diagnostics;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FluentResults;
using Microsoft.Extensions.Logging;

/// <summary>
/// Resolves a Key Vault URL to a secret value.
/// </summary>
public class AzureKeyVaultResolver : IResolver<string>
{
    private readonly DefaultAzureCredential credential;
    private readonly ILogger<AzureKeyVaultResolver> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureKeyVaultResolver"/> class.
    /// </summary>
    /// <param name="credential">An optional DefaultAzureCredential object. If not specified, a DefaultAzureCredential object
    /// will be created supporting all credential options. As this can be quite slow, you are encouraged to inject your own
    /// DefaultAzureCredential object into DI.</param>
    /// <param name="logger">An optional ILogger.</param>
    public AzureKeyVaultResolver(DefaultAzureCredential credential = null, ILogger<AzureKeyVaultResolver> logger = null)
    {
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

    /// <inheritdoc />
    public Result<string> Resolve(string key)
    {
        // ensure this is a key vault URL
        if (string.IsNullOrWhiteSpace(key))
        {
            return Result.Fail("The key is null or empty.");
        }

        if (!key.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail("The key must start with 'https://'");
        }

        if (!key.Contains(".vault.azure.net/", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail("The key must contain '.vault.azure.net/'");
        }

        // decompose the key
        var decomposeResult = Decompose(key, "/secrets/");
        if (decomposeResult.IsFailed)
        {
            return Result.Fail(decomposeResult.Errors);
        }

        // get the secret value
        var (vault, name, version) = decomposeResult.Value;
        var watch = Stopwatch.StartNew();
        this.logger?.LogDebug("Getting secret '{name}' from '{vault}'...", name, vault);
        var client = new SecretClient(new Uri(vault), this.credential);
        var response = client.GetSecret(name, version);
        watch.Stop();
        this.logger?.LogDebug("Got secret '{name}' from '{vault}' successfully after {x} ms.", name, vault, watch.ElapsedMilliseconds);

        // return the secret value
        var kvsecret = response.Value;
        return kvsecret.Value;
    }

    private static Result<(string Vault, string Name, string Version)> Decompose(string key, string splitOn)
    {
        // find the index
        int index = key.IndexOf(splitOn, StringComparison.OrdinalIgnoreCase);

        // if not found, return the input
        if (index == -1)
        {
            return Result.Fail($"The URL does not contain '{splitOn}'.");
        }

        // split
        var vault = key[..index];
        var withVersion = key[(index + splitOn.Length)..];

        // split the secret and version
        var parts = withVersion.Split('/');
        if (parts.Length == 1)
        {
            return (vault, parts[0], null);
        }
        else if (parts.Length == 2)
        {
            return (vault, parts[0], parts[1]);
        }

        return Result.Fail("The URL format does not appear to be '/secrets/<secret-name>/<version>'.");
    }
}