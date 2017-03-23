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
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using System.Linq;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ICodeService), RegisterType.Transient)]
    public class CodeService : ICodeService
    {
        private readonly IContextManager contextManager;
        private readonly ITranslationEntity translationEntToVm;
        private ILogger logger;

        public CodeService(IContextManager contextManager, ITranslationEntity translationEntToVm, ILogger<CodeService> logger)
        {
            this.contextManager = contextManager;
            this.translationEntToVm = translationEntToVm;
            this.logger = logger;
        }

        public IVmListItem GetMunicipalityByCode(string code)
        {
            var result = new VmListItem();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IMunicipalityRepository>();
                var municipalityCode = rep.All().Where(x => x.Code == code).FirstOrDefault();
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

        public string GetPostalCodeByCode(string code)
        {
            var result = string.Empty;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IPostalCodeRepository>();
                var postalCode = rep.All().Where(x => x.Code == code).ToList();
                result = (postalCode?.Count > 0) ? postalCode.FirstOrDefault().Code : null;
            });

            return result;
        }

        public IVmListItem GetLanguageByCode(string code)
        {
            var result = new VmListItem();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<ILanguageRepository>();
                result = translationEntToVm.TranslateFirst<Language, VmListItem>(rep.All().Where(x => x.Code == code.ToLower()));
            });
            return result;
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
    }
}
