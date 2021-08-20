using System;

namespace Model
{
    // in ye olden days
    public static partial class EnumExtensions
    {
        public static string ToSomeStringWithReflection(this Enum value) =>
            value.GetAttribute<SomeStringAttribute>()?.Emoji ?? throw new Exception("No String defined!");

        private static T? GetAttribute<T>(this Enum enumValue) where T : Attribute
        {
            var memberInfos = enumValue.GetType().GetMember(enumValue.ToString());
            var attributes = memberInfos[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
    }
}