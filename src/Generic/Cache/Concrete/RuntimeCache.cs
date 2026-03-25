using Generic.Cache.Abstract;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Generic.Cache.Concrete
{
    [Export(typeof(IRuntimeCache<>))]
    public class RuntimeCache<T> : IRuntimeCache<T>
    {
        readonly ConcurrentDictionary<string, T> _cache = new();

        public T GetOrAdd(string key, T item)
        {
            return _cache.GetOrAdd(key, item);
        }

        public async Task<T> GetOrAddAsync(string key, Func<Task<T>> function)
        {
            if (_cache.TryGetValue(key, out T value))
                return value;

            return _cache.GetOrAdd(key, await function());
        }
    }
}
