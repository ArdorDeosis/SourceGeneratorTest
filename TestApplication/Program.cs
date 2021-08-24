using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Model;
using SourceCodeBuilding;

namespace TestApplication
{
    internal static class Program
    {
        private static void Main()
        {
            // BenchmarkRunner.Run<EnumAttributeBenchmark>();

            var builder = new SourceCodeBuilder();
            builder.AddUsingDirective("System")
                .OpenNamespace("FancyNamespace")
                .OpenClass("Extensions")
                .CloseClass()
                .CloseNameSpace();
            Console.WriteLine(builder);
        }
    }

    [MemoryDiagnoser]
    public class EnumAttributeBenchmark
    {
#pragma warning disable CA1822
        [Benchmark]
        public string WithReflection() => Food.Pizza.ToSomeStringWithReflection();
        
        [Benchmark]
        public string WithSourceGeneration() => Food.Pizza.ToSomeStringGenerated();
#pragma warning restore CA1822
    }
}