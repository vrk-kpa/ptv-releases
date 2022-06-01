using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.Service
{
    public static class ToOldModelMapper
    {
        private static VmAreaInformation ToOldViewModel(this AreaInformationModel input, ITypesCache typesCache)
        {
            return new VmAreaInformation
            {
                AreaInformationTypeId = typesCache.Get<AreaInformationType>(input.AreaInformationType.ToString()),
                BusinessRegions = input.BusinessRegions,
                HospitalRegions = input.HospitalRegions,
                Municipalities = input.Municipalities,
                Provinces = input.Provinces
            };
        }

        private static VmChargeType ToOldChargeViewModel(ServiceModel input, ITypesCache typesCache)
        {
            return new VmChargeType
            {
                AdditionalInformation = input.LanguageVersions.ToDictionary(x => x.Key.ToString(), x => x.Value.Charge.Info),
                ChargeType = input.ChargeType.HasValue ? typesCache.Get<ServiceChargeType>(input.ChargeType.ToString()) : (Guid?) null
            };
        }

        private static Dictionary<string, string> ToOldDescription(
            this Dictionary<LanguageEnum, ServiceLanguageVersionModel> input, DescriptionTypeEnum? type)
        {
            switch (type)
            {
                // Null means conditions and criteria
                case null:
                    return input.ToDictionary(x => x.Key.ToString(), x => x.Value.Conditions);
                case DescriptionTypeEnum.DeadLineAdditionalInfo:
                    return input.ToDictionary(x => x.Key.ToString(), x => x.Value.Deadline);
                case DescriptionTypeEnum.Description:
                    return input.ToDictionary(x => x.Key.ToString(), x => x.Value.Description);
                case DescriptionTypeEnum.ProcessingTimeAdditionalInfo:
                    return input.ToDictionary(x => x.Key.ToString(), x => x.Value.ProcessingTime);
                case DescriptionTypeEnum.ShortDescription:
                    return input.ToDictionary(x => x.Key.ToString(), x => x.Value.Summary);
                case DescriptionTypeEnum.ServiceUserInstruction:
                    return input.ToDictionary(x => x.Key.ToString(), x => x.Value.UserInstructions);
                case DescriptionTypeEnum.ValidityTimeAdditionalInfo:
                    return input.ToDictionary(x => x.Key.ToString(), x => x.Value.PeriodOfValidity);
                default:
                    return null;
            }
        }

        private static IEnumerable<VmLanguageAvailabilityInfo> ToOldLanguageAvailabilities(
            this Dictionary<LanguageEnum, ServiceLanguageVersionModel> input, ITypesCache typesCache)
        {
            foreach (var value in input.Values)
            {
                yield return new VmLanguageAvailabilityInfo
                {
                    LanguageId = typesCache.Get<Language>(value.Language.ToString()),
                    StatusId = typesCache.Get<PublishingStatusType>(value.Status.ToString())
                };
            }
        }

        private static List<VmLaw> ToOldLaws(this Dictionary<LanguageEnum, ServiceLanguageVersionModel> input)
        {
            var result = new List<VmLaw>();
            var maxLaws = input.Values.Max(x => x.Laws.Count);
            for (var i = 0; i < maxLaws; i++)
            {
                var newLaw = new VmLaw
                {
                    Name = new Dictionary<string, string>(),
                    WebPage = new Dictionary<string, VmLawWebPage>()
                };

                foreach (var value in input.Values)
                {
                    if (value.Laws.Count > i)
                    {
                        newLaw.Name.Add(value.Language.ToString(), value.Laws[i].Name);
                        newLaw.WebPage.Add(value.Language.ToString(), new VmLawWebPage { UrlAddress = value.Laws[i].Url });
                    }
                }

                result.Add(newLaw);
            }

            return result;
        }

        private static IEnumerable<VmServiceVoucher> ToOldVoucher(this ServiceVoucherModel input, VoucherTypeEnum voucherType)
        {
            switch (voucherType)
            {
                case VoucherTypeEnum.NotUsed:
                    yield break;
                case VoucherTypeEnum.NoUrl:
                    if (string.IsNullOrEmpty(input.Info))
                    {
                        yield break;
                    }

                    yield return new VmServiceVoucher
                    {
                        UrlAddress = null,
                        Name = null,
                        AdditionalInformation = input.Info
                    };
                    break;
                case VoucherTypeEnum.Url:
                    {
                        if (input?.Links == null)
                        {
                            yield break;
                        }

                        foreach (var link in input.Links)
                        {
                            yield return new VmServiceVoucher
                            {
                                UrlAddress = link.Url,
                                Name = link.Name,
                                AdditionalInformation = link.AdditionalInformation
                            };
                        }
                        break;
                    }
            }
        }

        private static IEnumerable<VmServiceProducer> ToOldServiceProviderModel(this ServiceModel input, ITypesCache typesCache)
        {
            if (input.SelfProducers.Any())
            {
                yield return new VmServiceProducer
                {
                    ProvisionType = typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()),
                    OtherProducerType = OtherProducerTypeEnum.Organization,
                    SelfProducers = input.SelfProducers.Select(x => x.Id).ToList(),
                };
            }

            foreach (var purchaseOrg in input.PurchaseProducers)
            {
                yield return new VmServiceProducer
                {
                    ProvisionType = typesCache.Get<ProvisionType>(ProvisionTypeEnum.PurchaseServices.ToString()),
                    OtherProducerType = OtherProducerTypeEnum.Organization,
                    Organization = purchaseOrg.Id,
                };
            }

            foreach (var otherProducerOrg in input.OtherProducers)
            {
                yield return new VmServiceProducer
                {
                    ProvisionType = typesCache.Get<ProvisionType>(ProvisionTypeEnum.Other.ToString()),
                    OtherProducerType = OtherProducerTypeEnum.Organization,
                    Organization = otherProducerOrg.Id,
                };
            }

            foreach (var languageVersion in input.LanguageVersions)
            {
                foreach (var purchaseName in languageVersion.Value?.PurchaseProducerNames)
                {
                    var purchaseAdditionalInfo = new Dictionary<string, string>
                    {
                        {languageVersion.Key.ToString(), purchaseName}
                    };

                    yield return new VmServiceProducer
                    {
                        ProvisionType = typesCache.Get<ProvisionType>(ProvisionTypeEnum.PurchaseServices.ToString()),
                        OtherProducerType = OtherProducerTypeEnum.AdditionalInformation,
                        AdditionalInformation = purchaseAdditionalInfo,
                    };
                }

                foreach (var otherName in languageVersion.Value?.OtherProducerNames)
                {
                    var otherAdditionalInfo = new Dictionary<string, string>
                    {
                        {languageVersion.Key.ToString(), otherName}
                    };

                    yield return new VmServiceProducer
                    {
                        ProvisionType = typesCache.Get<ProvisionType>(ProvisionTypeEnum.Other.ToString()),
                        OtherProducerType = OtherProducerTypeEnum.AdditionalInformation,
                        AdditionalInformation = otherAdditionalInfo,
                    };
                }
            }
        }

        private static Dictionary<string, VmTranslationOrderAvailability> ToOldTranslationAvailabilityModel(
            this ServiceModel input)
        {
            return input.LanguageVersions.Select(x => new {x.Key, x.Value.TranslationAvailability}).ToDictionary(
                x => x.Key.ToString(), 
                x => new VmTranslationOrderAvailability
                {
                    CanBeTranslated = x.TranslationAvailability?.CanBeTranslated ?? false,
                    IsInTranslation = x.TranslationAvailability?.IsInTranslation ?? false,
                    IsTranslationDelivered = x.TranslationAvailability?.IsTranslationDelivered ?? false
                });
        }

        private static Dictionary<string, string> MapNonServiceDescription(ServiceModel input, DescriptionTypeEnum descriptionType)
        {
            if (input.ServiceType == ServiceTypeEnum.Service) return null;
            return input.LanguageVersions.ToOldDescription(descriptionType);
        }
        
        internal static VmServiceInput ToOldViewModel(this ServiceModel input, ActionTypeEnum action,
            ITypesCache typesCache, ITargetGroupDataCache targetGroupDataCache, Guid defaultOrganizationId)
        {
            var versionParts = input.Version.Split(".");

            // TODO: commented lines may need additional mapping
            var result = new VmServiceInput
            {
                Action = action,
                AlternateName =
                    input.LanguageVersions.ToDictionary(x => x.Key.ToString(), x => x.Value.AlternativeName),
                // AlternativeId = null,
                AreaInformation = input.AreaInformation.ToOldViewModel(typesCache),
                ChargeType = ToOldChargeViewModel(input, typesCache),
                ConditionOfServiceUsage = input.LanguageVersions.ToOldDescription(null),
                DeadLineInformation = MapNonServiceDescription(input,DescriptionTypeEnum.DeadLineAdditionalInfo),
                Description = input.LanguageVersions.ToOldDescription(DescriptionTypeEnum.Description),
                // ExistingKeywords = 
                // ExpireOn = 
                FundingType = typesCache.Get<ServiceFundingType>(input.FundingType.ToString()),
                Id = input.Id,
                IndustrialClasses = input.IndustrialClasses,
                // IsExpireWarningVisible =
                // IsNotUpdatedWarningVisible =
                // TODO: In the old UI, this was filled with keywords from previous versions
                Keywords = new Dictionary<string, List<Guid>>(),
                Languages = input.Languages.Select(x => typesCache.Get<Language>(x)).ToList(),
                LanguagesAvailabilities = input.LanguageVersions.ToOldLanguageAvailabilities(typesCache).ToList(),
                Laws = input.LanguageVersions.ToOldLaws(),
                LifeEvents = input.LifeEvents,
                // MissingLanguages = 
                Name = input.LanguageVersions.ToDictionary(x => x.Key.ToString(), x => x.Value.Name),
                // TODO: Is it OK to map all keywords as new?
                NewKeywords = input.LanguageVersions.ToDictionary(x => x.Key.ToString(), x => x.Value.Keywords),
                // NumberOfConnections = 
                // Oid = 
                OntologyTerms = input.OntologyTerms.Select(x => x.Id).ToList(),
                Organization = input.ResponsibleOrganization?.Id ?? defaultOrganizationId,
                // OverrideTargetGroups =
                // PreviousInfo =
                ProcessingTimeInformation = MapNonServiceDescription(input,DescriptionTypeEnum.ProcessingTimeAdditionalInfo),
                Publish = action == ActionTypeEnum.SaveAndPublish,
                // PublishAction = 
                // PublishingStatus =
                ResponsibleOrganizations = input.OtherResponsibleOrganizations.Select(x => x.Id).ToList(),
                ServiceClasses = input.ServiceClasses,
                ServiceProducers = input.ToOldServiceProviderModel(typesCache).ToList(),
                ServiceType = typesCache.Get<ServiceType>(input.ServiceType.ToString()),
                ServiceVoucherInUse = input.LanguageVersions.Values.Any(x => x.Voucher?.Links?.Count > 0),
                VoucherType = typesCache.Get<VoucherType>(input.VoucherType.ToString()),
                ServiceVouchers = input.LanguageVersions.ToDictionary(x => x.Key.ToString(),
                    x => x.Value.Voucher?.ToOldVoucher(input.VoucherType)?.ToList()),
                ShortDescription = input.LanguageVersions.ToOldDescription(DescriptionTypeEnum.ShortDescription),
                TargetGroups = targetGroupDataCache.GetByCode(input.TargetGroups).Select(x => x.Id).ToList(),
                // TemplateId = 
                // TemplateOrganizationId = 
                TranslationAvailability = input.ToOldTranslationAvailabilityModel(),
                UnificRootId = input.UnificRootId ?? Guid.Empty,
                UserInstruction = input.LanguageVersions.ToOldDescription(DescriptionTypeEnum.ServiceUserInstruction),
                // UserName =
                ValidityTimeInformation = MapNonServiceDescription(input, DescriptionTypeEnum.ValidityTimeAdditionalInfo),
                Version = new VmVersion
                    {Major = versionParts[0].ParseToInt() ?? 0, Minor = versionParts[1].ParseToInt() ?? 0}
            };

            if (input.GeneralDescription != null)
            {
                var gd = input.GeneralDescription;

                result.GeneralDescriptionChargeTypeId = gd.ChargeType.HasValue
                    ? typesCache.Get<ServiceChargeType>(gd.ChargeType.ToString())
                    : (Guid?)null;
                result.GeneralDescriptionId = gd?.UnificRootId;
                result.GeneralDescriptionKeywords = gd?.LanguageVersions.SelectMany(x =>
                        x.Value.Keywords.Select(y => new VmKeywordItem {LocalizationCode = x.Key.ToString(), Name = y}))
                    .ToList();
                result.GeneralDescriptionName =
                    gd?.LanguageVersions.ToDictionary(x => x.Key.ToString(), x => x.Value.Name);
                result.GeneralDescriptionOntologyTerms = gd?.OntologyTerms.Select(x => x.Id).ToList();
                result.GeneralDescriptionServiceClasses = gd?.ServiceClasses;
                result.GeneralDescriptionServiceTypeId =
                    typesCache.Get<GeneralDescriptionType>(gd?.GeneralDescriptionType.ToString());
                result.GeneralDescriptionTargetGroups =
                    targetGroupDataCache.GetByCode(gd?.TargetGroups).Select(x => x.Id).ToList();
            }

            return result;
        }
    }

}