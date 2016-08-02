using System;

namespace Utilitarian.Caching
{
    public abstract class CacheServiceBase
    {
        public T Get<T>(string key)
        {
            T value;

            if (!TryGet(key, out value)) throw new Exception($"Value for key \"{key}\" could not be found in the {GetType().Name}");

            return value;
        }

        public abstract bool TryGet<T>(string key, out T value);
    }
}
