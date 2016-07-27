using System;
using Utilitarian.Extensions;

namespace Utilitarian.FluentExcel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConditionalExportAttribute : Attribute, IExportAttribute
    {
        private readonly Type _conditionType;

        public ConditionalExportAttribute(Type conditionType)
        {
            if (!conditionType.ImplementsInterface<IExportCondition>()) throw new ArgumentException("Type must implement interface IExportCondition", "conditionType");

            _conditionType = conditionType;
        }

        public bool ShouldExport(object collection)
        {
            return ((IExportCondition)Activator.CreateInstance(_conditionType)).IsTrue(collection);
        }
    }
}