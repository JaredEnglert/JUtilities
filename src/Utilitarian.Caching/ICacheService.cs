using System;

namespace Utilitarian.Caching
{
    public interface ICacheService
    {
        void Set(string key, object value);

        void SetSingleUse(string key, object value);

        void SetExpiring(string key, object value, TimeSpan expiration, bool rollExpirationOnAccess = false);

        void SetSingleUseExpiring(string key, object value, TimeSpan expiration, bool rollExpirationOnAccess = false);

        T Get<T>(string key);

        bool TryGet<T>(string key, out T value);

        void Clear();
    }
}
