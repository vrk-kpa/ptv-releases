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
using PTV.Domain.Model.Models.V2.Organization;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations.V2
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmOrganizationBase>), RegisterType.Transient)]
    internal class OrganizationBaseTranslator : Translator<OrganizationVersioned, VmOrganizationBase>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public OrganizationBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOrganizationBase TranslateEntityToVm(OrganizationVersioned entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.ParentId, output => output.ParentId)
                .AddSimple(input => !input.ParentId.IsAssigned(), output => output.IsMainOrganization)
                .AddSimple(input => input.TypeId, output => output.OrganizationType)
                .AddCollection(input => input.OrganizationDisplayNameTypes.Where(x => x.DisplayNameTypeId == typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())).Select(x => languageCache.GetByValue(x.LocalizationId)), output => output.IsAlternateNameUsedAsDisplayName)
                .AddNavigation(input => input, output => output.AreaInformation)
                .AddNavigation(input => input.Oid, output => output.OrganizationId)
                .AddNavigation(input => input.Business, output => output.Business)
                .AddDictionary(input => GetName(input, NameTypeEnum.AlternateName), output => output.AlternateName, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.Description), output => output.Description, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.ShortDescription), output => output.ShortDescription, k => languageCache.GetByValue(k.LocalizationId))
               //IN/OUT .AddNavigation(input => input.Municipality, output => output.Municipality)
                .AddDictionaryList(input => input.OrganizationEmails.Select(x => x.Email).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.Emails, k=> languageCache.GetByValue(k.LocalizationId))
                .AddDictionaryList(input => input.OrganizationPhones.Select(x => x.Phone).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.PhoneNumbers, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionaryList(input => input.OrganizationWebAddress.Select(x => x.WebPage).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.WebPages, k => languageCache.GetByValue(k.LocalizationId))
                .AddCollection(input => input.OrganizationAddresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Postal.ToString())).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.PostalAddresses)
                .AddCollection(input => input.OrganizationAddresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString())).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.VisitingAddresses)
                //.AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString())).Select(x => x.AreaId), output => output.BusinessRegions)
                //.AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString())).Select(x => x.AreaId), output => output.HospitalRegions)
                //.AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString())).Select(x => x.AreaId), output => output.Provinces)
                //.AddSimpleList(input => input.OrganizationAreaMunicipalities.Select(x => x.MunicipalityId), output => output.Municipalities)
                .AddPartial(input => input, output => output as VmOrganizationHeader)
                .GetFinal();
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
            names.AddNullRange(vModel.Name?.Select(pair => CreateName(pair.Key, pair.Value, vModel, NameTypeEnum.Name)));
            names.AddNullRange(vModel.AlternateName?.Where(x=>!string.IsNullOrEmpty(x.Value)).Select(pair => CreateName(pair.Key, pair.Value, vModel, NameTypeEnum.AlternateName)));

            var descriptions = new List<VmDescription>();
            descriptions.AddNullRange(vModel.Description?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.Description)));
            descriptions.AddNullRange(vModel.ShortDescription?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ShortDescription)));

            vModel.VisitingAddresses.ForEach(a => a.AddressCharacter = AddressCharacterEnum.Visiting);
            vModel.PostalAddresses.ForEach(a => a.AddressCharacter = AddressCharacterEnum.Postal);

            vModel.AreaInformation.AreaInformationTypeId = vModel.OrganizationType.IsAssigned() && typesCache.GetByValue<OrganizationType>(vModel.OrganizationType.Value) == OrganizationTypeEnum.RegionalOrganization.ToString()
                ? typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString()) 
                : vModel.AreaInformation.AreaInformationTypeId;

            var displayNameTypes = CreateDispalyNameTypes(vModel);

            var transaltionDefinition = CreateViewModelEntityDefinition<OrganizationVersioned>(vModel)
            .UseDataContextCreate(input => !input.Id.HasValue, output => output.Id, input => Guid.NewGuid())
            .UseDataContextLocalizedUpdate(input => input.Id.HasValue, input => output => input.Id.Value == output.Id)            
            .AddLanguageAvailability(i => i, o => o)
            .AddCollectionWithRemove(i => names, o => o.OrganizationNames, r => true)
            .AddCollection(i => descriptions, o => o.OrganizationDescriptions, true)
            .AddNavigation(i => i.Business, o => o.Business)
            .AddSimple(i => i.OrganizationType, o => o.TypeId)
            .AddNavigation(i => i.OrganizationId, o => o.Oid)
            .AddSimple(i => i.IsMainOrganization ? null : i.ParentId, o => o.ParentId)
            .AddCollectionWithRemove(i => displayNameTypes, o => o.OrganizationDisplayNameTypes, TranslationPolicy.FetchData, x=>true)
            .AddCollection(i => i.Emails.SelectMany(pair =>
            {
                var localizationId = languageCache.Get(pair.Key);
                var orderNumber = 0;
                return pair.Value.Select(sv =>
                {
                    sv.OwnerReferenceId = i.Id;
                    sv.LanguageId = localizationId;
                    sv.OrderNumber = orderNumber++;
                    return sv;
                });
            }), o => o.OrganizationEmails, false)
            .AddCollection(i => i.PhoneNumbers.SelectMany(pair =>
            {
                var localizationId = languageCache.Get(pair.Key);
                var orderNumber = 0;
                return pair.Value.Select(sv =>
                {
                    sv.OwnerReferenceId = i.Id;
                    sv.LanguageId = localizationId;
                    sv.OrderNumber = orderNumber++;
                    return sv;
                });
            }), o => o.OrganizationPhones, false)
            .AddCollection(i => i.WebPages.SelectMany(pair =>
            {
                var localizationId = languageCache.Get(pair.Key);
                var orderNumber = 0;
                return pair.Value.Select(sv =>
                {
                    sv.OwnerReferenceId = i.Id;
                    sv.LocalizationId = localizationId;
                    sv.OrderNumber = orderNumber++;
                    return sv;
                });
            }), o => o.OrganizationWebAddress, false)
            .AddCollection(i =>
            {
                var addresses = i.PostalAddresses.Concat(i.VisitingAddresses);
                var orderNumber = 0;
                addresses.ForEach(a =>
                {
                    a.OrderNumber = orderNumber++;
                    a.OwnerReferenceId = i.Id;
                });
                return addresses;
            }, o => o.OrganizationAddresses, false)            
            .AddPartial(i =>
             {
                 i.AreaInformation.AreaInformationTypeId =
                  !i.AreaInformation.AreaInformationTypeId.IsAssigned()
                  ? typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString())
                  : i.AreaInformation.AreaInformationTypeId;
                i.AreaInformation.OwnerReferenceId = i.Id;
                 i.AreaInformation.OrganizationType = i.OrganizationType;
                return i.AreaInformation;
             })
            .GetFinal();

            return transaltionDefinition;
        }

        private IEnumerable<VmDispalyNameType> CreateDispalyNameTypes(VmOrganizationBase model)
        {
            var languages = model.Name.Keys;
            return languages.Select(lang =>
            {
                var alterName = model.AlternateName.TryGet(lang);
                return new VmDispalyNameType()
                {
                    OwnerReferenceId = model.Id,
                    LocalizationId = languageCache.Get(lang),
                    NameTypeId = model.IsAlternateNameUsedAsDisplayName != null &&
                                 model.IsAlternateNameUsedAsDisplayName.Contains(lang.ToString()) &&
                                 !string.IsNullOrEmpty(alterName) ?
                            typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString()) :
                            typesCache.Get<NameType>(NameTypeEnum.Name.ToString())
                };
            });
        }
        private VmName CreateName(string language, string value, VmOrganizationBase vModel, NameTypeEnum typeEnum)
        {
            return new VmName
            {
                Name = value,
                TypeId = typesCache.Get<NameType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language)
            };
        }

        private VmDescription CreateDescription(string language, string value, VmOrganizationBase vModel, DescriptionTypeEnum typeEnum)
        {
            return new VmDescription
            {
                Description = value,
                TypeId = typesCache.Get<DescriptionType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language)
            };
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
    internal class ServiceSaveTranslator : Translator<OrganizationVersioned, VmOrganizationInput>
    {
        private ITypesCache typesCache;
        public ServiceSaveTranslator(IResolveManager resolveManager,
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