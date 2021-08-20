using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SomeStringAttribute : Attribute
    {
        public string Emoji { get; }
        
        public SomeStringAttribute(string emoji)
        {
            Emoji = emoji;
        }
    }
}