using NPOI.HSSF.UserModel;

namespace Utilitarian.FluentExcel
{
    public interface IExcelService
    {
        HSSFWorkbook CreateWorkbook();
    }
}
