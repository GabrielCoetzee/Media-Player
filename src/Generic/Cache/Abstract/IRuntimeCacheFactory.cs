namespace Generic.Cache.Abstract
{
    public interface IRuntimeCacheFactory
    {
        IRuntimeCache<T> Create<T>();
    }
}
