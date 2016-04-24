using System;

namespace Utilities.Extensions
{
    public static class TypeExtensions
    {
        public static bool ImplementsInterface<T>(this Type type)
        {
            var interfaceType = typeof(T);

            if (!interfaceType.IsInterface) throw new Exception(string.Format("Type \"{0}\" is not an interface", interfaceType.Name));

            return interfaceType.IsAssignableFrom(type);
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsGuid(this Type type)
        {
            return type == typeof(Guid);
        }
    }
}
