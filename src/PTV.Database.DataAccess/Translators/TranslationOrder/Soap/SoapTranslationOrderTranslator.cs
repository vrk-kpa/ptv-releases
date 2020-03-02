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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.TranslationOrder.Soap;

namespace PTV.Database.DataAccess.Translators.TranslationOrder
{
    [RegisterService(typeof(ITranslator<Model.Models.TranslationOrder, VmSoapTranslationOrder>), RegisterType.Transient)]
    internal class SoapTranslationOrderTranslator : Translator<Model.Models.TranslationOrder, VmSoapTranslationOrder>
    {
        private ITypesCache typesCache;
        private ILanguageStateCultureCache languageStateCultureCache;

        public SoapTranslationOrderTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageStateCultureCache = cacheManager.LanguageStateCultureCache;
        }

        public override VmSoapTranslationOrder TranslateEntityToVm(Model.Models.TranslationOrder entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.OrderIdentifier, output => output.OrderIdentifier)
                .AddSimple(input => input.SourceLanguageId, output => output.SourceLanguage)
                .AddSimple(input => input.TargetLanguageId, output => output.TargetLanguage)
                .AddSimple(input => input.PreviousTranslationOrderId, output => output.PreviousTranslationOrderId)
                .AddNavigation(input => languageStateCultureCache.Get(input.SourceLanguageId), output => output.SourceLanguageStateCultureCode)
                .AddNavigation(input => languageStateCultureCache.Get(input.TargetLanguageId), output => output.TargetLanguageStateCultureCode)
                .AddNavigation(input => input.SenderName, output => output.SenderName)
                .AddNavigation(input => input.SenderEmail, output => output.SenderEmail)
                .AddNavigation(input => input.AdditionalInformation, output => output.AdditionalInformation)
                .AddNavigation(input => input.TranslationCompanyOrderIdentifier, output => output.TranslationCompanyOrderIdentifier)
                .AddNavigation(input => input.SourceEntityName, output => output.SourceEntityName)
                .AddNavigation(input => input.OrganizationName, output => output.SourceOrganizationName)
                .AddSimple(input => ParseEnum<TranslationStateTypeEnum>(typesCache.GetByValue<TranslationStateType>(input.TranslationOrderStates.OrderByDescending(x => x.SendAt).First().TranslationStateId)), output => output.OrderStateType);

            return definition.GetFinal();
        }

        private T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public override Model.Models.TranslationOrder TranslateVmToEntity(VmSoapTranslationOrder vModel)
        {
            throw new NotImplementedException();
        }
    };
}
