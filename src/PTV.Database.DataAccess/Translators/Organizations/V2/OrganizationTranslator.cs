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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.V2.Organization;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Translators.Channels.V2;

namespace PTV.Database.DataAccess.Translators.Organizations.V2
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmOrganizationBase>), RegisterType.Transient)]
    internal class OrganizationBaseTranslator : Translator<OrganizationVersioned, VmOrganizationBase>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly CommonTranslatorHelper commonTranslatorHelper;
        private readonly EntityDefinitionHelper entityDefinitionHelper;

        public OrganizationBaseTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager,
            CommonTranslatorHelper commonTranslatorHelper,
            EntityDefinitionHelper entityDefinitionHelper
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
            this.commonTranslatorHelper = commonTranslatorHelper;
            this.entityDefinitionHelper = entityDefinitionHelper;
        }

        public override VmOrganizationBase TranslateEntityToVm(OrganizationVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.ParentId, output => output.ParentId)
                .AddSimple(input => !input.ParentId.IsAssigned(), output => output.IsMainOrganization)
                .AddSimple(input => input.TypeId, output => output.OrganizationType)
                .AddCollection(input => input.OrganizationDisplayNameTypes.Where(x => x.DisplayNameTypeId == typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())).Select(x => languageCache.GetByValue(x.LocalizationId)), output => output.IsAlternateNameUsedAsDisplayName)
                .AddNavigation(input => input, output => output.AreaInformation)
                .AddNavigation(input => input.Oid, output => output.Oid)
                .AddNavigation(input => input.Business, output => output.Business)
                .AddDictionary(input => GetName(input, NameTypeEnum.AlternateName), output => output.AlternateName, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.Description), output => output.Description, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.ShortDescription), output => output.ShortDescription, k => languageCache.GetByValue(k.LocalizationId))
                //IN/OUT .AddNavigation(input => input.Municipality, output => output.Municipality)
                //.AddDictionaryList(input => input.OrganizationEmails.Select(x => x.Email).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.Emails, k => languageCache.GetByValue(k.LocalizationId))
                //.AddDictionaryList(input => input.OrganizationPhones.Select(x => x.Phone).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.PhoneNumbers, k => languageCache.GetByValue(k.LocalizationId))
                //.AddDictionaryList(input => input.OrganizationWebAddress.Select(x => x.WebPage).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.WebPages, k => languageCache.GetByValue(k.LocalizationId))
                //.AddCollection(input => input.OrganizationAddresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Postal.ToString())).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.PostalAddresses)
                //.AddCollection(input => input.OrganizationAddresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString())).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.VisitingAddresses)
                //.AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString())).Select(x => x.AreaId), output => output.BusinessRegions)
                //.AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString())).Select(x => x.AreaId), output => output.HospitalRegions)
                //.AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString())).Select(x => x.AreaId), output => output.Provinces)
                //.AddSimpleList(input => input.OrganizationAreaMunicipalities.Select(x => x.MunicipalityId), output => output.Municipalities)
                //.AddCollection(input => input.OrganizationEInvoicings.OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.ElectronicInvoicingAddresses)
                .AddPartial(input => input, output => output as VmOrganizationHeader)
                .AddSimple(i => i.ResponsibleOrganizationRegionId, o => o.ResponsibleOrganizationRegionId);

            entityDefinitionHelper
                .AddOrderedDictionaryList(
                    definition,
                    i => i.OrganizationEmails.Select(x => x.Email),
                    o => o.Emails,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.OrganizationPhones.Select(x => x.Phone),
                    o => o.PhoneNumbers,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.OrganizationWebAddress.Select(x => x.WebPage),
                    o => o.WebPages,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddOrderedCollection(
                    definition,
                    i => i.OrganizationAddresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Postal.ToString())).Select(x => x.Address),
                    o => o.PostalAddresses
                )
                .AddOrderedCollection(
                    definition,
                    i => i.OrganizationAddresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString())).Select(x => x.Address),
                    o => o.VisitingAddresses
                )
                .AddOrderedCollection(
                    definition,
                    i => i.OrganizationEInvoicings,
                    o => o.ElectronicInvoicingAddresses
                );


            return definition.GetFinal();
        }

        private IEnumerable<IName> GetName(OrganizationVersioned organizationVersioned, NameTypeEnum type)
        {
            var names = organizationVersioned.OrganizationNames.Where(x => x.TypeId == typesCache.Get<NameType>(type.ToString())).ToList();
            organizationVersioned.LanguageAvailabilities.ForEach(lang =>
            {
                if (!names.Where(x => x.LocalizationId == lang.LanguageId).Any())
                {
                    names.Add(new OrganizationName { LocalizationId = lang.LanguageId });
                }
            });
            return names;

        }

        private IEnumerable<IDescription> GetDescription(OrganizationVersioned organizationVersioned, DescriptionTypeEnum type)
        {
            return organizationVersioned.OrganizationDescriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(type.ToString()));

        }

        public override OrganizationVersioned TranslateVmToEntity(VmOrganizationBase vModel)
        {
            var names = new List<VmName>();
            names.AddNullRange(vModel.Name?.Select(pair => commonTranslatorHelper.CreateName(pair.Key, pair.Value, vModel, NameTypeEnum.Name)));
            names.AddNullRange(vModel.AlternateName?.Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(pair => commonTranslatorHelper.CreateName(pair.Key, pair.Value, vModel, NameTypeEnum.AlternateName)));

            var descriptions = new List<VmDescription>();
            descriptions.AddNullRange(
                vModel.Description?.Select(pair => commonTranslatorHelper.CreateDescription(pair.Key, pair.Value, vModel.Id, DescriptionTypeEnum.Description)));
            descriptions.AddNullRange(vModel.ShortDescription?.Select(pair =>
                commonTranslatorHelper.CreateDescription(pair.Key, pair.Value, vModel.Id, DescriptionTypeEnum.ShortDescription)));

            vModel.VisitingAddresses.ForEach(a => a.AddressCharacter = AddressCharacterEnum.Visiting);
            vModel.PostalAddresses.ForEach(a => a.AddressCharacter = AddressCharacterEnum.Postal);
            var orgType = vModel.OrganizationType.IsAssigned() ? typesCache.GetByValue<OrganizationType>(vModel.OrganizationType.Value) : string.Empty;
            vModel.AreaInformation.AreaInformationTypeId = vModel.OrganizationType.IsAssigned()
                                                           && (orgType == OrganizationTypeEnum.RegionalOrganization.ToString() ||
                                                               orgType == OrganizationTypeEnum.Region.ToString())
                ? typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString())
                : vModel.AreaInformation.AreaInformationTypeId;

            var displayNameTypes = commonTranslatorHelper.CreateDispalyNameTypes(vModel);

            var definition = CreateViewModelEntityDefinition<OrganizationVersioned>(vModel)
                .UseDataContextCreate(input => !input.Id.HasValue, output => output.Id, input => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(input => input.Id.HasValue, input => output => input.Id.Value == output.Id)
                .AddLanguageAvailability(i => i, o => o)
                .AddCollectionWithRemove(i => names, o => o.OrganizationNames, r => true)
                .AddCollection(i => descriptions, o => o.OrganizationDescriptions, true)
                .AddNavigation(i => i.Business, o => o.Business)
                .AddSimple(i => i.OrganizationType, o => o.TypeId)
                .AddNavigation(i => i.Oid, o => o.Oid)
                .AddSimple(i => i.IsMainOrganization ? null : i.ParentId, o => o.ParentId)
                .AddCollectionWithRemove(i => displayNameTypes, o => o.OrganizationDisplayNameTypes, TranslationPolicy.FetchData, x => true)
                .AddPartial(i =>
                {
                    i.AreaInformation.AreaInformationTypeId =
                        !i.AreaInformation.AreaInformationTypeId.IsAssigned()
                            ? typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString())
                            : i.AreaInformation.AreaInformationTypeId;
                    i.AreaInformation.OwnerReferenceId = i.Id;
                    i.AreaInformation.OrganizationType = i.OrganizationType;
                    return i.AreaInformation;
                });

            entityDefinitionHelper
                .AddOrderedCollection(
                    definition,
                    i => i.Emails,
                    o => o.OrganizationEmails,
                    false,
                    (i, o, k) =>
                    {
                        o.LanguageId = languageCache.Get(k);
                        o.OwnerReferenceId = i.Id;
                    }
                )
                .AddOrderedCollection(
                    definition,
                    i => i.PhoneNumbers,
                    o => o.OrganizationPhones,
                    false,
                    (i, o, k) =>
                    {
                        o.LanguageId = languageCache.Get(k);
                        o.OwnerReferenceId = i.Id;
                    }
                )
                .AddOrderedCollection(
                    definition,
                    i => i.WebPages,
                    o => o.OrganizationWebAddress,
                    false,
                    (i, o, k) =>
                    {
                        o.LocalizationId = languageCache.Get(k);
                        o.OwnerReferenceId = i.Id;
                    }
                )
                .AddOrderedCollection(
                    definition,
                    i => i.PostalAddresses.Concat(i.VisitingAddresses),
                    o => o.OrganizationAddresses,
                    false,
                    (i, o) => o.OwnerReferenceId = i.Id
                )
                .AddOrderedCollection(
                    definition,
                    i => i.ElectronicInvoicingAddresses,
                    o => o.OrganizationEInvoicings,
                    false,
                    (i, o) => o.OwnerReferenceId = i.Id
                );
            if (orgType == OrganizationTypeEnum.SotePublic.ToString() || orgType == OrganizationTypeEnum.SotePrivate.ToString())
            {
                definition.AddSimple(i => i.ResponsibleOrganizationRegionId, o => o.ResponsibleOrganizationRegionId);
            }
            return definition.GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmOrganizationOutput>), RegisterType.Transient)]
    internal class OrganizationReadTranslator : Translator<OrganizationVersioned, VmOrganizationOutput>
    {
        public OrganizationReadTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOrganizationOutput TranslateEntityToVm(OrganizationVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(i => i, o => o as VmOrganizationBase)
                .AddNavigation(input => input.Municipality, output => output.Municipality);

            return definition.GetFinal();
        }

        public override OrganizationVersioned TranslateVmToEntity(VmOrganizationOutput vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmOrganizationInput>), RegisterType.Transient)]
    internal class OrganizationSaveTranslator : Translator<OrganizationVersioned, VmOrganizationInput>
    {
        private ITypesCache typesCache;
        public OrganizationSaveTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmOrganizationInput TranslateEntityToVm(OrganizationVersioned entity)
        {
            throw new NotImplementedException();
        }

        public override OrganizationVersioned TranslateVmToEntity(VmOrganizationInput vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                .UseVersioning<OrganizationVersioned, Organization>(o => o)
                .AddPartial(i => i as VmOrganizationBase)
                .AddSimple(i => typesCache.Compare<OrganizationType>(i.OrganizationType, OrganizationTypeEnum.Municipality.ToString()) ? i.Municipality : null, o => o.MunicipalityId);           
            return definition.GetFinal();
        }
    }
};