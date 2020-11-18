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

using System.Collections.Generic;
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
using VmServiceChannel = PTV.Domain.Model.Models.V2.Channel.VmServiceChannel;
using PTV.Domain.Model.Models.V2.Channel;

namespace PTV.Database.DataAccess.Translators.Channels.V2
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmServiceChannel>), RegisterType.Transient)]
    internal class ServiceChannelCommonTranslator : Translator<ServiceChannelVersioned, VmServiceChannel>
    {
        private readonly ITypesCache typesCache;
        private readonly CommonTranslatorHelper commonTranslatorHelper;
        private readonly EntityDefinitionHelper definitionHelper;

        public ServiceChannelCommonTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager,
            CommonTranslatorHelper commonTranslatorHelper,
            EntityDefinitionHelper entityHelper
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            typesCache = cacheManager.TypesCache;
            this.commonTranslatorHelper = commonTranslatorHelper;
            definitionHelper = entityHelper;
        }

        public override VmServiceChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddPartial(i => i, o => o as VmChannelHeader)
                .AddSimple(i => i.Id, o => o.Id)
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.ShortDescription), output => output.ShortDescription, desc => languageCache.GetByValue(desc.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.Description), output => output.Description, desc => languageCache.GetByValue(desc.LocalizationId))
                .AddSimple(input => input.ConnectionTypeId, output => output.ConnectionTypeId)
                .AddSimple(input => input.UnificRootId, outupt => outupt.UnificRootId)
                .AddSimple(input => input.OrganizationId, output => output.OrganizationId)
                .AddNavigation(input => input, output => output.AreaInformation)
                //.AddDictionaryList(
                //    i => i.Phones.Select(x => x.Phone).OrderBy(x => x.OrderNumber),
                //    o => o.PhoneNumbers,
                //    x => languageCache.GetByValue(x.LocalizationId)
                //)
                //.AddDictionaryList(
                //    i => i.Emails.Select(x => x.Email).OrderBy(x => x.OrderNumber),
                //    o => o.Emails,
                //    x => languageCache.GetByValue(x.LocalizationId)
                //)
                .AddCollection(i => i.UnificRoot?.ServiceServiceChannels, o => o.ConnectedServicesUnific)
                .AddCollection(input => input.DisplayNameTypes.Where(x => x.DisplayNameTypeId == typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())).Select(x => languageCache.GetByValue(x.LocalizationId)), output => output.IsAlternateNameUsedAsDisplayName)
                .AddDictionary(input => GetName(input, NameTypeEnum.AlternateName), output => output.AlternateName, k => languageCache.GetByValue(k.LocalizationId));

            definitionHelper
                .AddOrderedDictionaryList(
                    definition,
                    i => i.Phones.Select(x => x.Phone),
                    o => o.PhoneNumbers,
                    x => languageCache.GetByValue(x.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.Emails.Select(x => x.Email),
                    o => o.Emails,
                    x => languageCache.GetByValue(x.LocalizationId)
                );

            return definition.GetFinal();
        }

        private IEnumerable<IName> GetName(ServiceChannelVersioned serviceChannelVersioned, NameTypeEnum type)
        {
            return serviceChannelVersioned.ServiceChannelNames.Where(x => typesCache.Compare<NameType>(x.TypeId, type.ToString()));
        }

        private IEnumerable<IDescription> GetDescription(ServiceChannelVersioned serviceChannelVersioned, DescriptionTypeEnum type)
        {
            return serviceChannelVersioned.ServiceChannelDescriptions.Where(x => typesCache.Compare<DescriptionType>(x.TypeId, type.ToString()));
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmServiceChannel vModel)
        {
            var descriptions = new List<VmDescription>();
            if (vModel.ShortDescription != null && vModel.ShortDescription.Any())
            {
                descriptions.AddRange(vModel.ShortDescription.Select(pair => commonTranslatorHelper.CreateDescription(pair.Key, pair.Value, vModel.Id, DescriptionTypeEnum.ShortDescription)));
            }

            if (vModel.Description != null && vModel.Description.Any())
            {
                descriptions.AddRange(vModel.Description?.Select(pair => commonTranslatorHelper.CreateDescription(pair.Key, pair.Value, vModel.Id, DescriptionTypeEnum.Description)));
            }


            var names = new List<VmName>();
            names.AddNullRange(vModel.Name?.Select(pair => commonTranslatorHelper.CreateName(pair.Key, pair.Value, vModel, NameTypeEnum.Name)));
            names.AddNullRange(vModel.AlternateName?.Where(x=>!string.IsNullOrEmpty(x.Value)).Select(pair => commonTranslatorHelper.CreateName(pair.Key, pair.Value, vModel, NameTypeEnum.AlternateName)));

            var displayNameTypes = commonTranslatorHelper.CreateDispalyNameTypes(vModel);

            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .AddSimple(i => i.OrganizationId, o => o.OrganizationId)
                .AddCollectionWithRemove(i => names, o => o.ServiceChannelNames, r => true)
                .AddCollectionWithRemove(i => displayNameTypes, o => o.DisplayNameTypes, TranslationPolicy.FetchData, x => true)
                .AddCollection(i => descriptions, o => o.ServiceChannelDescriptions, true)
                .AddPartial(i =>
                {
                    if (i.AreaInformation == null)
                    {
                        return new VmAreaInformation();
                    }
                    i.AreaInformation.OwnerReferenceId = i.Id;
                    return i.AreaInformation;
                })
                .AddSimple(i => i.ConnectionTypeId.IsAssigned()
                        // ReSharper disable once PossibleInvalidOperationException
                        ? i.ConnectionTypeId.Value
                        : typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString())
                    , o => o.ConnectionTypeId);
                //.AddCollectionWithRemove(
                //    input => input.PhoneNumbers?.SelectMany(pair =>
                //    {
                //        var localizationId = languageCache.Get(pair.Key);
                //        var phoneNumberOrder = 0;
                //        return pair.Value?.Select(at =>
                //        {
                //            if (at != null)
                //            {
                //                at.LanguageId = localizationId;
                //                at.OrderNumber = phoneNumberOrder++;
                //            }
                //            return at;
                //        }) ?? pair.Value;
                //    }),
                //    output => output.Phones,
                //    x => true
                //)
                //.AddCollectionWithRemove(
                //    input => input.Emails?.SelectMany(pair =>
                //    {
                //        var localizationId = languageCache.Get(pair.Key);
                //        var emailOrderNumber = 0;
                //        return pair.Value?.Select(x =>
                //        {
                //            if (x != null)
                //            {
                //                x.LanguageId = localizationId;
                //                x.OrderNumber = emailOrderNumber++;
                //            }
                //            return x;
                //        });
                //    }),
                //    output => output.Emails,
                //    x => true
                //);
                definitionHelper
                    .AddOrderedCollectionWithRemove(
                        definition,
                        i => i.PhoneNumbers,
                        o => o.Phones,
                        x => true,
                        (i, o, k) =>
                        {
                            o.LanguageId = languageCache.Get(k);
                        }
                    )
                    .AddOrderedCollectionWithRemove(
                        definition,
                        i => i.Emails,
                        o => o.Emails,
                        x => true,
                        (i, o, k) =>
                        {
                            o.LanguageId = languageCache.Get(k);
                        }
                    );
                return definition.GetFinal();
        }
    }
}
