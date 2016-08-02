using System.Collections.Generic;

namespace Utilitarian.FluentExcel
{
    public static class EnumerableExtensions
    {
        public static NpoiWorkbook ToExcelWorkBook<T>(this IEnumerable<T> collection, string workSheetName, StylingOptions stylingOptions = null)
        {
            var workbook = new NpoiWorkbook();

            return workbook.AddWorkSheet(collection, workSheetName, stylingOptions);
        }
    }
}
