using System;
using System.Runtime.Caching;

namespace ProxyCache
{
    internal class GenericProxyCache<T> where T : new()
    {
        ObjectCache cache = MemoryCache.Default;


        public T Get(string CacheItemName)
        {


            if (!cache.Contains(CacheItemName))
            {
                T newItem = (T)Activator.CreateInstance(typeof(T), CacheItemName);
                cache.Add(CacheItemName, newItem, ObjectCache.InfiniteAbsoluteExpiration);
            }
            return (T)cache.Get(CacheItemName);
        }

        public T Get(string CacheItemName, double dt_seconds)
        {
            if (!cache.Contains(CacheItemName))
            {
                T newItem = (T)Activator.CreateInstance(typeof(T), CacheItemName);
                cache.Add(CacheItemName, newItem, DateTimeOffset.Now.AddSeconds(dt_seconds));
            }
            return (T)cache.Get(CacheItemName);

        }

        public T Get(string CacheItemName, DateTimeOffset dt)
        {
            if (!cache.Contains(CacheItemName))
            {
                
                T newItem = (T)Activator.CreateInstance(typeof(T), CacheItemName);
                cache.Add(CacheItemName, newItem, dt);
            }
            return (T)cache.Get(CacheItemName);
        }

    }
}
