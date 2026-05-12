using System.Threading.Tasks;
using System;

namespace Generic.Cache.Abstract
{
    public interface IRuntimeCache<T>
    {
        Task<T> GetOrAddAsync(string key, Func<Task<T>> function);
    }
}
