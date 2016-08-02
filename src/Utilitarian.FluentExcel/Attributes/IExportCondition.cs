namespace Utilitarian.FluentExcel.Attributes
{
    public interface IExportCondition
    {
        bool IsTrue(object collection);
    }
}