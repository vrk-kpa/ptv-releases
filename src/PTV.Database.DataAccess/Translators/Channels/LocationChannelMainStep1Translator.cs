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
using Microsoft.AspNetCore.Razor.TagHelpers;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmLocationChannelStep1>), RegisterType.Transient)]
    internal class LocationChannelMainStep1Translator : Translator<ServiceChannelVersioned, VmLocationChannelStep1>
    {
        private ServiceChannelTranslationDefinitionHelper definitionHelper;
        private readonly ILanguageCache _languageCache;
        private readonly ITypesCache _typesCache;

        public LocationChannelMainStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ServiceChannelTranslationDefinitionHelper definitionHelper, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
            _languageCache = cacheManager.LanguageCache;
            _typesCache = cacheManager.TypesCache;
        }

        public override VmLocationChannelStep1 TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var phoneTypeId = _typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());
            var faxTypeId = _typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString());

            var definition = CreateEntityViewModelDefinition<VmLocationChannelStep1>(entity)
                    .AddSimple(i => i.Id, o => o.Id)
                    .AddSimple(input => input.UnificRootId, outupt => outupt.UnificRootId)
                    .AddSimple(input => input.AreaInformationTypeId, outupt => outupt.AreaInformationTypeId)
                    .AddSimple(input => input.PublishingStatusId, output => output.PublishingStatusId)
                    .AddSimple(input => input.ConnectionTypeId, output => output.ConnectionTypeId)
                    .AddPartial(input => input.ServiceLocationChannels?.FirstOrDefault())
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(input => input.LanguageAvailabilities, o => o.LanguagesAvailabilities);
            definitionHelper.AddEmailsDefinition(definition, RequestLanguageCode);
            definitionHelper.AddPhonesPhoneTypeDefinition(definition, RequestLanguageCode);
            definitionHelper.AddPhonesFaxTypeDefinition(definition, RequestLanguageCode);
            definitionHelper.AddLanguagesDefinition(definition);
            definitionHelper.AddWebPagesDefinition(definition, RequestLanguageCode);
            definitionHelper.AddAllAreasDefinition(definition);

            return definitionHelper.AddChannelDescriptionsDefinition(definition).GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmLocationChannelStep1 vModel)
        {
            var phoneTypeId = _typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());
            var faxTypeId = _typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString());

            var definition = CreateViewModelEntityDefinition<ServiceChannelVersioned>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                .AddLanguageAvailability(o => o)
                .AddSimple(i => i.AreaInformationTypeId, output => output.AreaInformationTypeId)
                .AddNavigationOneMany(i => i, o => o.ServiceLocationChannels, true);
            definitionHelper.AddLanguagesDefinition(definition);

            definition.AddSimple(i => i.ConnectionTypeId.IsAssigned()
                    // ReSharper disable once PossibleInvalidOperationException
                    ? i.ConnectionTypeId.Value
                    : _typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString())
                , o => o.ConnectionTypeId);

            var phonesAndFaxesList = new List<VmPhone>();

            if (vModel.FaxNumbers != null)
            {
                var number = 1;
                vModel.FaxNumbers.ForEach(p =>
                    {
                        if (p.TypeId == Guid.Empty) p.TypeId = faxTypeId;
                        p.OrderNumber = number++;
                    });
                phonesAndFaxesList.AddRange(vModel.FaxNumbers);
            }

            if (vModel.PhoneNumbers != null)
            {
                var number = 1;
                vModel.PhoneNumbers.ForEach(p =>
                    {
                        if (p.TypeId == Guid.Empty) p.TypeId = phoneTypeId;
                        p.OrderNumber = number++;
                    });
                phonesAndFaxesList.AddRange(vModel.PhoneNumbers);
            }

            definition.AddCollection(i => phonesAndFaxesList, o => o.Phones);

//            definitionHelper.AddPhonesDefinition(definition);
            definitionHelper.AddEmailsDefinition(definition);
            definitionHelper.AddWebPagesDefinition(definition, vModel, vModel.Id);
            definitionHelper.AddAllAreasDefinition(definition, vModel.AreaInformationTypeId, vModel.Id);

            return definitionHelper.AddChannelDescriptionsDefinition(definition).GetFinal();
        }
    }
}
