using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

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
                throw new Exception($"Supplied value could not be converted to type {typeOfT.Name}", exception);
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
            return (T)(object)Guid.Parse(@object.ToString());
        }

        public static string GetHash<T>(this object @object) where T : HashAlgorithm, new()
        {
            var cryptoServiceProvider = new T();

            return ComputeHash(@object, cryptoServiceProvider);
        }

        public static string GetMd5Hash(this object @object)
        {
            return GetHash<MD5CryptoServiceProvider>(@object);
        }

        public static string GetSha1Hash(this object @object)
        {
            return GetHash<SHA1CryptoServiceProvider>(@object);
        }

        private static string ComputeHash<T>(object @object, T cryptoServiceProvider) where T : HashAlgorithm, new()
        {
            var serializer = new DataContractSerializer(@object.GetType());

            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, @object);
                cryptoServiceProvider.ComputeHash(memoryStream.ToArray());

                return Convert.ToBase64String(cryptoServiceProvider.Hash);
            }
        }
    }
}
