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
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using VmServiceBase = PTV.Domain.Model.Models.V2.Service.VmServiceBase;
using PTV.Database.DataAccess.Translators.Channels.V2;

namespace PTV.Database.DataAccess.Translators.Services.V2
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, VmServiceBase>), RegisterType.Transient)]
    internal class ServiceBaseTranslator : Translator<ServiceVersioned, VmServiceBase>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly EntityDefinitionHelper entityDefinitionHelper;

        public ServiceBaseTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager,
            EntityDefinitionHelper entityDefinitionHelper
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
            this.entityDefinitionHelper = entityDefinitionHelper;
        }

        public override VmServiceBase TranslateEntityToVm(ServiceVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.StatutoryServiceGeneralDescriptionId, output => output.GeneralDescriptionId)
                .AddSimple(input => input.TypeId, output => output.ServiceType)
                .AddSimple(input => input.OrganizationId, output => output.Organization)
                .AddSimple(input => input.WebPageInUse, output => output.ServiceVoucherInUse)
                // Name is done in header translator
                //.AddDictionary(input => GetName(input, NameTypeEnum.Name), output => output.Name, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetAllAlterName(input), output => output.AlternateName, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.Description), output => output.Description, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.ShortDescription), output => output.ShortDescription, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.DeadLineAdditionalInfo), output => output.DeadLineInformation, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.ProcessingTimeAdditionalInfo), output => output.ProcessingTimeInformation, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.ValidityTimeAdditionalInfo), output => output.ValidityTimeInformation, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.ServiceUserInstruction), output => output.UserInstruction, k => languageCache.GetByValue(k.LocalizationId))
                .AddSimple(input => input.FundingTypeId, output => output.FundingType)
                .AddSimpleList(input => input.OrganizationServices.Select(x => x.OrganizationId), output => output.ResponsibleOrganizations)
                .AddDictionary(input => input.ServiceRequirements, output => output.ConditionOfServiceUsage, k => languageCache.GetByValue(k.LocalizationId))
                .AddNavigation(input => input as IChargeType, output => output.ChargeType)
                //.AddSimpleList(input => input.ServiceLanguages.OrderBy(x => x.OrderNumber).Select(x => x.LanguageId), output => output.Languages)
                .AddNavigation(input => input, output => output.AreaInformation).AddSimpleList(input => input.ServiceTargetGroups.Where(x => !x.Override).Select(x => x.TargetGroupId), output => output.TargetGroups)
                .AddSimpleList(input => input.ServiceTargetGroups.Where(x => x.Override).Select(x => x.TargetGroupId), output => output.OverrideTargetGroups)
                //.AddCollection(input => input.ServiceProducers.OrderBy(x => x.OrderNumber).ThenBy(x => x.Modified), output => output.ServiceProducers)
                //.AddDictionaryList(input => input.ServiceWebPages.Select(x => x.WebPage).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.ServiceVouchers, k => languageCache.GetByValue(k.LocalizationId))
                //.AddCollection(input => input.ServiceLaws.Select(x => x.Law).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.Laws)
                .AddPartial(input => input, output => output as VmServiceHeader)
                .AddNavigation(input => input, output => output.AreaInformation);

            entityDefinitionHelper
                .AddOrderedCollection(
                    definition,
                    i => i.ServiceLaws.Select(x => x.Law),
                    o => o.Laws
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.ServiceWebPages.Select(x => x.WebPage),
                    o => o.ServiceVouchers,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddOrderedSimpleList(
                    definition,
                    input => input.ServiceLanguages,
                    s => s.Select(x => x.LanguageId),
                    o => o.Languages
                )
                .AddOrderedCollection(
                    definition,
                    i => i.ServiceProducers,
                    o => o.ServiceProducers
                );

            return definition.GetFinal();
        }

        private IEnumerable<IName> GetAllAlterName(ServiceVersioned serviceVersioned)
        {
            var alterNames = serviceVersioned.ServiceNames.Where(x => typesCache.Compare<NameType>(x.TypeId, NameTypeEnum.AlternateName.ToString()));
            var langIds = alterNames.Select(x => x.LocalizationId);

            return serviceVersioned.LanguageAvailabilities.Select(x =>
            {
                return langIds.Contains(x.LanguageId) ? alterNames.Single(an => an.LocalizationId == x.LanguageId)
                : new ServiceName { LocalizationId = x.LanguageId, TypeId = typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString()) };
            });
        }

        private IEnumerable<IDescription> GetDescription(ServiceVersioned serviceVersioned, DescriptionTypeEnum type)
        {
            return serviceVersioned.ServiceDescriptions.Where(x => typesCache.Compare<DescriptionType>(x.TypeId, type.ToString()));
        }

        public override ServiceVersioned TranslateVmToEntity(VmServiceBase vModel)
        {
            var names = new List<VmName>();
            var serviceType = typesCache.Get<ServiceType>(ServiceTypeEnum.Service.ToString());
            var vmServiceType = vModel.GeneralDescriptionServiceTypeId.IsAssigned() ? vModel.GeneralDescriptionServiceTypeId : vModel.ServiceType;
            var descriptions = new List<VmDescription>();
            descriptions.AddNullRange(vModel.ShortDescription?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ShortDescription)));
            descriptions.AddNullRange(vModel.Description?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.Description)));
            descriptions.AddNullRange(vModel.DeadLineInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.DeadLineAdditionalInfo, serviceType == vmServiceType)));
            descriptions.AddNullRange(vModel.ProcessingTimeInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ProcessingTimeAdditionalInfo, serviceType == vmServiceType)));
            descriptions.AddNullRange(vModel.ValidityTimeInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ValidityTimeAdditionalInfo, serviceType == vmServiceType)));
            descriptions.AddNullRange(vModel.UserInstruction?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ServiceUserInstruction)));
            descriptions.AddNullRange(vModel.ChargeType?.AdditionalInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ChargeTypeAdditionalInfo)));
            var languageCount = 0;
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .AddLanguageAvailability(i => i, o => o)
                .AddCollectionWithRemove(i => i.ResponsibleOrganizations?.Select(x => new VmTreeItem { Id = x, OwnerReferenceId = i.Id }), o => o.OrganizationServices, r => true)
                .AddSimple(i => i.Organization ?? throw new ArgumentNullException("Organization cannot be empty"), o => o.OrganizationId)
                .AddSimple(i => i.FundingType, o => o.FundingTypeId)
                .AddCollection(i => descriptions, o => o.ServiceDescriptions, true)
                .AddSimple(i => i.GeneralDescriptionId, o => o.StatutoryServiceGeneralDescriptionId)
                .AddSimple(i => i.ServiceVoucherInUse, o => o.WebPageInUse)
                .AddCollection(i => i.ConditionOfServiceUsage?.Select(
                    pair => new VmServiceRequirement() { Id = i.Id, Requirement = pair.Value, LocalizationId = languageCache.Get(pair.Key) }), o => o.ServiceRequirements, true)
                .AddPartial(i =>
                {
                    i.AreaInformation.OwnerReferenceId = i.Id;
                    return i.AreaInformation;
                })
                .Propagation((i, o) =>
                    {
                        names.AddNullRange(CreateNames(vModel, vModel.AlternateName, NameTypeEnum.AlternateName));
                        names.AddNullRange(CreateNamesWithComparison(vModel, vModel.Name, NameTypeEnum.Name,
                            vModel.GeneralDescriptionName, o.ServiceNames));

                    }).AddCollectionWithRemove(i => names, o => o.ServiceNames, r => true)
                .AddCollectionWithRemove(i => i.Languages?.Select(x => new VmListItem { Id = x, OrderNumber = languageCount++, OwnerReferenceId = i.Id }) ?? new List<VmListItem>(), o => o.ServiceLanguages, r => true)
                .AddCollectionWithRemove(i => (i.TargetGroups?.Select(x => new VmTargetGroupListItem { Id = x, OwnerReferenceId = i.Id }) ?? new List<VmTargetGroupListItem>()).Concat(
                        i.OverrideTargetGroups?.Select(x => new VmTargetGroupListItem { Id = x, Override = true, OwnerReferenceId = i.Id }) ?? new List<VmTargetGroupListItem>()),
                    o => o.ServiceTargetGroups, x => true);

            entityDefinitionHelper
                .AddOrderedCollectionWithRemove(
                    definition,
                    i => i.Laws,
                    o => o.ServiceLaws,
                    x => true,
                    (i, o) => o.OwnerReferenceId = i.Id
                )
                .AddOrderedCollectionWithRemove(
                    definition,
                    i => i.ServiceProducers,
                    o => o.ServiceProducers,
                    x => true,
                    (i, o) => o.OwnerReferenceId = i.Id
                )
                .AddOrderedCollectionWithRemove(
                    definition,
                    i => i.ServiceVoucherInUse
                        ? i.ServiceVouchers
                        : new Dictionary<string, List<VmServiceVoucher>>(),
                    o => o.ServiceWebPages,
                    x => true,
                    (i, o, k) =>
                    {
                        o.LocalizationId = languageCache.Get(k);
                        o.OwnerReferenceId = i.Id;
                    }
                );

            return definition.GetFinal();
        }

        private IEnumerable<VmName> CreateNames(VmServiceBase vModel, Dictionary<string, string> name, NameTypeEnum nameType, Dictionary<string, string> gdName = null)
        {
            return name?.Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(pair => 
                    {
                        var inherited = false;
                        if (gdName != null)
                        {
                            if (gdName.ContainsKey(pair.Key))
                            {
                                inherited = pair.Value?.Trim() == gdName[pair.Key]?.Trim();
                            }
                        }

                        return CreateName(pair.Key, pair.Value, vModel, nameType, inherited);
                    }
                );
        }

        private IEnumerable<VmName> CreateNamesWithComparison(VmServiceBase vModel, Dictionary<string, string> name, NameTypeEnum nameType, Dictionary<string, string> gdName, ICollection<ServiceName> previousNames)
        {
            return name?.Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(pair =>
                    {
                        bool? inherited = null;
                        if (vModel.GeneralDescriptionId.IsAssigned())
                        {
                            if (previousNames.Count > 0)
                            {
                                var nameTypeId = typesCache.Get<NameType>(nameType.ToString());

                                var langId = languageCache.Get(pair.Key);
                                inherited = previousNames.Any(
                                    x => x.TypeId == nameTypeId && x.LocalizationId == langId && x.Name?.Trim() == pair.Value.Trim());
                            }

                            if (gdName != null)
                            {
                                if (gdName.ContainsKey(pair.Key))
                                {
                                    inherited = pair.Value?.Trim() == gdName[pair.Key]?.Trim();
                                }
                            }
                        }
                        else
                        {
                            inherited = false;
                        }

                        return CreateName(pair.Key, pair.Value, vModel, nameType, inherited);
                    }
                );
        }

        private VmName CreateName(string language, string value, VmServiceBase vModel, NameTypeEnum typeEnum, bool? inherited = null)
        {
            return new VmName
            {
                Name = value,
                TypeId = typesCache.Get<NameType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language),
                Inherited = inherited
            };
        }

        private VmDescription CreateDescription(string language, string value, VmServiceBase vModel, DescriptionTypeEnum typeEnum, bool isEmpty = false)
        {
            return new VmDescription
            {
                Description = isEmpty ? null : value,
                TypeId = typesCache.Get<DescriptionType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language)
            };
        }
    }

    [RegisterService(typeof(ITranslator<ServiceVersioned, VmServiceOutput>), RegisterType.Transient)]
    internal class ServiceReadTranslator : Translator<ServiceVersioned, VmServiceOutput>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        public ServiceReadTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmServiceOutput TranslateEntityToVm(ServiceVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)

                .AddPartial(i => i, o => o as VmServiceBase)
                .AddCollection(input => input.ServiceServiceClasses.Select(x => x.ServiceClass as IFintoItemBase), output => output.ServiceClasses)
                .AddCollection(input => input.ServiceIndustrialClasses.Select(x => x.IndustrialClass as IFintoItemBase), output => output.IndustrialClasses)
                .AddCollection(input => input.ServiceOntologyTerms.Select(x => x.OntologyTerm as IFintoItemBase).OrderBy(x => x.Uri), output => output.OntologyTerms)
                .AddCollection(input => input.ServiceLifeEvents.Select(x => x.LifeEvent as IFintoItemBase).OrderBy(x => x.Uri), output => output.LifeEvents)
                .AddDictionaryList(input => input.ServiceKeywords.Select(x => x.Keyword), output => output.Keywords, k => languageCache.GetByValue(k.LocalizationId))
                .AddCollection(i => i.UnificRoot.ServiceServiceChannels, o => o.ConnectedChannels);
            //.AddCollection(input => input.UnificRoot.ServiceServiceChannels.Where(x=> x.ServiceChannel.Versions.All(v => v.PublishingStatusId != typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()))), output => output.Connections);

            return definition.GetFinal();
        }

        public override ServiceVersioned TranslateVmToEntity(VmServiceOutput vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceVersioned, VmServiceInput>), RegisterType.Transient)]
    internal class ServiceSaveTranslator : Translator<ServiceVersioned, VmServiceInput>
    {
        private ILanguageCache languageCache;
        private IEntityTreesCache entityTreesCache;

        public ServiceSaveTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            entityTreesCache = cacheManager.EntityTreesCache;
        }

        public override VmServiceInput TranslateEntityToVm(ServiceVersioned entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceVersioned TranslateVmToEntity(VmServiceInput vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .DefineEntitySubTree(i => i.Include(j => j.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.WebPages).ThenInclude(j => j.WebPage))
                .DefineEntitySubTree(i => i.Include(j => j.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.Names))
                .DefineEntitySubTree(i => i.Include(j => j.ServiceWebPages).ThenInclude(j => j.WebPage))
                .DefineEntitySubTree(i => i.Include(j => j.ServiceNames))
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                .UseVersioning<ServiceVersioned, Service>(o => o)
                .AddPartial(i => i as VmServiceBase)
                .AddSimple(i => i.GeneralDescriptionServiceTypeId.HasValue ? null : i.ServiceType, o => o.TypeId)
                .AddSimple(i => i.GeneralDescriptionChargeTypeId.HasValue ? null : i.ChargeType?.ChargeType, o => o.ChargeTypeId)
                .AddCollectionWithRemove(i => i.ServiceClasses?.Where(x => !i.GeneralDescriptionServiceClasses.Contains(x)).Select(x => new VmListItem() { Id = x, OwnerReferenceId = i.Id }), o => o.ServiceServiceClasses, x => true)
                .AddCollectionWithRemove(i => i.OntologyTerms?.Where(x => !i.GeneralDescriptionOntologyTerms.Contains(x)).Select(x => new VmListItem() { Id = x, OwnerReferenceId = i.Id }), o => o.ServiceOntologyTerms, x => true)
                .AddCollectionWithRemove
                (
                    i => IsAssignedTargetGroup(i, "KR1")  
                        ? i.LifeEvents?.Select(x => new VmListItem() { Id = x, OwnerReferenceId = i.Id })
                        : new List<VmListItem>(),
                    o => o.ServiceLifeEvents,
                    x => true
                )
                .AddCollectionWithRemove
                (
                    i => IsAssignedTargetGroup(i, "KR2")
                        ? i.IndustrialClasses?.Select(x => new VmListItem() { Id = x, OwnerReferenceId = i.Id })
                        : new List<VmListItem>(),
                    o => o.ServiceIndustrialClasses,
                    x => true
                )
                .AddCollectionWithRemove(i =>
                {
                    var existingOnes = i.Keywords.SelectMany(pair =>
                    {
                        return pair.Value.Select(kw =>
                            new VmKeywordItem() { Id = kw, OwnerReferenceId = i.Id });
                    });

                    var newKeywords = i.NewKeywords?.SelectMany(pair =>
                    {
                        var localizationId = languageCache.Get(pair.Key);
                        return pair.Value?.Select(
                                   x => new VmKeywordItem() { Name = x, OwnerReferenceId = i.Id, LocalizationId = localizationId }) ??
                               new List<VmKeywordItem>();
                    });

                    return existingOnes.Concat(newKeywords ?? new List<VmKeywordItem>());
                }, o => o.ServiceKeywords, x => true /*c => c.Check(h => h.Keyword).Check(h => h.LocalizationId).Not(RequestLanguageId)*/);

            /*.AddCollectionWithRemove(i => i.Keywords?.Select(x => new VmKeywordItem() { Id = x.Id, OwnerReferenceId = i.Id }).Concat(i.NewKeywords?.Select(
                x => new VmKeywordItem() {Name = x, OwnerReferenceId = i.Id}) ?? new List<VmKeywordItem>()), o => o.ServiceKeywords, x => true);*/

            return definition.GetFinal();
        }

        private bool IsAssignedTargetGroup(VmServiceInput model, string code)
        {
            Guid tgId = entityTreesCache.Get<TargetGroup>(code);
            return model.TargetGroups.Contains(tgId) ||
                   (
                       !(model.OverrideTargetGroups != null && model.OverrideTargetGroups.Contains(tgId)) &&
                       model.GeneralDescriptionTargetGroups.Contains(tgId)
                   );
        }
    }
}