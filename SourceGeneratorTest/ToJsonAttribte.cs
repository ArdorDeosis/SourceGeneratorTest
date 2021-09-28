using System;

namespace SourceGeneratorTest
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ToJsonAttribute : Attribute
    {
    }
}