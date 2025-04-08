namespace ArgumentParser
{

public abstract class Token
{
}

public class OptionToken : Token
{
    public OptionToken(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }
    public string Value { get; }
}

public class PositionalToken : Token
{
    public PositionalToken(int position, string value)
    {
        Position = position;
        Value = value;
    }

    public int Position { get; }
    public string Value { get; }
}

public class FlagToken : Token
{
    public FlagToken(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

public class UnknownToken : Token
{
    public UnknownToken(string name, int position, string value)
    {
        Name = name;
        Position = position;
        Value = value;
    }

    public string Name { get; }
    public int Position { get; }
    public string Value { get; }
}

}