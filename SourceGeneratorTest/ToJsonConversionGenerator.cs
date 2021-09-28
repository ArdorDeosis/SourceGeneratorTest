using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGeneratorTest
{
    [Generator]
    public class ToJsonConversionGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var jsonConversionTypes = context.Compilation.SyntaxTrees
                .Select(tree => tree.GetRoot())
                .SelectMany(root => root.DescendantNodesAndSelf(_ => true))
                .OfType<TypeDeclarationSyntax>()
                .Where(Extensions.HasAttribute<ToJsonAttribute>)
                .ToList();
            
            foreach (var jsonConversionType in jsonConversionTypes)
            {
                try
                {
                    context.AddSource($"{jsonConversionType.Identifier}.Generated.cs",
                        GeneratePartialClassWithJsonConverter(jsonConversionType));
                }
                catch (Exception e)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor("FJ01", 
                            "JSON conversion generation failed", 
                            "JSON conversion generation failed for {0} {1} with exception message '{2}'",
                            "FastJson",
                            DiagnosticSeverity.Error,
                            true), 
                        Location.None, 
                        jsonConversionType.Keyword,
                        jsonConversionType.Identifier,
                        e.Message));
                }
            }

            string GeneratePartialClassWithJsonConverter(TypeDeclarationSyntax typeDeclarationSyntax)
            {
                var builder = new StringBuilder();
                var fullNamespace =
                    string.Join(".", typeDeclarationSyntax.GetNamespaceList().Select(n => n.Name.ToString()));
                builder.AppendLine($"namespace {fullNamespace}{Environment.NewLine}{{");
                builder.AppendLine($"    public partial {typeDeclarationSyntax.Keyword} {typeDeclarationSyntax.Identifier}{Environment.NewLine}    {{");
                if (typeDeclarationSyntax.Keyword.ToString() == "struct")
                    builder.AppendLine("        public readonly string ToJson() =>");
                else
                    builder.AppendLine("        public string ToJson() =>");
                builder.AppendLine($"            {TypeAsJsonStringOutput(typeDeclarationSyntax)}");
                builder.AppendLine("    }");
                builder.AppendLine("}");
                return builder.ToString();
            }

            string TypeAsJsonStringOutput(TypeDeclarationSyntax typeDeclarationSyntax)
            {
                var propertiesAsJsonStringInterpolation = typeDeclarationSyntax.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .Select(property => $"\\\"{property.Identifier}\\\":{PropertyValueStringInterpolation(property)}");
                return $"\"{{{string.Join(",", propertiesAsJsonStringInterpolation)}}}\";";
            }

            string PropertyValueStringInterpolation(PropertyDeclarationSyntax propertyDeclarationSyntax) =>
                PropertyValueStringInterpolationInternal(propertyDeclarationSyntax.Type.ToString(), 
                    propertyDeclarationSyntax.Identifier.ToString());

            string PropertyValueStringInterpolationInternal(string type, string name)
            {
                // complex types having a .ToJason() method
                if (jsonConversionTypes.Any(jsonConversionType => jsonConversionType.Identifier.ToString() == type))
                    return $"\" + {name}.ToJson() + \"";
                
                // arrays
                if (type.EndsWith("[]"))
                    return $"[\" + string.Join(\",\", System.Linq.Enumerable.Select({name}, element => $\"{PropertyValueStringInterpolationInternal(type[..^2], "element")}\")) + \"]";
                
                // List
                if (type.StartsWith("List<"))
                    return $"[\" + string.Join(\",\", System.Linq.Enumerable.Select({name}, element => $\"{PropertyValueStringInterpolationInternal(type[5..^1], "element")}\")) + \"]";
                
                // IList
                if (type.StartsWith("IList<"))
                    return $"[\" + string.Join(\",\", System.Linq.Enumerable.Select({name}, element => $\"{PropertyValueStringInterpolationInternal(type[6..^1], "element")}\")) + \"]";
                
                // IEnumerable
                if (type.StartsWith("IEnumerable<"))
                    return $"[\" + string.Join(\",\", System.Linq.Enumerable.Select({name}, element => $\"{PropertyValueStringInterpolationInternal(type[12..^1], "element")}\")) + \"]";
                
                // primitive types
                return type switch
                {
                    "string" or "char" => $"\\\"\" + {name}  + \"\\\"",
                    "bool" => $"\" + ({name} ? \"true\" : \"false\") + \"",
                    "sbyte" or "byte" or "short" or "ushort" or "int" or "uint" or "long" or "ulong" =>
                        $"\" + {name} + \"",
                    "float" => $"\" + $\"{{{name}:G9}}\" + \"",
                    "double" => $"\" + $\"{{{name}:G17}}\" + \"",
                    _ => throw new Exception($"cannot convert type {type} to JSON")
                };
            }
        }
    }
}