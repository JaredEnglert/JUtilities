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
            public static Color HeaderRowColor
            {
                get { return Color.DarkSlateBlue; }
            }

            public static Color HeaderFontColor
            {
                get { return Color.White; }
            }

            public static Color DataFontColor
            {
                get { return Color.Black; }
            }

            public static Color TotalsRowColor
            {
                get { return Color.LightGray; }
            }

            public static Color TotalsFontColor
            {
                get { return Color.Black; }
            }

            public static short HeaderFontSize
            {
                get { return 12; }
            }

            public static short DataFontSize
            {
                get { return 11; }
            }

            public static short TotalsFontSize
            {
                get { return 12; }
            }
        }
    }
}
