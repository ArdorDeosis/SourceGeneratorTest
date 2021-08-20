using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Model;

namespace TestApplication
{
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<ToEmojiBenchmark>();
        }
    }

    [MemoryDiagnoser]
    public class ToEmojiBenchmark
    {
#pragma warning disable CA1822
        [Benchmark]
        public string WithReflection() => Food.Pizza.ToSomeStringWithReflection();
        
        [Benchmark]
        public string WithSourceGeneration() => Food.Pizza.ToSomeStringGenerated();
#pragma warning restore CA1822
    }
}