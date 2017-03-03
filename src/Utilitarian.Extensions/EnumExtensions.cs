using System;
using System.ComponentModel;

namespace Utilitarian.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum @enum)
        {
            var type = @enum.GetType();

            var memberInfo = type.GetMember(@enum.ToString());

            if (memberInfo.Length <= 0) return @enum.ToString();

            var customAttributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return customAttributes.Length > 0
                ? ((DescriptionAttribute)customAttributes[0]).Description
                : @enum.ToString();
        }
    }
}
