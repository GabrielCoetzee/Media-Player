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
        readonly ConcurrentDictionary<string, Lazy<Task<T>>> _cache = new();

        public Task<T> GetOrAddAsync(string key, Func<Task<T>> function)
        {
            return _cache.GetOrAdd(key, _ => new Lazy<Task<T>>(function)).Value;
        }
    }
}
