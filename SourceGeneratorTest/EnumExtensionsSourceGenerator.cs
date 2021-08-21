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

            context.AddSource("testFile", $@"
using {nameof(System)};

namespace Model
{{
    public static partial class EnumExtensions
    {{
        public static string ToSomeStringGenerated(this Enum value) => value switch
        {{
{CodeGenerator.GenerateSwitchExpressionCases(valueMapping, "            ")}
            _ => throw new Exception(""No String defined!"")
        }};
    }}
}}
");
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