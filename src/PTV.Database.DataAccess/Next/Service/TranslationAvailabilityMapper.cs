using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.Service
{
    public class TranslationAvailabilityMapper
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;
        private readonly IValidationManager validationManager;

        private readonly List<TranslationStateTypeEnum> InTranslationStates = new List<TranslationStateTypeEnum>
        {
            TranslationStateTypeEnum.ReadyToSend,
            TranslationStateTypeEnum.Sent,
            TranslationStateTypeEnum.InProgress,
            TranslationStateTypeEnum.FileError,
            TranslationStateTypeEnum.SendError,
            TranslationStateTypeEnum.DeliveredFileError,
            TranslationStateTypeEnum.FailForInvestigation,
            TranslationStateTypeEnum.RequestForRepetition,
            TranslationStateTypeEnum.RequestForCancel
        };
        private readonly List<TranslationStateTypeEnum> DeliveredStates = new List<TranslationStateTypeEnum>
        {
            TranslationStateTypeEnum.Arrived
        };

        internal TranslationAvailabilityMapper(
            ILanguageCache languageCache, 
            ITypesCache typesCache,
            IValidationManager validationManager)
        {
            this.languageCache = languageCache;
            this.typesCache = typesCache;
            this.validationManager = validationManager;
        }
        
        private bool CanBeTranslatedViaGD(ServiceModel input)
        {
            return (input.AreaInformation?.Municipalities?.Any() == true ||
                    input.AreaInformation?.AreaTypes?.Contains(AreaTypeEnum.BusinessRegions) == true) &&
                   input.FundingType == ServiceFundingTypeEnum.PubliclyFunded &&
                   (input.TargetGroups?.Contains(TargetGroupCache.EnterpriseCode) == true ||
                    (input.GeneralDescription?.TargetGroups?.Contains(TargetGroupCache.EnterpriseCode) == true)) &&
                   input.GeneralDescription?.GeneralDescriptionType == GeneralDescriptionTypeEnum.BusinessSubregion;
        }
        
        public bool CanBeTranslated(ServiceModel input)
        {
            return (input.AreaInformation.AreaInformationType == AreaInformationTypeEnum.WholeCountry ||
                    input.AreaInformation.AreaInformationType ==
                    AreaInformationTypeEnum.WholeCountryExceptAlandIslands) &&
                   input.FundingType == ServiceFundingTypeEnum.PubliclyFunded;
        }

        internal ServiceModel FillInTranslationOrders<T>(ServiceModel input, List<TranslationOrderState> latestTranslationOrders, IUnitOfWork unitOfWork)
        {
            var models = latestTranslationOrders.Select(x => new LastTranslationModel
            {
                SourceLanguage = languageCache.GetByValue(x.TranslationOrder.SourceLanguageId).ToEnum<LanguageEnum>(),
                TargetLanguage = languageCache.GetByValue(x.TranslationOrder.TargetLanguageId).ToEnum<LanguageEnum>(),
                State = typesCache.GetByValue<TranslationStateType>(x.TranslationStateId).ToEnum<TranslationStateTypeEnum>(),
                EstimatedDelivery = x.TranslationOrder.DeliverAt,
                OrderedAt = x.SendAt,
                Checked = x.Checked,
                TranslationId = x.TranslationOrderId
            })
            .GroupBy(x => x.TargetLanguage)
            .Select(x => x.OrderByDescending(y => y.OrderedAt).FirstOrDefault())
            .ToList();

            input.LastTranslations = models;
        
            if (CanBeTranslated(input) || CanBeTranslatedViaGD(input))
            {
                var validationResult = Check<T>(input.Id, unitOfWork);
                foreach (var version in input.LanguageVersions.Values)
                {
                    var languageTranslationOrder = models.FirstOrDefault(x => x.TargetLanguage == version.Language);
                    var languageTranslationState = languageTranslationOrder?.State;

                    version.TranslationAvailability = new TranslationAvailabilityModel
                    {
                        CanBeTranslated = !validationResult.ContainsKey(version.Language) &&
                            languageCache.TranslationOrderLanguageCodes.Contains(version.Language.ToString()),
                        IsInTranslation = IsInState(languageTranslationState, InTranslationStates),
                        IsTranslationDelivered = (languageTranslationOrder?.Checked ?? false) &&
                                                 IsInState(languageTranslationState, DeliveredStates)
                    };
                }
            }

            return input;
        }

        private Dictionary<LanguageEnum, bool> Check<T>(Guid? id, IUnitOfWork unitOfWork)
        {
            if (id == null)
            {
                return new Dictionary<LanguageEnum, bool>();
            }
            
            return validationManager.CheckEntity<T>(id.Value, unitOfWork)
                    .ToDictionary(x => languageCache.GetByValue(x.Key).ToEnum<LanguageEnum>(), x => x.Value.Any());
        }

        private bool IsInState(TranslationStateTypeEnum? input, List<TranslationStateTypeEnum> toCheck)
        {
            return input.HasValue && toCheck.Contains(input.Value);
        }
    }
}