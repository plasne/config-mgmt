namespace Client;

using dotenv.net;
using CSE.ConfigMgmt;
using FluentResults;
using CSE.ConfigMgmt.Transforms;

/// <inheritdoc />
public class Config : IConfig
{
    private readonly CSE.ConfigMgmt.IConfig config;

    /// <summary>
    /// Initializes a new instance of the <see cref="Config"/> class.
    /// </summary>
    /// <param name="config">The library configuration.</param>
    /// <param name="keyVaultTransform">The Azure Key Vault resolver.</param>
    public Config(CSE.ConfigMgmt.IConfig config, IAzureKeyVaultTransform keyVaultTransform)
    {
        this.config = config;

        // load from .env if it exists
        DotEnv.Load(options: new DotEnvOptions().WithoutExceptions());

        // set the values
        this.StringExample = config.AsString("STRING_EXAMPLE").Get().Require().Log().Value;
        this.IntegerExample = config.AsInteger("INTEGER_EXAMPLE").Get().Fit(1, 10).Log().Value;
        this.DecimalExample = config.AsRational("DECIMAL_EXAMPLE").Get().Log().Value;
        this.BooleanExample0 = config.AsBoolean("BOOLEAN_EXAMPLE_0").Get().Require().Default(false).Log().Value;
        this.BooleanExample1 = config.AsBoolean("BOOLEAN_EXAMPLE_1").Get().Default(true).Log().Value;
        this.StringArrayExample = config.AsStrings("STRING_ARRAY_EXAMPLE").Get().Log().Value;
        this.ColorExample = config.AsEnum<Colors>("COLOR_EXAMPLE").Get().Require().Log().Value;
        this.TrySetExample = config.AsInteger("TRY_SET_EXAMPLE").Get("NOT_A_NUMBER").Set("100").Require().Log().Value;
        this.TransformExample = config.AsString("TRANSFORM_EXAMPLE").Get().Transform(s =>
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return Result.Fail("TRANSFORM_EXAMPLE cannot be empty.");
            }

            return s.ToUpper();
        }).Log().Value;
        this.KeyVaultExample = config.AsString("KEY_VAULT_EXAMPLE").Get().Transform(keyVaultTransform.Resolve).Log().Value;
        this.AppConfigExample = config.AsString("APP_SERVICE_EXAMPLE").Get("KEY_IN_VAULT").Log().Value;
    }

    /// <inheritdoc />
    public string StringExample { get; }

    /// <inheritdoc />
    public int IntegerExample { get; }

    /// <inheritdoc />
    public decimal DecimalExample { get; }

    /// <inheritdoc />
    public bool BooleanExample0 { get; }

    /// <inheritdoc />
    public bool BooleanExample1 { get; }

    /// <inheritdoc />
    public string[] StringArrayExample { get; }

    /// <inheritdoc />
    public Colors ColorExample { get; }

    /// <inheritdoc />
    public int TrySetExample { get; }

    /// <inheritdoc />
    public string TransformExample { get; }

    /// <inheritdoc />
    public string KeyVaultExample { get; }

    /// <inheritdoc />
    public string AppConfigExample { get; }

    /// <inheritdoc />
    public Result Validate()
    {
        return this.config.Validate();
    }
}