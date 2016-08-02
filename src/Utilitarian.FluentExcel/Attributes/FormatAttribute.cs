using System;
using Utilitarian.Extensions;

namespace Utilitarian.FluentExcel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormatAttribute : Attribute
    {
        public string FormatString { get; private set; }

        public FormatAttribute(ExcelFormatters excelFormatters)
        {
            FormatString = excelFormatters.ToDescription();
        }

        public FormatAttribute(string formatString)
        {
            FormatString = formatString;
        }
    }
}
