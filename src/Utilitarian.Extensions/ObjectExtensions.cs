using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utilitarian.Extensions
{
    public static class ObjectExtensions
    {
        public static T ToType<T>(this object @object)
        {
            if (@object == null) throw new ArgumentException("Null is not convertable.");

            var typeOfT = typeof(T);

            if (typeOfT.IsNullable()) throw new ArgumentException("Can not convert to nullable type.");

            try
            {
                if (typeOfT.IsEnum) return CastToEnum<T>(typeOfT, @object);
                if (typeOfT.IsGuid()) return CastToGuid<T>(@object);

                return (T)Convert.ChangeType(@object, typeof(T));
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Supplied value could not be converted to type {0}", typeOfT.Name), exception);
            }
        }

        public static byte[] SerializeToByteArray(this object @object)
        {
            if (@object == null) throw new ArgumentException("Null is not convertable.");

            var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(memoryStream, @object);

            return memoryStream.ToArray();
        }

        public static void ToFile(this object @object, string filePath)
        {
            @object.SerializeToByteArray().ToFile(filePath);
        }

        private static T CastToEnum<T>(Type type, object @object)
        {
            int enumAsInt;
            var enumAsString = @object.ToString();

            return int.TryParse(enumAsString, out enumAsInt)
                ? (T)Enum.ToObject(typeof(T), enumAsInt)
                : (T)Enum.Parse(type, enumAsString);
        }

        private static T CastToGuid<T>(object @object)
        {
            return (T)((object)Guid.Parse(@object.ToString()));
        }
    }
}
