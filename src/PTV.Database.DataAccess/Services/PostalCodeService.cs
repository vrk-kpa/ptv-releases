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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IPostalCodeService), RegisterType.Transient)]
    internal class PostalCodeService : ServiceBase, IPostalCodeService
    {
        private readonly IContextManager contextManager;
        private readonly ILanguageCache languageCache;
        private ILogger logger;

        public PostalCodeService(IContextManager contextManager, ITranslationEntity translationEntToVm, ITranslationViewModel translationManagerToEntity, IPublishingStatusCache publishingStatusCache, IUserOrganizationChecker userOrganizationChecker, ILogger<PostalCodeService> logger, ILanguageCache languageCache)
                        : base(translationEntToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.languageCache = languageCache;
        }

        public IVmListItemsData<IVmPostalCode> GetPostalCodes(string searchedCode, bool onlyValid = true)
        {
            IReadOnlyList<IVmPostalCode> result = new List<IVmPostalCode>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var searchCode = searchedCode.ToLower();
                var postalCodeRep = unitOfWork.CreateRepository<IPostalCodeRepository>();
                var qry = postalCodeRep.All();
                if (onlyValid) qry = qry.Where(pc => pc.IsValid);
                var resultTemp = unitOfWork.ApplyIncludes(qry, i => i.Include(j => j.PostalCodeNames));

                if (!string.IsNullOrEmpty(searchCode))
                {
                    resultTemp = resultTemp
                                    .Where(x => x.Code.ToLower().Contains(searchedCode) ||
                                                x.PostalCodeNames.Any(y => y.Name.ToLower().Contains(searchedCode))
                                            )
                                    .OrderBy(x => x.Code)
                                    .Take(CoreConstants.MaximumNumberOfAllItems);

                    result = TranslationManagerToVm.TranslateAll<PostalCode, VmPostalCode>(resultTemp);
                }
            });

            return new VmListItemsData<IVmPostalCode> (result);
        }

        public IVmPostalCode GetPostalCode(Guid postalCodeId)
        {
            var result = new VmPostalCode();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var postalCodeRepository = unitOfWork.CreateRepository<IPostalCodeRepository>();
                result = TranslationManagerToVm.TranslateFirst<PostalCode, VmPostalCode>(
                    unitOfWork.ApplyIncludes(
                        postalCodeRepository.All().Where(pc => pc.IsValid && pc.Id == postalCodeId),
                        i => i.Include(j => j.PostalCodeNames)
                    )
                );

            });
            return result;
        }

        public IVmOpenApiGuidPageVersionBase GetPostalCodeList(int pageNumber, int pageSize)
        {
            var vm = new VmOpenApiCodeListPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            List<PostalCode> postalCodes = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                postalCodes = GetEntitiesForPage<PostalCode>(vm, unitOfWork, q => q.Include(j => j.PostalCodeNames), keySelector: x => x.Code);
            });

            if (postalCodes?.Count > 0)
            {
                vm.ItemList = TranslationManagerToVm.TranslateAll<PostalCode, VmOpenApiCodeListItem>(postalCodes).ToList();
            }

            return vm;
        }
    }
}