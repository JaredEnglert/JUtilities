using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using Utilitarian.FluentExcel.Attributes;

namespace Utilitarian.FluentExcel
{
    public class NpoiWorkbook
    {
        private readonly HSSFWorkbook _hssfWorkbook;

        public NpoiWorkbook()
        {
            _hssfWorkbook = new HSSFWorkbook();
        }

        public NpoiWorkbook AddWorkSheet<T>(IEnumerable<T> collection, string workSheetName, StylingOptions stylingOptions = null)
        {
            GetXlColor(Color.Azure);

            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (string.IsNullOrWhiteSpace(workSheetName)) throw new ArgumentNullException(nameof(workSheetName));

            if (stylingOptions == null) stylingOptions = new StylingOptions();

            var enumeratedCollection = collection.ToList();

            var type = typeof(T);
            var properties = type.GetProperties()
                .Where(p => p.CustomAttributes.All(a =>
                    !a.AttributeType.IsSubclassOf(typeof(ExportAttributeBase))
                    || a.CreateInstance<ExportAttributeBase>().ShouldExport(collection))
                ).ToList();

            var worksheet = _hssfWorkbook.CreateSheet(workSheetName);
            var columnFormatters = new Dictionary<int, short>();
            var styleCache = new Dictionary<string, ICellStyle>();
            
            if (stylingOptions.ShadeAlternateRows) ShadeAlternatingRows(worksheet, stylingOptions, enumeratedCollection.Count, properties.Count);

            var totalsColumns = CreateHeader(worksheet, type, properties, stylingOptions, columnFormatters, styleCache);
            CreateDataRows(worksheet, properties, enumeratedCollection, stylingOptions, columnFormatters, styleCache);
            CreateTotals(worksheet, totalsColumns, properties, enumeratedCollection.Count, stylingOptions, styleCache);
            
            return this;
        }

        public MemoryStream ToMemoryStream()
        {
            var stream = new MemoryStream();
            _hssfWorkbook.Write(stream);
            stream.Position = 0;

            return stream;
        }

        private short GetXlColor(Color color)
        {
            var xlPalette = _hssfWorkbook.GetCustomPalette();
            var xlColour = xlPalette.FindColor(color.R, color.G, color.B) ?? xlPalette.FindSimilarColor(color.R, color.G, color.B);

            return xlColour.Indexed;
        }
        
        private Collection<TotalsColumn> CreateHeader(ISheet worksheet, Type type, IList<PropertyInfo> properties, StylingOptions stylingOptions,
            Dictionary<int, short> columnFormatters, Dictionary<string, ICellStyle> styleCache)
        {
            var headerRowColorId = GetXlColor(stylingOptions.HeaderRowColor);
            var headerFontColorId = GetXlColor(stylingOptions.HeaderFontColor);
            var headerCellStyle = GetCellStyle(0, headerFontColorId, stylingOptions.HeaderFontSize, styleCache, true, headerRowColorId, HorizontalAlignment.Center);
            var headerRow = worksheet.CreateRow(0);
            var totalsColumns = new Collection<TotalsColumn>();

            for (var columnIndex = 0; columnIndex < properties.Count; columnIndex++)
            {
                var displayNameAttribute = Attribute.GetCustomAttribute(type.GetProperty(properties[columnIndex].Name), typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                headerRow.CreateCell(columnIndex).SetCellValue(displayNameAttribute == null ? properties[columnIndex].Name : displayNameAttribute.DisplayName);
                headerRow.Cells[columnIndex].CellStyle = headerCellStyle;

                var formatAttribute = Attribute.GetCustomAttribute(type.GetProperty(properties[columnIndex].Name), typeof(FormatAttribute)) as FormatAttribute;
                var format = GetFormatId(formatAttribute);
                columnFormatters.Add(columnIndex, format);

                var totalsAttribute = Attribute.GetCustomAttribute(type.GetProperty(properties[columnIndex].Name), typeof(FormulaAttribute)) as FormulaAttribute;
                if (totalsAttribute != null)
                {
                    totalsColumns.Add(new TotalsColumn
                    {
                        ColumnIndex = columnIndex,
                        Format = format,
                        TotalType = totalsAttribute.FormulaType
                    });
                }
            }

            worksheet.CreateFreezePane(0, 1);

            return totalsColumns;
        }

        private void CreateDataRows<T>(ISheet worksheet, IList<PropertyInfo> properties,
            IReadOnlyList<T> enumeratedCollection, StylingOptions stylingOptions, Dictionary<int, short> columnFormatters, Dictionary<string, ICellStyle> styleCache)
        {
            var dataFontColorId = GetXlColor(stylingOptions.DataFontColor);

            for (var rowIndex = 0; rowIndex < enumeratedCollection.Count; rowIndex++)
            {
                var row = worksheet.CreateRow(rowIndex + 1);

                foreach (var columnIndex in properties.Select(properties.IndexOf))
                {
                    SetCellValue(row.CreateCell(columnIndex), properties[columnIndex].GetValue(enumeratedCollection[rowIndex]), properties[columnIndex].PropertyType);
                    row.Cells[columnIndex].CellStyle = GetCellStyle(columnFormatters[columnIndex], dataFontColorId, stylingOptions.DataFontSize, styleCache);
                }
            }

            for (var i = 0; i < properties.Count; i++)
            {
                worksheet.AutoSizeColumn(i);
                worksheet.SetColumnWidth(i, worksheet.GetColumnWidth(i) + 1000);
            }
        }

        private void CreateTotals(ISheet worksheet, IList<TotalsColumn> totalsColumns, IList<PropertyInfo> properties, int rowCount,
            StylingOptions stylingOptions, Dictionary<string, ICellStyle> styleCache)
        {
            if (totalsColumns == null || !totalsColumns.Any()) return;

            var totalsRowColorId = GetXlColor(stylingOptions.TotalsRowColor);
            var totalsFontColorId = GetXlColor(stylingOptions.TotalsFontColor);
            var totalsRow = worksheet.CreateRow(rowCount + 1);

            foreach (var columnIndex in properties.Select(properties.IndexOf))
            {
                var totalsColumn = totalsColumns.SingleOrDefault(tc => tc.ColumnIndex == columnIndex);
                var cell = totalsRow.CreateCell(columnIndex);

                if (totalsColumn == null)
                {
                    SetCellValue(cell, columnIndex == 0 ? "Totals:" : null, typeof(string));
                    totalsRow.Cells[columnIndex].CellStyle = GetCellStyle(0, totalsFontColorId, stylingOptions.TotalsFontSize, styleCache, true, totalsRowColorId, HorizontalAlignment.Left);
                }
                else
                {
                    cell.SetCellFormula(GetCellFormula(totalsColumn.TotalType, columnIndex, rowCount));
                    totalsRow.Cells[columnIndex].CellStyle =
                        GetCellStyle(totalsColumn.Format, totalsFontColorId, stylingOptions.TotalsFontSize, styleCache, true, totalsRowColorId, HorizontalAlignment.Right);
                }
            }
        }

        private static void SetCellValue(ICell cell, object value, Type type)
        {
            if (value == null)
            {
                cell.SetCellValue(string.Empty);
                return;
            }

            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(DateTime))
            {
                var dateTime = (DateTime)value;
                if (dateTime.Kind == DateTimeKind.Utc)
                {
                    var cst = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                    cell.SetCellValue(TimeZoneInfo.ConvertTimeFromUtc(dateTime, cst));
                }
                else
                {
                    cell.SetCellValue(dateTime);
                }
            }
            else if (type == typeof(decimal)
                || (type == typeof(short))
                || (type == typeof(int))
                || (type == typeof(float))
                || (type == typeof(double)))
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (type == typeof(bool))
            {
                cell.SetCellValue((bool)value);
            }
            else
            {
                cell.SetCellValue(value.ToString());
            }
        }

