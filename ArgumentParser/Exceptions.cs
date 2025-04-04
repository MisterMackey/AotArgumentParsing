namespace ArgumentParser;

public abstract class ArgumentParserException : Exception
{
    protected ArgumentParserException(string message) : base(message)
    {
    }
}

public class UnexpectedArgumentException : ArgumentParserException
{
    public UnexpectedArgumentException(string message) : base(message)
    {
    }
}

public class MissingRequiredArgumentException : ArgumentParserException
{
    public MissingRequiredArgumentException(string message) : base(message)
    {
    }
}

public class InvalidArgumentValueException : ArgumentParserException
{
    public InvalidArgumentValueException(string message) : base(message)
    {
    }
}
