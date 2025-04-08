using Xunit;
using ArgumentParser;
using System.Collections.Generic;

namespace Tests;

public class ArgumentTokenizerTests
{
	[Fact]
	public void TokenizeArguments_FlagsOnly_ShouldParseCorrectly()
	{
		// Arrange
		var args = new[] { "-v", "--Optimize" };
		var flags = new[]
		{
	    new FlagAttribute("v", "Verbose"),
	    new FlagAttribute(null, "Optimize")
	};
		var options = new OptionAttribute[0];
		var positionals = new PositionalAttribute[0];
		var tokenizer = new ArgumentTokenizer();

		// Act
		var (tokens, errors) = tokenizer.TokenizeArguments(args, options, positionals, flags);

		// Assert
		Assert.Empty(errors);
		Assert.Collection(tokens,
		    token => {
			    Assert.IsType<FlagToken>(token);
			    Assert.Equal("v", ((FlagToken)token).Name);
		    },
		    token => {
			    Assert.IsType<FlagToken>(token);
			    Assert.Equal("Optimize", ((FlagToken)token).Name);
		    }
		);
	}

	[Fact]
	public void TokenizeArguments_OptionsOnly_ShouldParseCorrectly()
	{
		// Arrange
		var args = new[] { "-f", "filename.txt", "--output", "stdout" };
		var options = new[]
		{
	    new OptionAttribute("f", null),
	    new OptionAttribute(null, "output")
	};
		var flags = new FlagAttribute[0];
		var positionals = new PositionalAttribute[0];
		var tokenizer = new ArgumentTokenizer();

		// Act
		var (tokens, errors) = tokenizer.TokenizeArguments(args, options, positionals, flags);

		// Assert
		Assert.Empty(errors);
		Assert.Collection(tokens,
		    token => {
			    Assert.IsType<OptionToken>(token);
			    Assert.Equal("f", ((OptionToken)token).Name);
			    Assert.Equal("filename.txt", ((OptionToken)token).Value);
		    },
		    token => {
			    Assert.IsType<OptionToken>(token);
			    Assert.Equal("output", ((OptionToken)token).Name);
			    Assert.Equal("stdout", ((OptionToken)token).Value);
		    }
		);
	}

	[Fact]
	public void TokenizeArguments_PositionalsOnly_ShouldParseCorrectly()
	{
		// Arrange
		var args = new[] { "foo", "bar", "baz" };
		var positionals = new[]
		{
	    new PositionalAttribute(0, "First positional"),
	    new PositionalAttribute(1, "Second positional"),
	    new PositionalAttribute(2, "Third positional")
	};
		var options = new OptionAttribute[0];
		var flags = new FlagAttribute[0];
		var tokenizer = new ArgumentTokenizer();

		// Act
		var (tokens, errors) = tokenizer.TokenizeArguments(args, options, positionals, flags);

		// Assert
		Assert.Empty(errors);
		Assert.Collection(tokens,
		    token => {
			    Assert.IsType<PositionalToken>(token);
			    Assert.Equal(0, ((PositionalToken)token).Position);
			    Assert.Equal("foo", ((PositionalToken)token).Value);
		    },
		    token => {
			    Assert.IsType<PositionalToken>(token);
			    Assert.Equal(1, ((PositionalToken)token).Position);
			    Assert.Equal("bar", ((PositionalToken)token).Value);
		    },
		    token => {
			    Assert.IsType<PositionalToken>(token);
			    Assert.Equal(2, ((PositionalToken)token).Position);
			    Assert.Equal("baz", ((PositionalToken)token).Value);
		    }
		);
	}
}