        private short GetFormatId(FormatAttribute formatAttribute)
        {
            if (formatAttribute == null) return 0;

            var formatId = HSSFDataFormat.GetBuiltinFormat(formatAttribute.FormatString);

            if (formatId != -1) return formatId;

            return _hssfWorkbook.CreateDataFormat().GetFormat(formatAttribute.FormatString);
        }

        private static string GetCellFormula(FormulaType formulaType, int columnIndex, int rowCount)
        {
            switch (formulaType)
            {
                case FormulaType.Sum:
                    return string.Format("SUM({0}2:{0}{1})", GetExcelColumnName(columnIndex), rowCount + 1);
                case FormulaType.Average:
                    return string.Format("AVERAGE({0}2:{0}{1})", GetExcelColumnName(columnIndex), rowCount + 1);
                default:
                    throw new NotImplementedException($"There is no implementation for FormulaType \"{formulaType}\"");
            }
        }

        private static string GetExcelColumnName(int columnIndex)
        {
            var dividend = columnIndex + 1;
            var columnName = string.Empty;

            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }

        private ICellStyle GetCellStyle(short format, short fontColorId, short fontSize, Dictionary<string, ICellStyle> styleCache,
            bool isBold = false, short? rowColorId = null, HorizontalAlignment? horizontalAlignment = null)
        {
            var key = $"{format}:{isBold}:{rowColorId}:{horizontalAlignment}";

            if (styleCache.ContainsKey(key)) return styleCache[key];

            var font = _hssfWorkbook.CreateFont();
            font.FontHeightInPoints = fontSize;
            font.Color = fontColorId;
            if (isBold) font.Boldweight = (short)FontBoldWeight.Bold;

            var cellStyle = _hssfWorkbook.CreateCellStyle();
            cellStyle.SetFont(font);
            cellStyle.DataFormat = format;
            if (horizontalAlignment.HasValue) cellStyle.Alignment = horizontalAlignment.Value;
            if (rowColorId.HasValue)
            {
                cellStyle.FillForegroundColor = rowColorId.Value;
                cellStyle.FillPattern = FillPattern.SolidForeground;
            }

            styleCache.Add(key, cellStyle);

            return cellStyle;
        }

        private void ShadeAlternatingRows(ISheet sheet, StylingOptions stylingOptions, int rowCount, int columnCount)
        {
            var conditionalFormattingRule = sheet.SheetConditionalFormatting.CreateConditionalFormattingRule("MOD(ROW(),2)");

            var patternFormatting = conditionalFormattingRule.CreatePatternFormatting();
            patternFormatting.FillBackgroundColor = GetXlColor(stylingOptions.AlternatingDataRowColor);
            patternFormatting.FillPattern = (short)FillPattern.SolidForeground;

            CellRangeAddress[] regions = {new CellRangeAddress(2, rowCount, 0, columnCount - 1)};

            sheet.SheetConditionalFormatting.AddConditionalFormatting(regions, conditionalFormattingRule);
        }

        private class TotalsColumn
        {
            public int ColumnIndex { get; set; }

            public short Format { get; set; }

            public FormulaType TotalType { get; set; }
        }
    }
}