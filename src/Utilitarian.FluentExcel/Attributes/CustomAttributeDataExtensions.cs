using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Utilitarian.FluentExcel.Attributes
{
    public static class CustomAttributeDataExtensions
    {
        public static T CreateInstance<T>(this CustomAttributeData data)
        {
            var arguments = data.ConstructorArguments.GetConstructorValues().ToArray();
            var attribute = (T)data.Constructor.Invoke(arguments);

            if (data.NamedArguments == null) return attribute;

            foreach (var namedArgument in data.NamedArguments)
            {
                var propertyInfo = namedArgument.MemberInfo as PropertyInfo;
                var value = namedArgument.TypedValue.GetArgumentValue();

                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(attribute, value, null);
                }
                else
                {
                    var fieldInfo = namedArgument.MemberInfo as FieldInfo;
                    fieldInfo?.SetValue(attribute, value);
                }
            }

            return attribute;
        }

        private static IEnumerable<object> GetConstructorValues(this IEnumerable<CustomAttributeTypedArgument> arguments)
        {
            return from argument in arguments select argument.GetArgumentValue();
        }

        private static object GetArgumentValue(this CustomAttributeTypedArgument argument)
        {
            var value = argument.Value;
            var collectionValue = value as ReadOnlyCollection<CustomAttributeTypedArgument>;
            return collectionValue != null
                ? ConvertCustomAttributeTypedArgumentArray(collectionValue, argument.ArgumentType.GetElementType())
                : value;
        }

        private static Array ConvertCustomAttributeTypedArgumentArray(this IEnumerable<CustomAttributeTypedArgument> arguments, Type elementType)
        {
            var valueArray = arguments.Select(x => x.Value).ToArray();
            var newArray = Array.CreateInstance(elementType, valueArray.Length);
            Array.Copy(valueArray, newArray, newArray.Length);

            return newArray;
        }
    }
}