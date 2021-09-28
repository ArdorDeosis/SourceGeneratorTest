using SourceGeneratorTest;

namespace Model
{
    [ToJson]
    public partial struct Vector
    {
        public double X { get; set; }
        public float Y { get; set; }
        public char Z { get; set; }
    }
}