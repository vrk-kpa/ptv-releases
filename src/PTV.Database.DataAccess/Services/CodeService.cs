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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ICodeService), RegisterType.Transient)]
    public class CodeService : ICodeService
    {
        private readonly IContextManager contextManager;
        private readonly ITranslationEntity translationEntToVm;
        private ILanguageCache languageCache;

        public CodeService(IContextManager contextManager, ITranslationEntity translationEntToVm, ILanguageCache languageCache)
        {
            this.languageCache = languageCache;
            this.contextManager = contextManager;
            this.translationEntToVm = translationEntToVm;
        }

        public IVmListItem GetMunicipalityByCode(string code, bool onlyValid = false)
        {
            var result = new VmListItem();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IMunicipalityRepository>();
                var municipalityCodeQry = rep.All().Where(x => x.Code == code);
                if (onlyValid) municipalityCodeQry = municipalityCodeQry.Where(m => m.IsValid);
                var municipalityCode = municipalityCodeQry.FirstOrDefault();
                if (municipalityCode != null)
                {
                    result = translationEntToVm.Translate<Municipality, VmListItem>(municipalityCode);
                }
            });

            return result;
        }

        public string GetCountryByCode(string code)
        {
            var result = string.Empty;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<ICountryRepository>();
                var country = rep.All().Where(x => x.Code == code).FirstOrDefault();
                if (country != null)
                    result = country.Code;
            });

            return result;
        }

        public string GetPostalCodeByCode(string code, bool onlyValid = false)
        {
            var result = string.Empty;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IPostalCodeRepository>();
                var postalCodeQry = rep.All().Where(x => x.Code == code).AsQueryable();
                if (onlyValid) postalCodeQry = postalCodeQry.Where(pc => pc.IsValid);
                var postalCode = postalCodeQry.ToList();
                result = (postalCode?.Count > 0) ? postalCode.FirstOrDefault().Code : null;
            });

            return result;
        }

//        public Guid GetLanguageByCode(string code)
//        {
//            return languageCache.Get(code);
////            var result = new VmListItem();
////            contextManager.ExecuteReader(unitOfWork =>
////            {
////                var rep = unitOfWork.CreateRepository<ILanguageRepository>();
////                result = translationEntToVm.TranslateFirst<Language, VmListItem>(rep.All().Where(x => x.Code == code.ToLower() && x.IsValid == true));
////            });
////            return result;
//        }

        /// <summary>
        /// Returns a list of languages that do not exist (within codeList).
        /// </summary>
        /// <param name="codeList"></param>
        /// <returns></returns>
        public List<string> CheckLanguages(IList<string> codeList)
        {
            // Cannot use AllowedLanguages since it is containing only languages that are allowed within UI.
            // In this case we need the check to be done against all the valid languages. (Used when validating selected languages for a service or channel)
            return codeList.Except(this.languageCache.LanguageCodes).ToList();
//
//
//            List<string> existingCodes = null;
//            contextManager.ExecuteReader(unitOfWork =>
//            {
//                var query = unitOfWork.CreateRepository<ILanguageRepository>().All().Where(l => codeList.Contains(l.Code) && l.IsValid == true);
//
//                existingCodes = query.Select(s => s.Code).ToList();
//            });
//
//            if (existingCodes?.Count > 0) return codeList.Where(i => !existingCodes.Contains(i)).ToList();
//
//            return codeList;
        }

        public IVmDialCode GetDialCode(string code)
        {
            var result = new VmDialCode();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IDialCodeRepository>();
                result = translationEntToVm.TranslateFirst<DialCode, VmDialCode>(rep.All().Where(x => x.Code == code));
            });
            return result;
        }

        public IVmOpenApiArea GetAreaByCodeAndType(string code, string type)
        {
            var result = new VmOpenApiArea();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IAreaRepository>();
                var area = rep.All().Where(x => x.Code == code && x.AreaType.Code == type).FirstOrDefault();
                if (area != null)
                {
                    result = translationEntToVm.Translate<Area, VmOpenApiArea>(area);
                }
            });

            return result;
        }

        public Guid? GetAreaIdByCodeAndType(string code, string type)
        {
            Guid? id = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IAreaRepository>();
                var area = rep.All().Where(x => x.Code == code && x.AreaType.Code == type).FirstOrDefault();
                if (area != null)
                {
                    id = area.Id;
                }
            });

            return id;
        }
    }
}
