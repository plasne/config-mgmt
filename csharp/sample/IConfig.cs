namespace Client;

public interface IConfig
{
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

    public void Validate();
}