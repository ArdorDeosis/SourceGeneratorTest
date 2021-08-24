using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGeneratorTest
{
    [Generator]
    public class EnumExtensionsSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var valueMapping = new Dictionary<string, string>();
            foreach (var enumMemberDeclarationSyntax in context.Compilation.SyntaxTrees
                .Select(tree => tree.GetRoot())
                .SelectMany(root => root.DescendantNodesAndSelf(_ => true))
                .Where(node => node is EnumMemberDeclarationSyntax)
                .Cast<EnumMemberDeclarationSyntax>())
            {
                var dataAttributeSyntax = enumMemberDeclarationSyntax.AttributeLists
                    .SelectMany(attributeList => attributeList.Attributes)
                    .FirstOrDefault(attribute => attribute.Name.ToString() is "SomeString" or "SomeStringAttribute");
                if (dataAttributeSyntax?.ArgumentList == null)
                    continue;
                var parent = (EnumDeclarationSyntax)enumMemberDeclarationSyntax.Parent!;
                var dataValue = dataAttributeSyntax.ArgumentList.Arguments.First().Expression;
                valueMapping.Add($"{parent.Identifier}.{enumMemberDeclarationSyntax.Identifier}", dataValue.ToString());
            }

            var sourceCode = $@"
using {nameof(System)};

namespace Model
{{
    public static partial class EnumExtensions
    {{
        public static string ToSomeStringGenerated(this Enum value) => value switch
        {{
{CodeGenerator.GenerateSwitchExpressionCases(valueMapping, "            ")}
            _ => ""No String defined!""
        }};
    }}
}}
";

            var root = context.Compilation.SyntaxTrees.First().GetRoot();
            var additionalSource = $@"
/*

{string.Join("\n", root.DescendantNodes().Select(x => x + "\n---"))}

*/
";

            context.AddSource("EnumExtensions.Generated.cs", sourceCode + additionalSource);
        }
    }

    public static class CodeGenerator
    {
        public static string GenerateSwitchExpressionCases(IDictionary<string, string> valueMapping,
            string indentation = "") =>
            string.Join("\n", valueMapping.Select(
                keyValuePair => $"{indentation}{keyValuePair.Key} => {keyValuePair.Value},"));
    }
}