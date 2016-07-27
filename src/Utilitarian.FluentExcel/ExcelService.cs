using NPOI.HSSF.UserModel;

namespace Utilitarian.FluentExcel
{
    public class ExcelService : IExcelService
    {
        public HSSFWorkbook CreateWorkbook()
        {
            return new HSSFWorkbook();
        }
    }
}
