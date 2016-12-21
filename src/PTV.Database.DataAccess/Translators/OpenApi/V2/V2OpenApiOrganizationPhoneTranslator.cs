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
**/

using System;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    [RegisterService(typeof(ITranslator<VmOpenApiPhone, VmOpenApiOrganizationPhone>), RegisterType.Transient)]
    internal class V2OpenApiOrganizationPhoneTranslator : Translator<VmOpenApiPhone, VmOpenApiOrganizationPhone>
    {

        public V2OpenApiOrganizationPhoneTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        { }

        public override VmOpenApiOrganizationPhone TranslateEntityToVm(VmOpenApiPhone entity)
        {
            var definitions = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.OwnerReferenceId, o => o.OwnerReferenceId)
                .AddNavigation(i => i.Number, o => o.Number)
                .AddNavigation(i => i.PrefixNumber, o => o.PrefixNumber)
                .AddNavigation(i => i.ServiceChargeType, o => o.ChargeType);

            var descriptions = new List<VmOpenApiLocalizedListItem>();
            if (!string.IsNullOrEmpty(entity.AdditionalInformation))
            {
                descriptions.Add(new VmOpenApiLocalizedListItem
                {
                    Language = entity.Language,
                    Type = PhoneDescriptionTypeEnum.AdditionalInformation.ToString(),
                    Value = entity.AdditionalInformation
                });
            }

            if (!string.IsNullOrEmpty(entity.ChargeDescription))
            {
                descriptions.Add(new VmOpenApiLocalizedListItem
                {
                    Language = entity.Language,
                    Type = PhoneDescriptionTypeEnum.ChargeDescription.ToString(),
                    Value = entity.ChargeDescription
                });
            }

            if (descriptions.Count > 0)
            {
                definitions.AddCollection(i => descriptions, o => o.Descriptions);
            }

            return definitions.GetFinal();

        }

        public override VmOpenApiPhone TranslateVmToEntity(VmOpenApiOrganizationPhone vModel)
        {
            throw new NotImplementedException("Translator VmOpenApiOrganizationPhone -> VmOpenApiPhone is not implemented");
        }
    }
}
