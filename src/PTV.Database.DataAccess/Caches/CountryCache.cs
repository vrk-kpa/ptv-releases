using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(ICountryCache), RegisterType.Singleton)]
    internal class CountryCache: ICacheAfterChangeRefresh, ICountryCache
    {
        private readonly IResolveManager resolveManager;
        private Dictionary<string, Country> cacheByCode;

        public CountryCache(IResolveManager resolveManager)
        {
            RefreshableCaches.Add(this);
            this.resolveManager = resolveManager;
            CreateCache();
        }

        private void CreateCache()
        {
            resolveManager.RunInScope(rm =>
            {
                var contextManager = rm.Resolve<IContextManager>();

                contextManager.ExecuteIsolatedReader(unitOfWork =>
                {
                    var repository = unitOfWork.CreateRepository<ICountryRepository>();
                    var all = repository.All().Include(x => x.CountryNames).AsNoTracking().ToList();
                    cacheByCode = all.ToDictionary(x => x.Code, x => x);
                });
            });
        }

        public void CheckAndRefresh()
        {
            CreateCache();
        }

        public List<Country> GetAll()
        {
            return cacheByCode.Values.ToList();
        }

        public Country GetByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new Exception("Cannot get country from cache for empty/null code");
            }

            return cacheByCode[code];
        }
    }
}
