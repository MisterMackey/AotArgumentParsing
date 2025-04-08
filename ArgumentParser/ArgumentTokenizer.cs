using System.Collections.Generic;
using System.Linq;

namespace ArgumentParser
{

	public class ArgumentTokenizer
	{
		private void ProcessLongNameArgument(string arg, string[] args, ref int i, OptionAttribute[] options, FlagAttribute[] flags, List<Token> tokens, List<ArgumentParserException> errors)
		{
			var name = arg.Substring(2);
			var option = options.FirstOrDefault(o => o.LongName == name);
			var flag = flags.FirstOrDefault(f => f.LongName == name);

			if (option != null)
			{
				if (i + 1 < args.Length)
				{
					tokens.Add(new OptionToken(name, args[++i]));
				}
				else
				{
					errors.Add(new MissingRequiredArgumentException($"Option '--{name}' requires a value."));
				}
			}
			else if (flag != null)
			{
				tokens.Add(new FlagToken(name));
			}
			else
			{
				errors.Add(new UnexpectedArgumentException($"Unknown argument '--{name}'."));
			}
		}

		private void ProcessShortNameArguments(string arg, string[] args, ref int i, OptionAttribute[] options, FlagAttribute[] flags, List<Token> tokens, List<ArgumentParserException> errors)
		{
			var flagsOrOption = arg.Substring(1);

			for (int j = 0; j < flagsOrOption.Length; j++)
			{
				var name = flagsOrOption[j].ToString();
				var flag = flags.FirstOrDefault(f => f.ShortName == name);
				var option = options.FirstOrDefault(o => o.ShortName == name);

				if (flag != null)
				{
					tokens.Add(new FlagToken(name));
				}
				// option arguments are only allowed to appear at the end of the string
				// otherwise it's an error
				else if (j < flagsOrOption.Length - 1)
				{
					errors.Add(new UnexpectedArgumentException($"Unexpected flag '-{name}'."));
				}
				else if (option != null)
				{

					if (i + 1 < args.Length)
					{
						tokens.Add(new OptionToken(name, args[++i]));
					}
					else
					{
						errors.Add(new MissingRequiredArgumentException($"Option '-{name}' requires a value."));
					}
				}
				else
				{
					errors.Add(new UnexpectedArgumentException($"Unknown flag '-{name}'."));
				}
			}
		}

		private void ProcessPositionalArgument(string arg, PositionalAttribute[] positionals, ref int positionalIndex, List<Token> tokens, List<ArgumentParserException> errors)
		{
			if (positionalIndex < positionals.Length)
			{
				tokens.Add(new PositionalToken(positionalIndex, arg));
				positionalIndex++;
			}
			else
			{
				errors.Add(new UnexpectedArgumentException($"Unexpected positional argument '{arg}'."));
			}
		}

		public (List<Token> result, List<ArgumentParserException> err) TokenizeArguments(string[] args, OptionAttribute[] options, PositionalAttribute[] positionals, FlagAttribute[] flags)
		{
			var tokens = new List<Token>();
			var errors = new List<ArgumentParserException>();
			var positionalIndex = 0;

			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];

				if (arg.StartsWith("--"))
				{
					ProcessLongNameArgument(arg, args, ref i, options, flags, tokens, errors);
				}
				else if (arg.StartsWith("-"))
				{
					ProcessShortNameArguments(arg, args, ref i, options, flags, tokens, errors);
				}
				else
				{
					ProcessPositionalArgument(arg, positionals, ref positionalIndex, tokens, errors);
				}
			}

			return (tokens, errors);
		}
	}

}