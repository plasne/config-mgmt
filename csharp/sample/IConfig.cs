namespace Client;

using FluentResults;

/// <summary>
/// The configuration for this application.
/// </summary>
public interface IConfig
{
    /// <summary>
    /// Gets the value of a string example.
    /// </summary>
    public string StringExample { get; }

    /// <summary>
    /// Gets the value of an integer example.
    /// </summary>
    public int IntegerExample { get; }

    /// <summary>
    /// Gets the value of a decimal example.
    /// </summary>
    public decimal DecimalExample { get; }

    /// <summary>
    /// Gets a value indicating whether this boolean example is true or not.
    /// </summary>
    public bool BooleanExample0 { get; }

    /// <summary>
    /// Gets a value indicating whether this boolean example is true or not.
    /// </summary>
    public bool BooleanExample1 { get; }

    /// <summary>
    /// Gets the value of a list of strings example.
    /// </summary>
    public string[] StringArrayExample { get; }

    /// <summary>
    /// Gets the value of a color.
    /// </summary>
    public Colors ColorExample { get; }

    /// <summary>
    /// Gets the value of a TrySet example.
    /// </summary>
    public int TrySetExample { get; }

    /// <summary>
    /// Gets the value of a custom transform example.
    /// </summary>
    public string TransformExample { get; }

    /// <summary>
    /// Gets the value of a Key Vault example.
    /// </summary>
    public string KeyVaultExample { get; }

    /// <summary>
    /// Gets the value of a AppConfig example.
    /// </summary>
    public string AppConfigExample { get; }

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    /// <returns>The result of the validation.</returns>
    public Result Validate();
}