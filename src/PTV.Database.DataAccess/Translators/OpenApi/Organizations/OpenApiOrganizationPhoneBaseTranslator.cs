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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using System;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    internal abstract class OpenApiOrganizationPhoneBaseTranslator<TModel> : Translator<OrganizationPhone, TModel> where TModel : class, IVmOpenApiPhone
    {

        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        protected OpenApiOrganizationPhoneBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override TModel TranslateEntityToVm(OrganizationPhone entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }
        public override OrganizationPhone TranslateVmToEntity(TModel vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<OrganizationPhone, TModel> CreateBaseEntityVmDefinitions(OrganizationPhone entity)
        {

            if (entity?.Phone == null)
            {
                return null;
            }

            return CreateEntityViewModelDefinition<TModel>(entity)
                .AddPartial(i => i.Phone);
        }

        protected ITranslationDefinitions<TModel, OrganizationPhone> CreateBaseVmEntityDefinitions(TModel vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            var exists = vModel.OwnerReferenceId.IsAssigned();

            /*
            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextLocalizedUpdate(input => exists, input => output => (input.Id == output.PhoneId) &&
                (!input.OwnerReferenceId.IsAssigned() || output.OrganizationId == vModel.OwnerReferenceId));
            */

            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextUpdate(input => exists, input => output => input.OwnerReferenceId == output.OrganizationId &&
                    input.PrefixNumber == output.Phone.PrefixNumber && input.Number == output.Phone.Number, w => w.UseDataContextCreate(x => true));

            var entity = definition.GetFinal();
            if (entity.Created != DateTime.MinValue)
            {
                vModel.Id = entity.PhoneId;
            }

            definition.AddNavigation(input => new VmPhone
            {
                Id = input.Id,
                OwnerReferenceId = input.OwnerReferenceId,
                LanguageId = string.IsNullOrEmpty(input.Language) ? Guid.Empty : languageCache.Get(input.Language),
                Number = input.Number,
                PrefixNumber = input.PrefixNumber,
                ChargeTypeId = string.IsNullOrEmpty(input.ServiceChargeType) ? Guid.Empty : typesCache.Get<ServiceChargeType>(input.ServiceChargeType),
                AdditionalInformation = input.AdditionalInformation,
                ChargeDescription = input.ChargeDescription
            }, output => output.Phone);

            return definition;
        }
    }
}
