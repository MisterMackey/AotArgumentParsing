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
					.Any(attr => {
						if (!(semanticModel.GetSymbolInfo(attr).Symbol is IMethodSymbol symbol))
							return false;
						return symbol.ContainingType.ToDisplayString().Contains("OptionAttribute");
					}))
				.ToList();
			// get all of them with the PositionalAttribute
			var positionals = properties
				.Where(p => p.AttributeLists
					.SelectMany(al => al.Attributes)
					.Any(attr => {
						if (!(semanticModel.GetSymbolInfo(attr).Symbol is IMethodSymbol symbol))
							return false;
						return symbol.ContainingType.ToDisplayString().Contains("PositionalAttribute");
					}))
				.ToList();
			// get all of them with the FlagAttribute
			var flags = properties
				.Where(p => p.AttributeLists
					.SelectMany(al => al.Attributes)
					.Any(attr => {
						if (!(semanticModel.GetSymbolInfo(attr).Symbol is IMethodSymbol symbol))
							return false;
						return symbol.ContainingType.ToDisplayString().Contains("FlagAttribute");
					}))
				.ToList();
			var sourceText = GenerateSourceText(classDeclaration, options, positionals, flags);
			context.AddSource($"{classDeclaration.Identifier.Text}_Parser.g.cs", sourceText);
		}

		private string GenerateSourceText(ClassDeclarationSyntax classDeclaration, List<PropertyDeclarationSyntax> options, List<PropertyDeclarationSyntax> positionals, List<PropertyDeclarationSyntax> flags)
		{
			var className = classDeclaration.Identifier.Text;
			var namespaceName = classDeclaration.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name.ToString() ?? "GlobalNamespace";
			string sourceText = $@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace {namespaceName}
{{
	public partial class {className}
	{{
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