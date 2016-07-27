namespace Utilitarian.FluentExcel.Attributes
{
    public interface IExportAttribute
    {
        bool ShouldExport(object collection);
    }
}
