/**
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Domain.Model.Models.Interfaces;
using PTV.ExternalSources;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Database.DataAccess.Interfaces.Translators;
using Microsoft.EntityFrameworkCore;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ILanguageService), RegisterType.Transient)]
    internal class LanguageService : ILanguageService
    {
        private readonly ResourceManager resourceManager;
        private ICommonService commonService;
        private IContextManager contextManager;
        private readonly ITranslationEntity translationEntToVm;

        public LanguageService(ResourceManager resourceManager, ICommonService commonService,
            IContextManager contextManager, ITranslationEntity translationEntToVm)
        {
            this.resourceManager = resourceManager;
            this.commonService = commonService;
            this.contextManager = contextManager;
            this.translationEntToVm = translationEntToVm;
        }

        public VmWorldLangauges GetWorldLanguages()
        {
            return new VmWorldLangauges();
                // {WorldLanguages = resourceManager.GetDesrializedJsonResource<List<VmLanguageCode>>(JsonResources.LanguageCodes)};
        }

        public IVmListItemsData<VmListItem> GetTranslationLanguages()
        {
            return new VmListItemsData<VmListItem>(commonService.GetTranslationLanguages());
        }

        public IList<VmOpenApiCodeListItem> GetLanguageCodeList()
        {
            IReadOnlyList<VmOpenApiCodeListItem> result = new List<VmOpenApiCodeListItem>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var languageRep = unitOfWork.CreateRepository<ILanguageRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(languageRep.All(), i => i.Include(j => j.Names));

                resultTemp = resultTemp.OrderBy(x => x.Code);

                result = translationEntToVm.TranslateAll<Language, VmOpenApiCodeListItem>(resultTemp);
            });

            return new List<VmOpenApiCodeListItem>(result);
        }
    }
}
