using Generic.Cache.Abstract;
using System.ComponentModel.Composition;

namespace Generic.Cache.Concrete
{
    [Export(typeof(IRuntimeCacheFactory))]
    public class RuntimeCacheFactory : IRuntimeCacheFactory
    {
        public IRuntimeCache<T> Create<T>() => new RuntimeCache<T>();
    }
}
