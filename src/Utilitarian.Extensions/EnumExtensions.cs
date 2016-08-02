using System;
using System.ComponentModel;

namespace Utilitarian.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum @enum)
        {
            var type = @enum.GetType();

            var memInfo = type.GetMember(@enum.ToString());

            if (memInfo.Length <= 0) return @enum.ToString();

            var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attrs.Length > 0
                ? ((DescriptionAttribute)attrs[0]).Description
                : @enum.ToString();
        }
    }
}
