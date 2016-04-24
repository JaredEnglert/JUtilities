using System;
using Utilities.Extensions;

namespace Utilities.Caching
{
    public class CacheRepository : IDisposable
    {
        private readonly ICacheService _cacheService;

        private static readonly TimeSpan _defaultTimeSpan = new TimeSpan(0, 0, 15, 0);

        public CacheRepository(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public T Get<T>(string key, Func<T> getFromDataStore, TimeSpan? expiration = null, bool rollExpirationOnAccess = false)
        {
            return Get<T, string, string, string, string, string>(key, (a1, a2, a3, a4, a5) => getFromDataStore(), null, null, null, null, null, expiration, rollExpirationOnAccess);
        }

        public T Get<T, T1>(string key, Func<T1, T> getFromDataStore, T1 arg1, TimeSpan? expiration = null, bool rollExpirationOnAccess = false)
        {
            return Get<T, T1, string, string, string, string>(key, (a1, a2, a3, a4, a5) => getFromDataStore(a1), arg1, null, null, null, null, expiration, rollExpirationOnAccess);
        }

        public T Get<T, T1, T2>(string key, Func<T1, T2, T> getFromDataStore, T1 arg1, T2 arg2, TimeSpan? expiration = null, bool rollExpirationOnAccess = false)
        {
            return Get<T, T1, T2, string, string, string>(key, (a1, a2, a3, a4, a5) => getFromDataStore(a1, a2), arg1, arg2, null, null, null, expiration, rollExpirationOnAccess);
        }

        public T Get<T, T1, T2, T3>(string key, Func<T1, T2, T3, T> getFromDataStore, T1 arg1, T2 arg2, T3 arg3, TimeSpan? expiration = null, bool rollExpirationOnAccess = false)
        {
            return Get<T, T1, T2, T3, string, string>(key, (a1, a2, a3, a4, a5) => getFromDataStore(a1, a2, a3), arg1, arg2, arg3, null, null, expiration, rollExpirationOnAccess);
        }

        public T Get<T, T1, T2, T3, T4>(string key, Func<T1, T2, T3, T4, T> getFromDataStore, T1 arg1, T2 arg2, T3 arg3, T4 arg4, TimeSpan? expiration = null,
            bool rollExpirationOnAccess = false)
        {
            return Get<T, T1, T2, T3, T4, string>(key, (a1, a2, a3, a4, a5) => getFromDataStore(a1, a2, a3, a4), arg1, arg2, arg3, arg4, null, expiration, rollExpirationOnAccess);
        }

        public T Get<T, T1, T2, T3, T4, T5>(string key, Func<T1, T2, T3, T4, T5, T> getFromDataStore, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, TimeSpan? expiration = null,
            bool rollExpirationOnAccess = false)
        {
            T value;

            if (_cacheService.TryGet(key, out value)) return value.ToType<T>();

            value = getFromDataStore(arg1, arg2, arg3, arg4, arg5);

            _cacheService.SetExpiring(key, value, expiration ?? _defaultTimeSpan, rollExpirationOnAccess);

            return value;
        }

        public void Dispose()
        {
            if (_cacheService != null && _cacheService.GetType().ImplementsInterface<IDisposable>()) ((IDisposable)_cacheService).Dispose();
        }
    }
}
