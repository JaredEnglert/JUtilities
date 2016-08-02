using System;

namespace Utilitarian.FluentExcel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DoNotExportAttribute : ExportAttributeBase
    {
        public override bool ShouldExport(object collection)
        {
            return false;
        }
    }
}
