using System;

namespace Utilitarian.Caching
{
    public abstract class CacheServiceBase
    {
        public T Get<T>(string key)
        {
            T value;

            if (!TryGet(key, out value)) throw new Exception(string.Format("Value for key \"{0}\" could not be found in the {1}", key, GetType().Name));

            return value;
        }

        public abstract bool TryGet<T>(string key, out T value);
    }
}
