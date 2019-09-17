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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework;
using IName = PTV.Domain.Model.Models.Interfaces.V2.IName;

namespace PTV.Database.DataAccess.Translators
{
    [RegisterService(typeof(CommonTranslatorHelper), RegisterType.Scope)]
    internal class CommonTranslatorHelper
    {

        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        
        public CommonTranslatorHelper(ICacheManager cacheManager)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }
        
        public VmName CreateName<T>(string language, string value, T vModel, NameTypeEnum typeEnum)
            where T : IVmEntityBase
        {
            return new VmName
            {
                Name = value,
                TypeId = typesCache.Get<NameType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language)
            };
        }
        
        public IEnumerable<VmDispalyNameType> CreateDispalyNameTypes<T>(T model) 
            where T : IName, IAlternateName, IVmEntityBase
        {
            var languages = model.Name?.Keys;
            return languages?.Select(lang =>
            {
                var alternateName = model.AlternateName?.TryGet(lang);
                return new VmDispalyNameType
                {
                    OwnerReferenceId = model.Id,
                    LocalizationId = languageCache.Get(lang),
                    NameTypeId = model.IsAlternateNameUsedAsDisplayName != null &&
                                 model.IsAlternateNameUsedAsDisplayName.Contains(lang.ToString()) &&
                                 !string.IsNullOrEmpty(alternateName) ?
                        typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString()) :
                        typesCache.Get<NameType>(NameTypeEnum.Name.ToString())
                };
            });
        }
        
        public VmDescription CreateDescription(string language, string value, Guid? id, DescriptionTypeEnum typeEnum)
        {
            return new VmDescription
            {
                Description = value,
                TypeId = typesCache.Get<DescriptionType>(typeEnum.ToString()),
                OwnerReferenceId = id,
                LocalizationId = languageCache.Get(language)
            };
        }
    }
}