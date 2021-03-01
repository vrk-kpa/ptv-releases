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
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ICountryService), RegisterType.Transient)]
    internal class CountryService : ICountryService
    {
        private readonly IContextManager contextManager;
        private readonly ITranslationEntity translationEntToVm;

        public CountryService(IContextManager contextManager, ITranslationEntity translationEntToVm)
        {
            this.contextManager = contextManager;
            this.translationEntToVm = translationEntToVm;
        }

        public IVmListItemsData<IVmCountry> GetCountries(string searchedCode)
        {
            IReadOnlyList<IVmCountry> result = new List<IVmCountry>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var searchCode = searchedCode.ToLower();
                var countryRep = unitOfWork.CreateRepository<ICountryRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(countryRep.All(), i => i.Include(j => j.CountryNames));

                if (!string.IsNullOrEmpty(searchCode))
                {
                    resultTemp = resultTemp
                                    .Where(x => x.Code.ToLower().Contains(searchedCode) ||
                                          x.CountryNames.Any(y => y.Name.ToLower().Contains(searchCode)))
                                    .OrderBy(x => x.Code)
                                    .Take(CoreConstants.MaximumNumberOfAllItems);

                    result = translationEntToVm.TranslateAll<Country, VmCountry>(resultTemp);
                }
            });

            return new VmListItemsData<IVmCountry> (result);
        }

        public IList<VmOpenApiDialCodeListItem> GetCountryCodeList()
        {
            IReadOnlyList<VmOpenApiDialCodeListItem> result = new List<VmOpenApiDialCodeListItem>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var countryRep = unitOfWork.CreateRepository<ICountryRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(countryRep.All(), i => i.Include(j => j.CountryNames).Include(j => j.DialCodes));

                resultTemp = resultTemp.OrderBy(x => x.Code);

                result = translationEntToVm.TranslateAll<Country, VmOpenApiDialCodeListItem>(resultTemp);
            });

            return new List<VmOpenApiDialCodeListItem>(result);
        }
    }
}
