using System;

namespace Utilitarian.FluentExcel.Attributes
{
    public abstract class ExportAttributeBase : Attribute
    {
        public abstract bool ShouldExport(object collection);
    }
}
