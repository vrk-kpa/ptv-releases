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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.ServiceCollection;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Translators.ServiceCollections
{
    [RegisterService(typeof(ITranslator<ServiceCollectionVersioned, VmServiceCollectionBase>), RegisterType.Transient)]
    internal class ServiceCollectionBaseTranslator : Translator<ServiceCollectionVersioned, VmServiceCollectionBase>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly CommonTranslatorHelper commonTranslatorHelper;
        

        public ServiceCollectionBaseTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager, 
            CommonTranslatorHelper commonTranslatorHelper) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
            this.commonTranslatorHelper = commonTranslatorHelper;
        }

        public override VmServiceCollectionBase TranslateEntityToVm(ServiceCollectionVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.OrganizationId, output => output.Organization)
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.Description),
                    output => output.Description, k => languageCache.GetByValue(k.LocalizationId))
                .AddPartial(input => input, output => output as VmServiceCollectionHeader);

            return definition.GetFinal();
        }       

        private IEnumerable<IDescription> GetDescription(ServiceCollectionVersioned serviceCollectionVersioned, DescriptionTypeEnum type)
        {
            return serviceCollectionVersioned.ServiceCollectionDescriptions.Where(x => typesCache.Compare<DescriptionType>(x.TypeId, type.ToString()));
        }

        public override ServiceCollectionVersioned TranslateVmToEntity(VmServiceCollectionBase vModel)
        {
            var names = new List<VmName>();
            names.AddNullRange(CreateNames(vModel, vModel.Name, NameTypeEnum.Name));
            var descriptions = new List<VmDescription>();
            descriptions.AddNullRange(vModel.Description?.Select(pair => commonTranslatorHelper.CreateDescription(pair.Key, pair.Value, vModel.Id, DescriptionTypeEnum.Description)));
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                .UseVersioning<ServiceCollectionVersioned, ServiceCollection>(o => o)
                .DisableAutoTranslation()
                .AddLanguageAvailability(i => i, o => o)
                .AddSimple(i => i.Organization ?? throw new ArgumentNullException("Organization cannot be empty"),
                    o => o.OrganizationId)
                .AddCollectionWithRemove(i => names, o => o.ServiceCollectionNames, r => true)
                .AddCollection(i => descriptions, o => o.ServiceCollectionDescriptions, true);
               

            return definition.GetFinal();
        }

        private IEnumerable<VmName> CreateNames(VmServiceCollectionBase vModel, Dictionary<string, string> name, NameTypeEnum nameType)
        {
            return name?.Where(x => !string.IsNullOrEmpty(x.Value)).Select(pair => commonTranslatorHelper.CreateName(pair.Key, pair.Value, vModel, nameType));
        }
    }

    [RegisterService(typeof(ITranslator<ServiceCollectionVersioned, VmServiceCollectionOutput>), RegisterType.Transient)]
    internal class ServiceCollecrionReadTranslator : Translator<ServiceCollectionVersioned, VmServiceCollectionOutput>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        public ServiceCollecrionReadTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmServiceCollectionOutput TranslateEntityToVm(ServiceCollectionVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)

                .AddPartial(i => i, o => o as VmServiceCollectionBase);
                //.AddCollection(i => i.UnificRoot.ServiceCollectionServices, o => o.Connections);
            
            return definition.GetFinal();
        }

        public override ServiceCollectionVersioned TranslateVmToEntity(VmServiceCollectionOutput vModel)
        {
            throw new NotImplementedException();
        }
    }
}