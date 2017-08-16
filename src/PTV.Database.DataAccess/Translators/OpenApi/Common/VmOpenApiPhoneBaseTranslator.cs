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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    internal class VmOpenApiPhoneBaseTranslator<TModel> : Translator<Phone, TModel> where TModel : class, IV4VmOpenApiPhone
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public VmOpenApiPhoneBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override TModel TranslateEntityToVm(Phone entity)
        {
            return CreateEntityVmBaseDefinitions(entity).GetFinal();
        }

        public override Phone TranslateVmToEntity(TModel vModel)
        {
             return CreateVmEntityBaseDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<Phone, TModel> CreateEntityVmBaseDefinitions(Phone entity)
        {
            return CreateEntityViewModelDefinition(entity)
              .AddNavigation(i => i.Number, o => o.Number)
              .AddNavigation(i => i.PrefixNumber?.Code, o => o.PrefixNumber)
              .AddNavigation(i => i.AdditionalInformation, o => o.AdditionalInformation)
              .AddNavigation(i => i.ChargeDescription, o => o.ChargeDescription)
              .AddNavigation(i => typesCache.GetByValue<ServiceChargeType>(i.ServiceChargeTypeId), o => o.ServiceChargeType)
              .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
              .AddSimple(i => string.IsNullOrEmpty(i.PrefixNumber?.Code) ? true : false, o => o.IsFinnishServiceNumber)
              .AddSimple(i => i.OrderNumber, o => o.OrderNumber);
        }

        protected ITranslationDefinitions<TModel, Phone> CreateVmEntityBaseDefinitions(TModel vModel)
        {
            var exists = vModel.Id.IsAssigned();

            var serviceChargeType = (string.IsNullOrEmpty(vModel.ServiceChargeType))
                ? typesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString())
                : typesCache.Get<ServiceChargeType>(vModel.ServiceChargeType);

            var definitions = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id, e => e.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Number, o => o.Number)
                .AddNavigation(i => i.AdditionalInformation, o => o.AdditionalInformation)
                .AddNavigation(i => i.ChargeDescription, o => o.ChargeDescription)
                .AddSimple(i => serviceChargeType, o => o.ServiceChargeTypeId)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber);

            // IsFinnishServiceNumber is the determinant. See Jira bug https://jira.csc.fi/browse/PTV-1476
            if (!vModel.IsFinnishServiceNumber)
            {
                definitions.AddNavigation(i => i.PrefixNumber, o => o.PrefixNumber);
            }

            return definitions;
        }
    }
}
