using System.Threading.Tasks;
using System;

namespace Generic.Cache.Abstract
{
    public interface IRuntimeCache<T>
    {
        T GetOrAdd(string key, T item);

        Task<T> GetOrAddAsync(string key, Func<Task<T>> function);
    }
}
