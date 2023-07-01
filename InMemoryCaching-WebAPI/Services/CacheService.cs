using System.Runtime.Caching;

namespace InMemoryCaching_WebAPI.Services
{
    public class CacheService : ICacheService
    {
        private readonly ObjectCache _memoryCache = MemoryCache.Default;
        
        public T GetData<T>(string key)
        {
            try
            {
                if (_memoryCache.Contains(key))
                {
                    var item = (T)_memoryCache.Get(key);
                    return item;
                }

                return default;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var result = true;

            try
            {
                if (!string.IsNullOrEmpty(key) && value is not null)
                {
                    _memoryCache.Set(key, value, expirationTime);
                }
                else
                {
                    result = false;
                }

                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public object RemoveData(string key)
        {
            var result = true;

            try
            {
                if (!string.IsNullOrEmpty(key) && _memoryCache.Contains(key))
                {
                    _memoryCache.Remove(key);
                }
                else
                {
                    result = false;
                }

                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}