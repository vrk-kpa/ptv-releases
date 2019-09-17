﻿/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;
using System;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IDialCodeService), RegisterType.Transient)]
    internal class DialCodeService : IDialCodeService
    {
        private readonly IContextManager contextManager;
        private readonly ITranslationEntity translationEntToVm;
        private ILogger logger;
        private ILanguageCache languageCache;

        public DialCodeService(IContextManager contextManager, ITranslationEntity translationEntToVm, ILogger<DialCodeService> logger, ICacheManager cacheManager)
        {
            this.contextManager = contextManager;
            this.translationEntToVm = translationEntToVm;
            this.logger = logger;
            this.languageCache = cacheManager.LanguageCache;
        }

        public IVmListItemsData<IVmDialCode> GetDialCodes(string searchedCode)
        {
            IReadOnlyList<IVmDialCode> result = new List<IVmDialCode>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var searchCode = searchedCode.ToLower();
                var dialCodeRep = unitOfWork.CreateRepository<IDialCodeRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(dialCodeRep.All(), i => i.Include(j => j.Country).ThenInclude(j => j.CountryNames));

                if (!string.IsNullOrEmpty(searchCode))
                {
                    resultTemp = resultTemp
                                    .Where(x => x.Code.ToLower().Contains(searchedCode) ||
                                          x.Country.CountryNames.Any(y => y.Name.ToLower().Contains(searchCode)))
                                    .OrderBy(x => x.Code)
                                    .Take(CoreConstants.MaximumNumberOfAllItems);

                    result = translationEntToVm.TranslateAll<DialCode, VmDialCode>(resultTemp);
                }
            });

            return new VmListItemsData<IVmDialCode> (result);
        }

        public IVmDialCode GetDialCode(Guid dialCodeId)
        {
            IVmDialCode result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var dialCodeRepository = unitOfWork.CreateRepository<IDialCodeRepository>();
                result = translationEntToVm.TranslateFirst<DialCode, VmDialCode>(
                    unitOfWork.ApplyIncludes(
                        dialCodeRepository.All().Where(x => x.Id == dialCodeId),
                        i => i.Include(j => j.Country).ThenInclude(j => j.CountryNames)
                    )
                );
            });
            return result;
        }

    }
}