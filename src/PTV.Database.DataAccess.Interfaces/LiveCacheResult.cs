using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Interfaces
{
    public class LiveCacheResult<TKey, TData>
    {
        public Dictionary<TKey, TData> Data { get; }
        public DateTime BuildTime { get; }

        public LiveCacheResult(DateTime buildTime, Dictionary<TKey, TData> data)
        {
            this.BuildTime = buildTime;
            this.Data = data;
        }
    }
}