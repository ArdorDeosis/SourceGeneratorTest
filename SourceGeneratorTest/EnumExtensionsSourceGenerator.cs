using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            var caseBuilder = new StringBuilder("");
            foreach (var enumMemberDeclarationSyntax in context.Compilation.SyntaxTrees
                .Select(tree => tree.GetRoot())
                .SelectMany(root => root.DescendantNodesAndSelf(_ => true))
                .Where(node => node is EnumMemberDeclarationSyntax)
                .Cast<EnumMemberDeclarationSyntax>())
            {
                var emojiAttribute = enumMemberDeclarationSyntax.AttributeLists
                    .SelectMany(attributeList => attributeList.Attributes)
                    .FirstOrDefault(attribute => attribute.Name.ToString() is "SomeString" or "SomeStringAttribute");
                if (emojiAttribute?.ArgumentList == null)
                    continue;
                var parent = (EnumDeclarationSyntax)enumMemberDeclarationSyntax.Parent!;
                var emoji = emojiAttribute.ArgumentList.Arguments.First().Expression;
                caseBuilder.AppendLine($"{parent.Identifier}.{enumMemberDeclarationSyntax.Identifier} => {emoji},");
            }


            context.AddSource("testFile", @"
using System;

namespace Model
{
    public static partial class EnumExtensions
    {
        public static string ToSomeStringGenerated(this Enum value) => value switch
        {
" + caseBuilder + @"
            _ => throw new Exception(""No String defined!"")
        };
    }
}
");
        }
    }
}