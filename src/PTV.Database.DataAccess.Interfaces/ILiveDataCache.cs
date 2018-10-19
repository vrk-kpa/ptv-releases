using System.Collections.Generic;

namespace PTV.Database.DataAccess.Interfaces
{
    public interface ILiveDataCache
    {
        void InitRefreshCache();
    }

    public interface ILiveDataCache<TKey, TData> : ILiveDataCache
    {
        Dictionary<TKey, TData> GetData();
    }
}