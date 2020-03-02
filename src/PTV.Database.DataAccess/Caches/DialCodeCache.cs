/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
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
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IDialCodeCache), RegisterType.Singleton)]
    internal class DialCodeCache : IDialCodeCache
    {
        private readonly IContextManager contextManager;
        private readonly ApplicationConfiguration configuration;
        private List<DialCode> defaultDialCodes;
        private Dictionary<Guid, List<string>> countryDialCodes;

        public DialCodeCache(IContextManager contextManager, ApplicationConfiguration configuration)
        {
            RefreshableCaches.Add(this);
            this.contextManager = contextManager;
            this.configuration = configuration;
            CheckAndRefresh();
        }
        public void CheckAndRefresh()
        {
            contextManager.ExecuteReader(unitOfWork =>
            {
                var defaultCountryCode = configuration.GetDefaultCountryCode();
                var dialCodeRep = unitOfWork.CreateRepository<IDialCodeRepository>();
                var dialCodeList = dialCodeRep.All().ToList();
                this.countryDialCodes = dialCodeList.GroupBy(x => x.CountryId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Code).ToList());
                this.defaultDialCodes = unitOfWork.ApplyIncludes(
                    dialCodeRep.All().Where(x => x.Country.Code == defaultCountryCode.ToUpper()),
                    i => i.Include(j => j.Country).ThenInclude(j => j.CountryNames))
                .ToList();
            });
        }

        public string Get(Guid key)
        {
            return countryDialCodes.TryGetOrDefault(key)?.FirstOrDefault();
        }

        public Guid GetByValue(string value)
        {
            return countryDialCodes.FirstOrDefault(x => x.Value.Any(y => y == value)).Key;
        }

        public bool Exists(Guid key)
        {
            return countryDialCodes.ContainsKey(key);
        }

        public List<DialCode> GetDefaultDialCodes()
        {
            return defaultDialCodes;
        }
    }
}
