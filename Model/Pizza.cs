using System.Collections.Generic;
using SourceGeneratorTest;

namespace Model
{
    [ToJson]
    public partial class Pizza
    {
        public int Radius { get; set; }
        public bool Veggie { get; set; }
        public string Name { get; set; }
        public string[] Ingredients { get; set; }
        public List<Vector> VectorList { get; set; }
        public double DoubleValue { get; set; }
        public float BakingTimeInSeconds { get; set; }
    }
}