using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Model;
using System.Text.Json;

namespace TestApplication
{
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<JsonConverterBenchmark>();
        }
    }

    [MemoryDiagnoser]
    public class JsonConverterBenchmark
    {
        private readonly Pizza pizza = new()
        {
            Radius = 3567,
            Veggie = false,
            Name = "Super Awesome Pizza",
            Ingredients = new[] { "cheese", "more cheese", "even more cheese", "tomatoes", "Lego bricks" },
            VectorList = new List<Vector>
            {
                new() { X = .5, Y = .33f, Z = 'a' },
                new() { X = 723489.5568, Y = 56.33f, Z = 'b' },
            },
            DoubleValue = 32156745.34125685431287d,
            BakingTimeInSeconds = 1.4235674264123685427846231784612387468129f
        };

        private readonly PokemonList pokemonList = new();

        // [Benchmark]
        // public string LargeStruct_SystemTextJson() => JsonSerializer.Serialize(pokemonList);
        //
        // [Benchmark]
        // public string LargeStruct_Generated() => pokemonList.ToJson();
        //
        // [Benchmark]
        // public string Class_SystemTextJson() => JsonSerializer.Serialize(pizza);
        //
        // [Benchmark]
        // public string Class_Generated() => pizza.ToJson();
        
        [Benchmark]
        public string AttributeConversion_Reflection() => Pokemon.Mew.ToSomeStringWithReflection();
        
        [Benchmark]
        public string AttributeConversion_First_Generated() => Food.Pizza.ToSomeStringGenerated();
        [Benchmark]
        public string AttributeConversion_Last_Generated() => Pokemon.Mew.ToSomeStringGenerated();
    }
}