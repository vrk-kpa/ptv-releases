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
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(ILocalizationMessagesCache), RegisterType.Singleton)]
    internal class LocalizationMessagesCache : LiveDataCache<string, IVmLanguageMessages>, ILocalizationMessagesCache
    {
        private readonly ILoadMessagesService loadService;

        public LocalizationMessagesCache(ILoadMessagesService loadService)
        {
            MinRefreshInterval = 15;
            this.loadService = loadService;
        }

        protected override void LoadData()
        {
            var messages = loadService.Load(Data != null && CacheBuildTime != DateTime.MinValue ? CacheBuildTime : (DateTime?) null);
            if (messages != null)
            {
                Data = messages.ToDictionary(x => x.LanguageCode);
                CacheBuildTime = Data.Values.Max(x => x.Modified);
            }
        }

        protected override bool HasNewData(IUnitOfWork unitOfWork)
        {
            throw new NotImplementedException();
        }
    }
}
