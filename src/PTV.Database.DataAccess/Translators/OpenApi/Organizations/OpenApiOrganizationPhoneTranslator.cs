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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V4;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationPhone, V4VmOpenApiPhone>), RegisterType.Transient)]
    internal class OpenApiOrganizationPhoneTranslator : Translator<OrganizationPhone, V4VmOpenApiPhone>
    {
        public OpenApiOrganizationPhoneTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override V4VmOpenApiPhone TranslateEntityToVm(OrganizationPhone entity)
        {
            if (entity?.Phone == null)
            {
                return null;
            }

            return CreateEntityViewModelDefinition(entity)
                .AddPartial(i => i.Phone)
                .GetFinal();
        }

        public override OrganizationPhone TranslateVmToEntity(V4VmOpenApiPhone vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            var exists = vModel.OwnerReferenceId.IsAssigned();

            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextLocalizedUpdate(input => exists,
                    input => output => 
                        input.OwnerReferenceId.Value == output.OrganizationVersionedId 
                        && languageCache.Get(input.Language) == output.Phone.LocalizationId 
                        && input.Number == output.Phone.Number 
                        && input.PrefixNumber == output.Phone.PrefixNumber.Code
                        && input.AdditionalInformation == output.Phone.AdditionalInformation,
                    e => e.UseDataContextCreate(x=> true));

            if (exists)
            {
                var organizationPhone = definition.GetFinal();
                if (organizationPhone.Created > DateTime.MinValue)
                {
                    vModel.Id = organizationPhone.PhoneId;
                }
            }

            return  definition
                .AddNavigation(input => vModel, output => output.Phone)
                .GetFinal();
        }
    }
}
