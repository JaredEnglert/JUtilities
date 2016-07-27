using System;
using Utilitarian.Extensions;

namespace Utilitarian.FluentExcel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormatStringAttribute : Attribute
    {
        public string FormatString { get; private set; }

        public FormatStringAttribute(FileFormatters fileFormatter)
        {
            FormatString = fileFormatter.ToDescription();
        }

        public FormatStringAttribute(string formatString)
        {
            FormatString = formatString;
        }
    }
}
