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
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using PTV.Framework.Interfaces;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.Channels.V2.ServiceLocation
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmServiceLocationChannel>), RegisterType.Transient)]
    internal class ServiceLocationChannelMainTranslator : Translator<ServiceChannelVersioned, VmServiceLocationChannel>
    {
        private ServiceChannelTranslationDefinitionHelper definitionHelper;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public ServiceLocationChannelMainTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ServiceChannelTranslationDefinitionHelper definitionHelper
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            this.definitionHelper = definitionHelper;
            typesCache = definitionHelper.CacheManager.TypesCache;
            languageCache = definitionHelper.CacheManager.LanguageCache;
        }

        public override VmServiceLocationChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity);

            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition)
                .AddOpeningHoursDefinition(definition);
            //definition
            //    .AddDictionaryList(i =>
            //        i.Phones.Select(x => x.Phone).Where(x => typesCache.Compare<PhoneNumberType>(x.TypeId, PhoneNumberTypeEnum.Phone.ToString())).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created),
            //        o => o.PhoneNumbers,
            //        x => languageCache.GetByValue(x.LocalizationId))
            //    .AddDictionaryList(
            //        i => i.Phones.Select(x => x.Phone).Where(x => typesCache.Compare<PhoneNumberType>(x.TypeId, PhoneNumberTypeEnum.Fax.ToString())).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created),
            //        o => o.FaxNumbers,
            //        x => languageCache.GetByValue(x.LocalizationId));
            //definition.AddDictionaryList(i => i.WebPages.OrderBy(x => x.WebPage.OrderNumber).ThenBy(x => x.WebPage.Created), o => o.WebPages, key => languageCache.GetByValue(key.WebPage.LocalizationId));
            //definition.AddCollection(input => input.Addresses.Where(x => x.Character.Code == AddressCharacterEnum.Visiting.ToString()).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.VisitingAddresses);
            //definition.AddCollection(input => input.Addresses.Where(x => x.Character.Code == AddressCharacterEnum.Postal.ToString()).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.PostalAddresses);

            definitionHelper
                .AddOrderedDictionaryList(
                    definition,
                    i => i.Phones.Select(x => x.Phone).Where(x => typesCache.Compare<PhoneNumberType>(x.TypeId, PhoneNumberTypeEnum.Phone.ToString())),
                    o => o.PhoneNumbers,
                    x => languageCache.GetByValue(x.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.Phones.Select(x => x.Phone).Where(x => typesCache.Compare<PhoneNumberType>(x.TypeId, PhoneNumberTypeEnum.Fax.ToString())),
                    o => o.FaxNumbers,
                    x => languageCache.GetByValue(x.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.WebPages,
                    o => o.WebPages,
                    k => languageCache.GetByValue(k.WebPage.LocalizationId)
                )
                .AddOrderedCollection(
                    definition,
                    i => i.Addresses.Where(x => x.Character.Code == AddressCharacterEnum.Visiting.ToString()).Select(x => x.Address),
                    o => o.VisitingAddresses
                )
                .AddOrderedCollection(
                    definition,
                    i => i.Addresses.Where(x => x.Character.Code == AddressCharacterEnum.Postal.ToString()).Select(x => x.Address),
                    o => o.PostalAddresses
                );

            return definition.GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmServiceLocationChannel vModel)
        {
            MergeFaxNumbers(vModel);

            var definition = CreateViewModelEntityDefinition(vModel)
                    .DisableAutoTranslation()
                    .DefineEntitySubTree(i => i.Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours))
                    .DefineEntitySubTree(i => i.Include(j => j.Addresses).ThenInclude(j => j.Address))
                    .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                    .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                    .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                    .AddLanguageAvailability(i => i, o => o)
                    .AddSimple(i => typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString()), o => o.TypeId);

            definition.Propagation((vm, sl) =>
            {
                var order = 1;
                vModel.VisitingAddresses?.ForEach(item =>
                {
                    item.AddressCharacter = AddressCharacterEnum.Visiting;
                    item.OwnerReferenceId = sl.Id;
                    item.OrderNumber = order++;
                });
                order = 1;
                vModel.PostalAddresses?.ForEach(item =>
                {
                    item.AddressCharacter = AddressCharacterEnum.Postal;
                    item.OwnerReferenceId = sl.Id;
                    item.OrderNumber = order++;
                });
            });
            definition.AddCollectionWithRemove(i => i.VisitingAddresses?.Concat(i.PostalAddresses) ?? i.PostalAddresses, o => o.Addresses, x => true);

            definitionHelper
                .AddOrderedCollectionWithRemove(
                    definition,
                    i => i.WebPages,
                    o => o.WebPages,
                    x => true,
                    (i, o, k) =>
                    {
                        o.LocalizationId = languageCache.Get(k);
                        o.OwnerReferenceId = i.Id;
                    }
                );
            
            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition)
                .AddOpeningHoursDefinition(definition);
            return definition.GetFinal();
        }

        private void MergeFaxNumbers(VmServiceLocationChannel vModel)
        {
            if (vModel.FaxNumbers.Any())
            {
                var faxTypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString());

                foreach (var pair in vModel.FaxNumbers)
                {
                    pair.Value?.ForEach(x => x.TypeId = faxTypeId);
                }

                if (vModel.PhoneNumbers != null)
                {
                    vModel.PhoneNumbers = vModel.PhoneNumbers
                        .Concat(vModel.FaxNumbers)
                        .GroupBy(kv => kv.Key)
                        .ToDictionary(kv => kv.Key, kv => kv.SelectMany(g => g.Value).ToList());
                }
            }
        }
    }
}