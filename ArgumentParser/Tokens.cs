namespace ArgumentParser;

public abstract class Token
{
}

public class OptionToken(string name, string value) : Token
{
	public string Name { get; } = name;
	public string Value { get; } = value;
}

public class PositionalToken(int position, string value) : Token
{
	public int Position { get; } = position;
	public string Value { get; } = value;
}

public class FlagToken(string name) : Token
{
	public string Name { get; } = name;
}

public class UnknownToken(string name, int position, string value) : Token
{
	public string Name { get; } = name;
	public int Position { get; } = position;
	public string Value { get; } = value;
}
