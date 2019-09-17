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

using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, VmEntityNames>), RegisterType.Transient)]
    internal class ServiceEntityNameTranslator : Translator<ServiceVersioned, VmEntityNames>
    {
        private ILanguageCache languageCache;
        private ITypesCache typesCache;
        public ServiceEntityNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmEntityNames TranslateEntityToVm(ServiceVersioned entity)
        {
            var model = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => i.LanguageAvailabilities, o => o.LanguagesAvailabilities).GetFinal();
            model.Names =
                entity.ServiceNames.Where(
                    x => x.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())
                    && entity.LanguageAvailabilities.Select(y => y.LanguageId).Contains(x.LocalizationId))
                    .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name);
            return model;
        }

        public override ServiceVersioned TranslateVmToEntity(VmEntityNames vModel)
        {
            throw new System.NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmEntityNames>), RegisterType.Transient)]
    internal class ChannelEntityNameTranslator : Translator<ServiceChannelVersioned, VmEntityNames>
    {
        private ILanguageCache languageCache;
        private ITypesCache typesCache;
        public ChannelEntityNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmEntityNames TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var model = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => i.LanguageAvailabilities, o => o.LanguagesAvailabilities).GetFinal();
            model.Names =
                entity.ServiceChannelNames.Where(
                    x => x.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())
                    && entity.LanguageAvailabilities.Select(y => y.LanguageId).Contains(x.LocalizationId))
                    .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name);
            return model;
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmEntityNames vModel)
        {
            throw new System.NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmEntityNames>), RegisterType.Transient)]
    internal class OrganizationEntityNameTranslator : Translator<OrganizationVersioned, VmEntityNames>
    {
        private ILanguageCache languageCache;
        private ITypesCache typesCache;
        public OrganizationEntityNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmEntityNames TranslateEntityToVm(OrganizationVersioned entity)
        {
            var model = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => i.LanguageAvailabilities, o => o.LanguagesAvailabilities).GetFinal();
            model.Names =
                entity.OrganizationNames.Where(
                    x => x.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())
                    && entity.LanguageAvailabilities.Select(y => y.LanguageId).Contains(x.LocalizationId))
                    .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name);
            return model;
        }

        public override OrganizationVersioned TranslateVmToEntity(VmEntityNames vModel)
        {
            throw new System.NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmEntityNames>), RegisterType.Transient)]
    internal class GeneralDescriptionEntityNameTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmEntityNames>
    {
        private ILanguageCache languageCache;
        private ITypesCache typesCache;
        public GeneralDescriptionEntityNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmEntityNames TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            var model = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => i.LanguageAvailabilities, o => o.LanguagesAvailabilities).GetFinal();
            model.Names =
                entity.Names.Where(
                    x => x.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())
                    && entity.LanguageAvailabilities.Select(y => y.LanguageId).Contains(x.LocalizationId))
                    .ToDictionary(x => languageCache.GetByValue(x.LocalizationId), x => x.Name);
            return model;
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmEntityNames vModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
