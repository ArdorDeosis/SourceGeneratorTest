using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGeneratorTest
{
    public static class Extensions
    {
        public static bool HasAttribute<T>(this TypeDeclarationSyntax typeDeclarationSyntax) where T : Attribute
        {
            var attributeName = typeof(T).Name;
            var attributeNameShort = attributeName[..^9];
            return typeDeclarationSyntax.AttributeLists
                .SelectMany(list => list.Attributes)
                .Any(attribute =>
                    attribute.Name.ToString() == attributeName ||
                    attribute.Name.ToString() == attributeNameShort);
        }

        public static List<NamespaceDeclarationSyntax> GetNamespaceList(
            this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            Stack<NamespaceDeclarationSyntax> namespaces = new();
            var current = typeDeclarationSyntax.Parent;
            while (current != null)
            {
                if (current is NamespaceDeclarationSyntax @namespace)
                    namespaces.Push(@namespace);
                current = current.Parent;
            }
            return namespaces.ToList();
        }
    }
}