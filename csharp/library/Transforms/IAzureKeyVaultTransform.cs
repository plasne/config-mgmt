namespace CSE.ConfigMgmt.Transforms;

using FluentResults;

/// <summary>
/// Resolves a Key Vault URL to a secret value.
/// </summary>
public interface IAzureKeyVaultTransform
{
    /// <summary>
    /// Attempts to resolve the secret URL to a value.
    /// </summary>
    /// <param name="value">The value to resolve.</param>
    /// <returns>The resolved value wrapped in a Result.</returns>
    public Result<string> Resolve(string value);
}