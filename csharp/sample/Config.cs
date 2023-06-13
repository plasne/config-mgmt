namespace Client;

using dotenv.net;
using CSE.ConfigMgmt;
using FluentResults;

public class Config : IConfig
{
    private readonly CSE.ConfigMgmt.IConfig config;

    public Config(CSE.ConfigMgmt.IConfig config)
    {
        this.config = config;

        // load from .env if it exists
        DotEnv.Load(options: new DotEnvOptions().WithoutExceptions());

        // set the values
        this.StringExample = config.AsString("STRING_EXAMPLE").TryGet().Require().Log().Value;
        this.IntegerExample = config.AsInteger("INTEGER_EXAMPLE").TryGet().Fit(1, 10).Log().Value;
        this.DecimalExample = config.AsRational("DECIMAL_EXAMPLE").TryGet().Log().Value;
        this.BooleanExample0 = config.AsBoolean("BOOLEAN_EXAMPLE_0").TryGet().Require().Default(false).Log().Value;
        this.BooleanExample1 = config.AsBoolean("BOOLEAN_EXAMPLE_1").TryGet().Default(true).Log().Value;
        this.StringArrayExample = config.AsStrings("STRING_ARRAY_EXAMPLE").TryGet().Log().Value;
        this.ColorExample = config.AsEnum<Colors>("COLOR_EXAMPLE").TryGet().Require().Log().Value;
        this.TrySetExample = config.AsInteger("TRY_SET_EXAMPLE").TrySet("NOT_A_NUMBER").TrySet("100").Require().Log().Value;
        this.TransformExample = config.AsString("TRANSFORM_EXAMPLE").SetTransform(s =>
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return Result.Fail("TRANSFORM_EXAMPLE cannot be empty.");
            }

            return s.ToUpper();
        }).TryGet().Log().Value;
        this.KeyVaultExample = config.AsString("KEY_VAULT_EXAMPLE").TryGet().Resolve().Log().Value;
        this.AppServiceExample = config.AsString("APP_SERVICE_EXAMPLE").TryGet("KEY_IN_VAULT").Log().Value;
    }

    public string StringExample { get; }

    public int IntegerExample { get; }

    public decimal DecimalExample { get; }

    public bool BooleanExample0 { get; }

    public bool BooleanExample1 { get; }

    public string[] StringArrayExample { get; }

    public Colors ColorExample { get; }

    public int TrySetExample { get; }

    public string TransformExample { get; }

    public string KeyVaultExample { get; }

    public string AppServiceExample { get; }

    public void Validate()
    {
        this.config.Validate();
    }
}