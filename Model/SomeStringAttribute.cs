using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SomeStringAttribute : Attribute
    {
        public string SomeString { get; }
        
        public SomeStringAttribute(string someString)
        {
            SomeString = someString;
        }
    }
}