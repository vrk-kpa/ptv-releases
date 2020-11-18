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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.TranslationOrder;

namespace PTV.Database.DataAccess.Translators.TranslationOrder
{
    [RegisterService(typeof(ITranslator<Model.Models.TranslationOrder, VmTranslationOrderOutput>), RegisterType.Transient)]
    internal class TranslationOrderOutputTranslator : Translator<Model.Models.TranslationOrder, VmTranslationOrderOutput>
    {
        public TranslationOrderOutputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmTranslationOrderOutput TranslateEntityToVm(Model.Models.TranslationOrder entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.OrderIdentifier, output => output.OrderIdentifier)
                .AddSimple(input => input.SourceLanguageId, output => output.SourceLanguage)
                .AddSimple(input => input.TargetLanguageId, output => output.TargetLanguage)
                .AddNavigation(input => languageCache.GetByValue(input.SourceLanguageId),
                    output => output.SourceLanguageCode)
                .AddNavigation(input => languageCache.GetByValue(input.TargetLanguageId),
                    output => output.TargetLanguageCode)
                .AddNavigation(input => input.SenderName, output => output.SenderName)
                .AddNavigation(input => input.SenderEmail, output => output.SenderEmail)
                .AddNavigation(input => input.TranslationCompany.Name, output => output.TranslationCompanyName)
                .AddNavigation(input => input.TranslationCompany.Email, output => output.TranslationCompanyEmail)
                //Detail information
                .AddNavigation(input => input.AdditionalInformation, output => output.AdditionalInformation)
                .AddNavigation(input => input.SourceEntityName, output => output.EntityName);

            if (entity.ServiceTranslationOrders.Any())
            {
                definition
                    .AddSimple(input => input.ServiceTranslationOrders.Select(x => x.ServiceVersionedIdentifier).FirstOrDefault(), output => output.EntityId)
                    .AddSimple(input => input.OrganizationIdentifier, output => output.EntityOrganizationId);
            }
            else if (entity.ServiceChannelTranslationOrders.Any())
            {
                definition
                    .AddSimple(input => input.ServiceChannelTranslationOrders.Select(x => x.ServiceChannelIdentifier).FirstOrDefault(), output => output.EntityId)
                    .AddSimple(input => input.OrganizationIdentifier, output => output.EntityOrganizationId);
            }
            else if (entity.GeneralDescriptionTranslationOrders.Any())
            {
                definition
                    .AddSimple(input => input.GeneralDescriptionTranslationOrders.First().StatutoryServiceGeneralDescriptionId, output => output.EntityId);
            }

            return definition.GetFinal();
        }

        public override Model.Models.TranslationOrder TranslateVmToEntity(VmTranslationOrderOutput vModel)
        {
            throw new NotImplementedException();
        }
    };
}
