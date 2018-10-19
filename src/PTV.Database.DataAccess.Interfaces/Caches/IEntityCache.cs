namespace PTV.Database.DataAccess.Interfaces.Caches
{
    public interface IEntityCache<TKey, TValue>
    {
        TValue Get(TKey key);
        TKey GetByValue(TValue value);
        bool Exists(TKey key);
    }
}