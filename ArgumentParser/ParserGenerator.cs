using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArgumentParser
{

[Generator]
public class ParserAugmenter : ISourceGenerator
{
	public void Initialize(GeneratorInitializationContext context)
	{
		context.RegisterForSyntaxNotifications(() => new ArgumentSpecificationSyntaxReceiver());
	}

	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = context.SyntaxReceiver as ArgumentSpecificationSyntaxReceiver;
		if (receiver == null)
			return;

		foreach (var classDeclaration in receiver.CandidateClasses)
		{
			AddParserCode(classDeclaration, context);
		}
	}

	private class ArgumentSpecificationSyntaxReceiver : ISyntaxReceiver
	{
		public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			// Look for class declarations with the ArgumentSpecificationAttribute
			if (syntaxNode is ClassDeclarationSyntax classDeclaration &&
			    classDeclaration.AttributeLists
			        .SelectMany(al => al.Attributes)
			        .Any(attr => attr.Name.ToString().Contains("ArgumentSpecificationAttribute")))
			{
				CandidateClasses.Add(classDeclaration);
			}
		}
	}

	private void AddParserCode(ClassDeclarationSyntax classDeclaration, GeneratorExecutionContext context)
	{
		// Generate the parser code for the class
	}
}

}