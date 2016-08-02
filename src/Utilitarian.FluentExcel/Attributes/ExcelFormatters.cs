using System.ComponentModel;

namespace Utilitarian.FluentExcel.Attributes
{
    /// <summary>File formatters for excel spreadsheets.</summary>
    /// <see cref="http://cosicimiento.blogspot.com/2008/11/styling-excel-cells-with-mso-number.html"/>
    /// <remarks>
    /// Styling Excel cells with mso-number-format
    /// mso-number-format:"0"	NO Decimals
    /// mso-number-format:"0\.000"	3 Decimals
    /// mso-number-format:"\#\,\#\#0\.000"	Comma with 3 dec
    /// mso-number-format:"mm\/dd\/yy"	Date7
    /// mso-number-format:"mmmm\ d\,\ yyyy"	Date9
    /// mso-number-format:"m\/d\/yy\ h\:mm\ AM\/PM"	D -T AMPM
    /// mso-number-format:"Short Date"	01/03/1998
    /// mso-number-format:"Medium Date"	01-mar-98
    /// mso-number-format:"d\-mmm\-yyyy"	01-mar-1998
    /// mso-number-format:"Short Time"	5:16
    /// mso-number-format:"Medium Time"	5:16 am
    /// mso-number-format:"Long Time"	5:16:21:00
    /// mso-number-format:"Percent"	Percent - two decimals
    /// mso-number-format:"0%"	Percent - no decimals
    /// mso-number-format:"0\.E+00"	Scientific Notation
    /// mso-number-format:"\@"	Text
    /// mso-number-format:"\#\ ???\/???"	Fractions - up to 3 digits (312/943)
    /// mso-number-format:"\0022£\0022\#\,\#\#0\.00"	£12.76
    /// mso-number-format:"\#\,\#\#0\.00_ \;\[Red\]\-\#\,\#\#0\.00\"
    /// 2 decimals, negative numbers in red and signed (1.56 -1.56)
    /// </remarks>
    public enum ExcelFormatters
    {
        /// <summary>Formats the column as a number with no decimal places.</summary>
        [Description(@"#,##0")]
        NumberNoDecimal,

        /// <summary>Formats the column as a zip code, this works on 4 digit zip codes and +4 zip codes.</summary>
        [Description(@"\[<=99999\]00000\;\#\#\#\#\#\-\#\#\#\#")]
        ZipCode,

        /// <summary>Formats the column as a phone number.</summary>
        [Description(@"\[<=9999999\]\#\#\#\-\#\#\#\#\;\\\(\#\#\#\\\)\#\#\#\-\#\#\#\#")]
        PhoneNumber,

        /// <summary>Formats the column as a currency.</summary>
        [Description("\"$\"#,##0.00_);(\"$\"#,##0.00)")]
        Currency,

        /// <summary>Formats the column as a currency in dollars only.</summary>
        [Description("\"$\"#,##0_);(\"$\"#,##0)")]
        CurrencyInDollars,

        /// <summary>Formats the column as a percentage with 2 decimal places.</summary>
        [Description("0%")]
        Percent,

        /// <summary>Formats the column as a string.</summary>
        [Description(@"\@")]
        String,

        /// <summary>Formats the column as a DateTime.ToString("MM\dd\yyyy").</summary>
        [Description(@"mm\/dd\/yyyy")]
        Date,

        /// <summary>Formats the column as a DateTime.ToString("MM\dd\yyyy HH:mm:ss").</summary>
        [Description(@"mm\/dd\/yyyy hh\:mm\:ss")]
        DateTime,

        /// <summary>Formats the column as a DateTime.ToString("HH:mm:ss").</summary>
        [Description(@"hh\:mm\:ss")]
        Time
    }
}
