using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArgumentParser
{

	[Generator(LanguageNames.CSharp)]
	public class ParserAugmenter : IIncrementalGenerator
	{
		public void Initialize(IncrementalGeneratorInitializationContext initializationContext)
		{
			var parameterCollectionClasses = initializationContext.SyntaxProvider
			.ForAttributeWithMetadataName(
			    "ArgumentParser.ParameterCollectionAttribute", // Fully qualified name of the attribute
			    (node, _) => node is ClassDeclarationSyntax, // Filter for class declarations
			    (context, _) => new ParserClassDecleration
			    {
				    ClassDeclaration = context.TargetNode as ClassDeclarationSyntax,
				    SemanticModel = context.SemanticModel
			    }
			);

			// register output
			initializationContext.RegisterSourceOutput(parameterCollectionClasses, (context, classDeclaration) =>
			{
				DoStuff(context, classDeclaration.ClassDeclaration, classDeclaration.SemanticModel);
			});
		}

		private void DoStuff(SourceProductionContext context, ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
		{
			// get all properties
			var properties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>().ToList();
			// get all of them with the OptionAttribute
			var options = properties
				.Where(p => p.AttributeLists
					.SelectMany(al => al.Attributes)
					.Any(attr =>
					{
						if (!(semanticModel.GetSymbolInfo(attr).Symbol is IMethodSymbol symbol))
							return false;
						return symbol.ContainingType.ToDisplayString().Contains("OptionAttribute");
					}))
				.Select(o => InstantiateOptionAttribute(o, semanticModel))
				.ToList();
			// get all of them with the PositionalAttribute
			var positionals = properties
				.Where(p => p.AttributeLists
					.SelectMany(al => al.Attributes)
					.Any(attr =>
					{
						if (!(semanticModel.GetSymbolInfo(attr).Symbol is IMethodSymbol symbol))
							return false;
						return symbol.ContainingType.ToDisplayString().Contains("PositionalAttribute");
					}))
				.Select(p => InstantiatePositionalAttribute(p, semanticModel))
				.ToList();
			// get all of them with the FlagAttribute
			var flags = properties
				.Where(p => p.AttributeLists
					.SelectMany(al => al.Attributes)
					.Any(attr =>
					{
						if (!(semanticModel.GetSymbolInfo(attr).Symbol is IMethodSymbol symbol))
							return false;
						return symbol.ContainingType.ToDisplayString().Contains("FlagAttribute");
					}))
				.Select(f => InstantiateFlagAttribute(f, semanticModel))
				.ToList();
			var sourceText = GenerateSourceText(classDeclaration, options, positionals, flags);
			context.AddSource($"{classDeclaration.Identifier.Text}_Parser.g.cs", sourceText);
		}

		private FlagAttribute InstantiateFlagAttribute(PropertyDeclarationSyntax property, SemanticModel semanticModel)
		{
			//todo: verify this
			var flagAttributeSyntax = property.AttributeLists
			    .SelectMany(al => al.Attributes)
			    .First(attr => attr.Name.ToString().Contains("FlagAttribute"));

			var attributeData = semanticModel.GetSymbolInfo(flagAttributeSyntax).Symbol as IMethodSymbol;
			var flagAttributeData = semanticModel.GetDeclaredSymbol(property)?.GetAttributes()
			    .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString().Contains("FlagAttribute") == true);

			string flagName = string.Empty;
			string flagDescription = string.Empty;

			if (flagAttributeData != null)
			{
				// Extract positional arguments
				if (flagAttributeData.ConstructorArguments.Length > 0)
				{
					flagName = flagAttributeData.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
				}
				if (flagAttributeData.ConstructorArguments.Length > 1)
				{
					flagDescription = flagAttributeData.ConstructorArguments[1].Value?.ToString() ?? string.Empty;
				}

				// Extract named arguments
				foreach (var namedArg in flagAttributeData.NamedArguments)
				{
					if (namedArg.Key == "name")
					{
						flagName = namedArg.Value.Value?.ToString() ?? flagName; // Override if named argument is provided
					}
					else if (namedArg.Key == "description")
					{
						flagDescription = namedArg.Value.Value?.ToString() ?? flagDescription;
					}
				}
			}

			return new FlagAttribute(flagName, flagDescription);
		}

		private OptionAttribute InstantiateOptionAttribute(PropertyDeclarationSyntax property, SemanticModel semanticModel)
		{
			var optionAttributeSyntax = property.AttributeLists
				.SelectMany(al => al.Attributes)
				.First(attr => attr.Name.ToString().Contains("OptionAttribute"));

			var attributeData = semanticModel.GetSymbolInfo(optionAttributeSyntax).Symbol as IMethodSymbol;
			var optionAttributeData = semanticModel.GetDeclaredSymbol(property)?.GetAttributes()
				.FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString().Contains("OptionAttribute") == true);

			string shortName = string.Empty;
			string longName = string.Empty;

			if (optionAttributeData != null)
			{
				// Extract positional arguments
				if (optionAttributeData.ConstructorArguments.Length > 0)
				{
					shortName = optionAttributeData.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
				}
				if (optionAttributeData.ConstructorArguments.Length > 1)
				{
					longName = optionAttributeData.ConstructorArguments[1].Value?.ToString() ?? string.Empty;
				}

				// Extract named arguments
				foreach (var namedArg in optionAttributeData.NamedArguments)
				{
					if (namedArg.Key == "shortName")
					{
						shortName = namedArg.Value.Value?.ToString() ?? shortName;
					}
					else if (namedArg.Key == "longName")
					{
						longName = namedArg.Value.Value?.ToString() ?? longName;
					}
				}
			}

			return new OptionAttribute(shortName, longName);
		}

		private PositionalAttribute InstantiatePositionalAttribute(PropertyDeclarationSyntax property, SemanticModel semanticModel)
		{
			var positionalAttributeSyntax = property.AttributeLists
				.SelectMany(al => al.Attributes)
				.First(attr => attr.Name.ToString().Contains("PositionalAttribute"));

			var attributeData = semanticModel.GetSymbolInfo(positionalAttributeSyntax).Symbol as IMethodSymbol;
			var positionalAttributeData = semanticModel.GetDeclaredSymbol(property)?.GetAttributes()
				.FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString().Contains("PositionalAttribute") == true);

			string position = string.Empty;
			string description = string.Empty;

			if (positionalAttributeData != null)
			{
				// Extract positional arguments
				if (positionalAttributeData.ConstructorArguments.Length > 0)
				{
					position = positionalAttributeData.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
				}
				if (positionalAttributeData.ConstructorArguments.Length > 1)
				{
					description = positionalAttributeData.ConstructorArguments[1].Value?.ToString() ?? string.Empty;
				}

				// Extract named arguments
				foreach (var namedArg in positionalAttributeData.NamedArguments)
				{
					if (namedArg.Key == "position")
					{
						position = namedArg.Value.Value?.ToString() ?? position;
					}
					else if (namedArg.Key == "description")
					{
						description = namedArg.Value.Value?.ToString() ?? description;
					}
				}
			}

			return new PositionalAttribute(int.Parse(position), description);
		}

		private string GenerateSourceText(ClassDeclarationSyntax classDeclaration, List<OptionAttribute> options, List<PositionalAttribute> positionals, List<FlagAttribute> flags)
		{
			var className = classDeclaration.Identifier.Text;
			var namespaceName = classDeclaration.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name.ToString() ?? "GlobalNamespace";
			string sourceText = $@"
// This file is auto-generated. Do not edit it directly.
// avoid using statements to prevent accidental clobbering of user code
namespace {namespaceName}
{{
	public partial class {className}
	{{
		public static ({className} result, List<ArgumentParserException> errors) Parse(string[] args)
		{{
			var tokenizer = new ArgumentParser.ArgumentTokenizer();
			var (tokens, errors) = tokenizer.Tokenize(args);
			for (token in tokens)
		}}
	}}
}}";

			return sourceText;
		}

		private struct ParserClassDecleration
		{
			public ClassDeclarationSyntax ClassDeclaration { get; set; }
			public SemanticModel SemanticModel { get; set; }
		}
	}

}