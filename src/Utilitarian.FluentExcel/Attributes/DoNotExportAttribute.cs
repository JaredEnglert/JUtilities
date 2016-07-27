using System;

namespace Utilitarian.FluentExcel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DoNotExportAttribute : Attribute, IExportAttribute
    {
        public bool ShouldExport(object collection)
        {
            return false;
        }
    }
}
