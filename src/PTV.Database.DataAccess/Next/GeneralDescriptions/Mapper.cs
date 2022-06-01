using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Framework;
using PTV.Next.Model;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.GeneralDescriptions
{
    internal static class Mapper
    {
        private static string ToDescriptionModel(this List<StatutoryServiceDescription> descriptions, ITypesCache typesCache, DescriptionTypeEnum type)
        {
            return descriptions
                .FirstOrDefault(x => x.TypeId == typesCache.Get<DescriptionType>(type.ToString()))
                ?.Description;
        }

        internal static GeneralDescriptionLanguageVersionModel ToVersionModel(
            this GeneralDescriptionLanguageAvailability input,
            StatutoryServiceGeneralDescriptionVersioned gd,
            ITypesCache typesCache,
            ILanguageCache languageCache)
        {
            var languageId = input.LanguageId;
            var languageCode = languageCache.GetByValue(languageId);
            var names = gd.Names.Where(x => x.LocalizationId == languageId).ToList();
            var normalName = names.FirstOrDefault(x => x.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()));
            var alternativeName = names.FirstOrDefault(x => x.TypeId == typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString()));
            var descriptions = gd.Descriptions.Where(x => x.LocalizationId == languageId).ToList();

            var model = new GeneralDescriptionLanguageVersionModel();
            model.Name = normalName?.Name ?? string.Empty;
            model.AlternativeName = alternativeName?.Name ?? string.Empty;
            model.Status = typesCache.GetByValue<PublishingStatusType>(input.StatusId).ToEnum<PublishingStatus>();
            model.Language = languageCode.ToEnum<LanguageEnum>();
            model.Description = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.Description);
            model.Summary = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.ShortDescription).NullToEmpty();
            model.UserInstructions = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.ServiceUserInstruction);
            model.Deadline = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.DeadLineAdditionalInfo);
            model.ProcessingTime = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.ProcessingTimeAdditionalInfo);
            model.PeriodOfValidity = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.ValidityTimeAdditionalInfo);
            model.Conditions = gd.StatutoryServiceRequirements.FirstOrDefault(x => x.LocalizationId == languageId)?.Requirement;
            model.Charge = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.ChargeTypeAdditionalInfo).ToChargeModel();
            model.Keywords = gd.Keywords.Where(x => x.Keyword.LocalizationId == input.LanguageId).Select(x => x.Keyword.Name).ToList();
            model.BackgroundDescription = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.BackgroundDescription);
            model.GeneralDescriptionTypeAdditionalInformation = descriptions.ToDescriptionModel(typesCache, DescriptionTypeEnum.GeneralDescriptionTypeAdditionalInformation);
            return model;
        }

        internal static GeneralDescriptionModel ToModel(this StatutoryServiceGeneralDescriptionVersioned input,
            ITypesCache typesCache,
            ILanguageCache languageCache,
            List<GdServiceChannelModel> channels)
        {
            var model = new GeneralDescriptionModel();
            model.Id = input.Id;
            model.UnificRootId = input.UnificRootId;
            model.Status = typesCache.GetByValue<PublishingStatusType>(input.PublishingStatusId).ToEnum<PublishingStatus>();
            model.GeneralDescriptionType = typesCache.GetByValue<GeneralDescriptionType>(input.GeneralDescriptionTypeId).ToEnum<GeneralDescriptionTypeEnum>();
            model.ChargeType = input.ChargeTypeId.HasValue
                ? typesCache.GetByValue<ServiceChargeType>(input.ChargeTypeId.Value).ToEnum<ServiceChargeTypeEnum>()
                : (ServiceChargeTypeEnum?) null;
            model.ServiceType = typesCache.GetByValue<ServiceType>(input.TypeId).ToEnum<ServiceTypeEnum>();
            model.LanguageVersions = input.LanguageAvailabilities.Select(x => x.ToVersionModel(input, typesCache, languageCache)).ToDictionary(x => x.Language);
            model.Channels = channels;
            return model;
        }

        internal static GeneralDescriptionModel FillInCategorization(this GeneralDescriptionModel input, StatutoryServiceGeneralDescriptionVersioned gd, ILanguageCache languageCache)
        {
            input.TargetGroups = gd.TargetGroups.Select(x => x.TargetGroup.Code).Distinct().ToList();
            input.IndustrialClasses = gd.IndustrialClasses.Select(x => x.IndustrialClassId).Distinct().ToList();
            input.ServiceClasses = gd.ServiceClasses.Select(x => x.ServiceClassId).Distinct().ToList();
            input.LifeEvents = gd.LifeEvents.Select(x => x.LifeEventId).Distinct().ToList();
            input.OntologyTerms = gd.OntologyTerms.Select(x => ToOntologyTerm(x, languageCache)).DistinctBy(x => x.Id).ToList();
            return input;
        }

        internal static OntologyTermModel ToOntologyTerm(StatutoryServiceOntologyTerm source, ILanguageCache languageCache)
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
        
        internal static GeneralDescriptionModel FillInAdditionalInfo(
            this GeneralDescriptionModel input,
            IEnumerable<Law> laws,
            ILanguageCache languageCache)
        {
            var lawDictionary = laws.ToLawDictionary(languageCache);
            foreach (var (key, value) in input.LanguageVersions)
            {
                value.Laws = lawDictionary.TryGetOrDefault(key, new List<LinkModel>());
            }

            return input;
        }

        internal static GdSearchResultModel Map(this VmSearchResult<VmServiceGeneralDescriptionListItem> source, ITypesCache typesCache)
        {
            var result = new GdSearchResultModel();
            result.Count = source.Count;
            result.PageNumber = source.PageNumber;
            result.MoreAvailable = source.MoreAvailable;
            result.Skip = source.Skip;
            result.MaxPageCount = source.MaxPageCount;

            if (source.SearchResult != null)
            {
                result.Items = source.SearchResult.Select(x => Map(x, typesCache)).ToList();
            }

            return result;
        }

        internal static VmGeneralDescriptionSearchForm Map(this GdSearchModel input, ITypesCache typesCache)
        {
            var output = new VmGeneralDescriptionSearchForm();
            output.PageNumber = input.PageNumber;
            output.MaxPageCount = input.MaxPageCount;
            output.Skip = input.Skip;
            output.SortData = input.SortData;
            output.Name = input.Name;
            output.Languages = new List<string> { input.SortLanguage.ToString() };

            if (input.ServiceType.HasValue)
            {
                output.ServiceTypeId = typesCache.Get<ServiceType>(input.ServiceType.Value.ToString());
            }

            if (input.GeneralDescriptionType.HasValue)
            {
                output.GeneralDescriptionTypeId = typesCache.Get<GeneralDescriptionType>(input.GeneralDescriptionType.Value.ToString());
            }

            return output;
        }

        internal static GdSearchResultItemModel Map(VmServiceGeneralDescriptionListItem source, ITypesCache typesCache)
        {
            var result = new GdSearchResultItemModel();
            result.Id = source.Id;
            result.UnificRootId = source.UnificRootId;
            result.ServiceType = typesCache.GetByValue<ServiceType>(source.ServiceTypeId).ToEnum<ServiceTypeEnum>();
            result.GeneralDescriptionType = typesCache.GetByValue<GeneralDescriptionType>(source.GeneralDescriptionTypeId).ToEnum<GeneralDescriptionTypeEnum>();

            result.Names = source.Name.Select(kvp => new
            {
                Language = kvp.Key.ToEnum<LanguageEnum>(),
                Name = kvp.Value
            }).ToDictionary(y => y.Language, y => y.Name);

            return result;
        }
    }
}
