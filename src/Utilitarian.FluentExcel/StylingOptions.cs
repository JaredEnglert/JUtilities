using System.Drawing;

namespace Utilitarian.FluentExcel
{
    public class StylingOptions
    {
        public Color HeaderRowColor { get; set; }

        public Color HeaderFontColor { get; set; }

        public short HeaderFontSize { get; set; }

        public Color AlternatingDataRowColor { get; set; }

        public Color DataFontColor { get; set; }

        public short DataFontSize { get; set; }

        public Color TotalsRowColor { get; set; }

        public Color TotalsFontColor { get; set; }

        public short TotalsFontSize { get; set; }

        public bool ShadeAlternateRows { get; set; }

        public StylingOptions()
        {
            HeaderRowColor = Color.MediumSeaGreen;
            HeaderFontColor = Color.White;
            HeaderFontSize = 12;

            AlternatingDataRowColor = Color.LightGray;
            DataFontColor = Color.Black;
            DataFontSize = 11;

            TotalsRowColor = Color.MediumSeaGreen;
            TotalsFontColor = Color.White;
            TotalsFontSize = 12;

            ShadeAlternateRows = true;
        }
    }
}
