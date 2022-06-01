using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.TranslationOrders
{
    public static class TranslationOrderMapper
    {
        internal static TranslationDetailModel ToModel(this TranslationOrder input, ILanguageCache languageCache)
        {
            return new TranslationDetailModel
            {
                AdditionalInformation = input.AdditionalInformation,
                OrderedAt = input.Created,
                OrderId = input.Id,
                OrderNumber = input.OrderIdentifier,
                SourceLanguage = languageCache.GetByValue(input.SourceLanguageId).ToEnum<LanguageEnum>(),
                SubscriberEmail = input.SenderEmail,
                TargetLanguage = languageCache.GetByValue(input.TargetLanguageId).ToEnum<LanguageEnum>(),
            };
        }
        
        internal static TranslationHistoryModel ToNewModel(this VmTranslationOrderStateOutput input,
            ITypesCache typesCache)
        {
            return new TranslationHistoryModel
            {
                EstimatedDelivery = input.DeliverAt.FromEpochTime(),
                OrderedAt = input.SentAt.FromEpochTime(),
                OrderId = input.Id,
                OrderNumber = input.TranslationOrder.OrderIdentifier,
                SourceLanguage = input.TranslationOrder.SourceLanguageCode.ToEnum<LanguageEnum>(),
                State = typesCache.GetByValue<TranslationStateType>(input.TranslationStateTypeId)
                    .ToEnum<TranslationStateTypeEnum>(),
                SubscriberEmail = input.TranslationOrder.SenderEmail,
                TargetLanguage = input.TranslationOrder.TargetLanguageCode.ToEnum<LanguageEnum>()
            };
        }
    }
}