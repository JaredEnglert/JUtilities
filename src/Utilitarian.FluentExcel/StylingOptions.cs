using System.Drawing;

namespace Utilitarian.FluentExcel
{
    public class StylingOptions
    {
        public Color HeaderRowColor { get; set; }

        public Color HeaderFontColor { get; set; }

        public short HeaderFontSize { get; set; }

        public Color DataFontColor { get; set; }

        public short DataFontSize { get; set; }

        public Color TotalsRowColor { get; set; }

        public Color TotalsFontColor { get; set; }

        public short TotalsFontSize { get; set; }

        public StylingOptions()
        {
            HeaderRowColor = ExcelStyleDefaults.HeaderRowColor;
            HeaderFontColor = ExcelStyleDefaults.HeaderFontColor;
            HeaderFontSize = ExcelStyleDefaults.HeaderFontSize;

            DataFontColor = ExcelStyleDefaults.DataFontColor;
            DataFontSize = ExcelStyleDefaults.DataFontSize;

            TotalsRowColor = ExcelStyleDefaults.TotalsRowColor;
            TotalsFontColor = ExcelStyleDefaults.TotalsFontColor;
            TotalsFontSize = ExcelStyleDefaults.TotalsFontSize;
        }

        private static class ExcelStyleDefaults
        {
            public static Color HeaderRowColor => Color.MediumSeaGreen;

            public static Color HeaderFontColor => Color.White;

            public static Color DataFontColor => Color.Black;

            public static Color TotalsRowColor => Color.MediumSeaGreen;

            public static Color TotalsFontColor => Color.Black;

            public static short HeaderFontSize => 12;

            public static short DataFontSize => 11;

            public static short TotalsFontSize => 12;
        }
    }
}
