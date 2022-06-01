using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Next.Organization;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.Service
{
    public static class Mapper
    {
        private static string ToDescriptionModel(this List<ServiceDescription> descriptions, ITypesCache typesCache, DescriptionTypeEnum type)
        {
            return descriptions
                .FirstOrDefault(x => x.TypeId == typesCache.Get<DescriptionType>(type.ToString()))
                ?.Description;
        }

        private static ServiceLanguageVersionModel ToModel(this ServiceLanguageAvailability input,
            ServiceVersioned service, ILanguageCache languageCache, ITypesCache typesCache)
        {
            var languageId = input.LanguageId;
            var languageCode = languageCache.GetByValue(languageId);
            var names = service.ServiceNames.Where(x => x.LocalizationId == languageId).ToList();
            var normalName = names.FirstOrDefault(x => x.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()));
            var alternativeName = names.FirstOrDefault(x => x.TypeId == typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString()));
            var descriptions = service.ServiceDescriptions.Where(x => x.LocalizationId == languageId).ToList();

            return new ServiceLanguageVersionModel
            {
                AlternativeName = alternativeName?.Name ?? string.Empty,
                Charge = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.ChargeTypeAdditionalInfo)
                    .ToChargeModel(),
                Conditions = service.ServiceRequirements.FirstOrDefault(x => x.LocalizationId == languageId)
                    ?.Requirement,
                Deadline = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.DeadLineAdditionalInfo),
                Description = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.Description),
                HasAlternativeName = alternativeName != null,
                Language = languageCode.ToEnum<LanguageEnum>(),
                Name = normalName?.Name ?? string.Empty,
                PeriodOfValidity =
                    descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.ValidityTimeAdditionalInfo),
                ProcessingTime =
                    descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.ProcessingTimeAdditionalInfo),
                Status = typesCache.GetByValue<PublishingStatusType>(input.StatusId).ToEnum<PublishingStatus>(),
                Summary = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.ShortDescription).NullToEmpty(),
                UserInstructions =
                    descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.ServiceUserInstruction),
                Keywords = service.ServiceKeywords.Where(x => x.Keyword.LocalizationId == input.LanguageId)
                    .Select(x => x.Keyword.Name).ToList(),
                Modified = input.Modified,
                ModifiedBy = input.ModifiedBy,
                ScheduledArchive = input.ArchiveAt,
                ScheduledPublish = input.PublishAt
            };
        }

        internal static ServiceModel ToModel(this ServiceVersioned input, 
            ILanguageCache languageCache,
            ITypesCache typesCache,
            IOrganizationMapper organizationMapper)
        {
            return new ServiceModel
            {
                Id = input.Id,
                UnificRootId = input.UnificRootId,
                Status = typesCache.GetByValue<PublishingStatusType>(input.PublishingStatusId).ToEnum<PublishingStatus>(),
                ResponsibleOrganization = organizationMapper.Map(input.OrganizationId),
                OtherResponsibleOrganizations = input.OrganizationServices.Select(x => organizationMapper.Map(x.OrganizationId)).ToList(),
                Languages = input.ServiceLanguages.Select(x => languageCache.GetByValue(x.LanguageId)).ToList(),
                ChargeType = input.ChargeTypeId.HasValue
                    ? typesCache.GetByValue<ServiceChargeType>(input.ChargeTypeId.Value).ToEnum<ServiceChargeTypeEnum>()
                    : (ServiceChargeTypeEnum?)null,
                FundingType = input.FundingTypeId.HasValue
                    ? typesCache.GetByValue<ServiceFundingType>(input.FundingTypeId.Value)
                        .ToEnum<ServiceFundingTypeEnum>()
                    : default,
                ServiceType = input.TypeId.HasValue
                    ? typesCache.GetByValue<ServiceType>(input.TypeId.Value).ToEnum<ServiceTypeEnum>()
                    : default,
                LanguageVersions = input.LanguageAvailabilities.Select(x => x.ToModel(input, languageCache, typesCache))
                    .ToDictionary(x => x.Language),
                VoucherType = input.VoucherTypeId.HasValue
                    ? typesCache.GetByValue<VoucherType>(input.VoucherTypeId.Value)
                        .ToEnum<VoucherTypeEnum>()
                    : VoucherTypeEnum.NotUsed,
                Version = $"{input.Versioning.VersionMajor}.{input.Versioning.VersionMinor}"
            };
        }

        internal static ServiceModel FillInCategorization(this ServiceModel input, ServiceVersioned service, ILanguageCache languageCache)
        {
            input.TargetGroups = service.ServiceTargetGroups.Select(x => x.TargetGroup.Code).Distinct().ToList();
            input.IndustrialClasses = service.ServiceIndustrialClasses.Select(x => x.IndustrialClassId).Distinct().ToList();
            input.ServiceClasses = service.ServiceServiceClasses.Select(x => x.ServiceClassId).Distinct().ToList();
            input.LifeEvents = service.ServiceLifeEvents.Select(x => x.LifeEventId).Distinct().ToList();
            input.OntologyTerms = service.ServiceOntologyTerms.Select(x => ToOntologyTerm(x, languageCache)).DistinctBy(x => x.Id).ToList();
            return input;
        }

        internal static OntologyTermModel ToOntologyTerm(ServiceOntologyTerm source, ILanguageCache languageCache)
        {
            return new OntologyTermModel
            {
                Id = source.OntologyTermId,
                Code = source.OntologyTerm.Code,
                Names = source.OntologyTerm.Names
                    .Select(y => new { Language = languageCache.GetByValue(y.LocalizationId).ToEnum<LanguageEnum>()
                        , y.Name} )
                    .ToDictionary(y => y.Language, y => y.Name)
            };
        }
        
        internal static ServiceModel FillInAreas(this ServiceModel input, ServiceVersioned service, ITypesCache typesCache)
        {
            var areasByType = service.Areas.Select(x => x.Area)
                .GroupBy(x => typesCache.GetByValue<AreaType>(x.AreaTypeId).ToEnum<AreaTypeEnum>())
                .ToDictionary(x => x.Key, x => x.Select(x => x.Id).ToList());

            var municipalityIds = service.AreaMunicipalities.Select(x => x.MunicipalityId).ToList();
            var selectedAreaTypes = areasByType.Keys.ToList();
            if (municipalityIds.Any() && !selectedAreaTypes.Contains(AreaTypeEnum.Municipality))
            {
                selectedAreaTypes.Add(AreaTypeEnum.Municipality);
            }
            
            input.AreaInformation = new AreaInformationModel
            {
                AreaInformationType = typesCache.GetByValue<AreaInformationType>(service.AreaInformationTypeId)
                    .ToEnum<AreaInformationTypeEnum>(),
                AreaTypes = selectedAreaTypes,
                BusinessRegions = areasByType.TryGetOrDefault(AreaTypeEnum.BusinessRegions, new List<Guid>()),
                HospitalRegions = areasByType.TryGetOrDefault(AreaTypeEnum.HospitalRegions, new List<Guid>()),
                Municipalities = municipalityIds,
                Provinces = areasByType.TryGetOrDefault(AreaTypeEnum.Province, new List<Guid>())
            };

            return input;
        }
        
        internal static ServiceModel FillInServiceProviders(this ServiceModel input, 
            IEnumerable<ServiceProducer> serviceProducers, 
            ITypesCache typesCache, 
            ILanguageCache languageCache,
            IOrganizationMapper organizationMapper)
        { 
            var providerByType = serviceProducers.Select(x => x)
                .GroupBy(x => typesCache.GetByValue<ProvisionType>(x.ProvisionTypeId).ToEnum<ProvisionTypeEnum>())
                .ToDictionary(x => x.Key, x => x.Select(x => new { x.Id, x.Organizations, x.AdditionalInformations }).ToList());

            input.SelfProducers = providerByType.TryGet(ProvisionTypeEnum.SelfProduced)
                ?.SelectMany(x => x.Organizations.Select(o => organizationMapper.Map(o.OrganizationId)))
                .Distinct()
                .ToList() ?? new List<OrganizationModel>();
            
            input.PurchaseProducers = providerByType.TryGet(ProvisionTypeEnum.PurchaseServices)
                ?.SelectMany(x => x.Organizations.Select(o => organizationMapper.Map(o.OrganizationId)))
                .Distinct()
                .ToList() ?? new List<OrganizationModel>();
            
            input.OtherProducers = providerByType.TryGet(ProvisionTypeEnum.Other)
                ?.SelectMany(x => x.Organizations.Select(o => organizationMapper.Map(o.OrganizationId)))
                .Distinct()
                .ToList() ?? new List<OrganizationModel>();
            
            var purchaseServiceAdditionalInformationList = providerByType.TryGet(ProvisionTypeEnum.PurchaseServices)
                ?.SelectMany(x => x.AdditionalInformations)
                .ToList() ?? new List<ServiceProducerAdditionalInformation>();
            
            var purchaseAdditionalInformationDictionary = purchaseServiceAdditionalInformationList
                .GroupBy(x => languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>())
                .ToDictionary(x => x.Key, x => x.Select(x => x.Text)
                .ToList());
            
            var otherServiceAdditionalInformationList = providerByType.TryGet(ProvisionTypeEnum.Other)
                ?.SelectMany(x => x.AdditionalInformations)
                .ToList() ?? new List<ServiceProducerAdditionalInformation>();

            var otherAdditionalInformationDictionary = otherServiceAdditionalInformationList
                .GroupBy(x => languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>())
                .ToDictionary(x => x.Key, x => x.Select(x => x.Text)
                .ToList());
            
            foreach (var (key, value) in input.LanguageVersions)
            {
                value.PurchaseProducerNames = purchaseAdditionalInformationDictionary.TryGetOrDefault(key, new List<string>());
                value.OtherProducerNames = otherAdditionalInformationDictionary.TryGetOrDefault(key, new List<string>());
            }
            
            return input;
        }

        internal static ServiceModel FillInAdditionalInfo(
            this ServiceModel input,
            IEnumerable<Law> laws,
            IEnumerable<ServiceWebPage> vouchers,
            ILanguageCache languageCache)
        {
            var lawDictionary = laws.ToLawDictionary(languageCache);

            var voucherDictionary = vouchers
                .GroupBy(x => languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>())
                .ToDictionary(x => x.Key, x => x.Select(x => new AdditionalInfoLinkModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Url = x.WebPage.Url,
                    AdditionalInformation = x.Description,
                }).ToList());

            foreach (var (key, value) in input.LanguageVersions)
            {
                var languageVouchers = voucherDictionary.TryGetOrDefault(key);
                value.Laws = lawDictionary.TryGetOrDefault(key, new List<LinkModel>());
                
                if (input.VoucherType == VoucherTypeEnum.NotUsed)
                {
                    value.Voucher = new ServiceVoucherModel { Info = string.Empty, Links = new List<AdditionalInfoLinkModel>() };
                }

                if (input.VoucherType == VoucherTypeEnum.Url)
                {
                    value.Voucher = new ServiceVoucherModel { Info = string.Empty, Links = languageVouchers ?? new List<AdditionalInfoLinkModel>() };
                }
                
                if (input.VoucherType == VoucherTypeEnum.NoUrl)
                {
                    value.Voucher = new ServiceVoucherModel { Info = languageVouchers?.Select(x => x.AdditionalInformation).FirstOrDefault() ?? string.Empty, Links = new List<AdditionalInfoLinkModel>() };
                }
            }

            return input;
        }
    }
}
