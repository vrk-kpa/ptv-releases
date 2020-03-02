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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Domain.Model.Models;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models.Attributes;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.TranslationOrder
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, VmTranslationOrderEntityBase>), RegisterType.Transient)]
    internal class ServiceLanguageAvailabilityTranslator : Translator<ServiceVersioned, VmTranslationOrderEntityBase>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public ServiceLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmTranslationOrderEntityBase TranslateEntityToVm(ServiceVersioned entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceVersioned TranslateVmToEntity(VmTranslationOrderEntityBase vModel)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            var names =
                from ln in vModel.LanguagesAvailabilities.Where(lang => lang.IsNew)
                let language = " [" + languageCache.GetByValue(ln.LanguageId).ToUpper() + "]"
                let name = vModel.SourceName ?? string.Empty
                select new VmName
                {
                    LocalizationId = ln.LanguageId,
                    Name = name.Length + language.Length < AttributeValue.ServiceNameMaxLength
                        ? name + language
                        : name.Substring(0, name.Length - language.Length) + language,
                    TypeId = nameTypeId
                };

            var translationDefinition = CreateViewModelEntityDefinition<ServiceVersioned>(vModel)
                .UseDataContextUpdate(i => true, i => o => vModel.Id == o.Id)
                .UseVersioning<ServiceVersioned, Service>(o => o, vModel.KeepInPreviousState ? VersioningMode.KeepInPreviousState | VersioningMode.KeepStateOfLanguages : 0)
                .AddLanguageAvailability(i => i, o => o)
                .AddCollectionWithKeep(i => names, o => o.ServiceNames, t => true);

            var entity = translationDefinition.GetFinal();
            return entity;
        }
    }
}
