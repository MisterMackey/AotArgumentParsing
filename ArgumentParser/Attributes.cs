namespace ArgumentParser;

/// <summary>
/// This attribute is used to mark a class as a collection of parameters.
/// The class should be a public partial class with a parameterless constructor.
///  The parameters should be defined as public, mutable properties of the class
/// and should in turn be decorated with one of the parameter attributes.
/// 
/// During compilation, the parser generator will generate a parser for this class,
/// based on the parameters defined in the class. The parser will be generated as a
/// static method called Parse in the class. This method will take a string[] as
/// input and will return an instance of the class with the parameters set. 
/// </summary>
public class ParameterCollectionAttribute : Attribute
{
    public ParameterCollectionAttribute()
    {
    }
}

/// <summary>
/// This attribute is used to define an option parameter for the parser.
/// Options are identified by a short name (e.g., -o) or a long name (e.g., --option).
/// When an option is used, it is ALWAYS followed by a value (e.g., -o value).
/// They can include a description and specify whether they are required.
/// </summary>
public class OptionAttribute : Attribute
{
	public OptionAttribute(string? shortName = null, string? longName = null, string description = "", bool required = false)
	{
		if (shortName == null && string.IsNullOrEmpty(longName))
			throw new ArgumentException("Either shortName or longName must be provided.");

		if (shortName != null && shortName.Length != 1)
			throw new ArgumentException("ShortName must be a single character.");

		ShortName = shortName;
		LongName = longName;
		Description = description;
		Required = required;
	}
	public string? ShortName { get; }
	public string? LongName { get; }
	public string Description { get; }
	public bool Required { get; }
}

/// <summary>
/// This attribute is used to define a positional parameter for the parser.
/// Positional parameters are identified by their position in the argument list
/// and can include a description and specify whether they are required.
/// </summary>
public class PositionalAttribute : Attribute
{
	public PositionalAttribute(int position, string description, bool required = false)
	{
		Position = position;
		Description = description;
		Required = required;
	}
	public int Position { get; }
	public string Description { get; }
	public bool Required { get; }
}

/// <summary>
/// This attribute is used to define a flag parameter for the parser.
/// Flags are identified by a short name (e.g., -f) or a long name (e.g., --flag).
/// Flags are boolean parameters that indicate whether a specific condition is true.
/// When used, they are NEVER followed by a value.
/// </summary>
public class FlagAttribute : Attribute
{
	public FlagAttribute(string? shortName = null, string? longName = null, string description = "")
	{
		if (shortName == null && string.IsNullOrEmpty(longName))
			throw new ArgumentException("Either shortName or longName must be provided.");

		if (shortName != null && shortName.Length != 1)
			throw new ArgumentException("ShortName must be a single character.");

		ShortName = shortName;
		LongName = longName;
		Description = description;
	}
	public string? ShortName { get; }
	public string? LongName { get; }
	public string Description { get; }
}
