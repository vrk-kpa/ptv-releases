using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;

namespace PTV.Database.DataAccess.Next
{
    public static class Common
    {
        public static TEnum ToEnum<TEnum>(this string input) where TEnum : struct
        {
            Enum.TryParse<TEnum>(input, out var result);
            return result;
        }

        public static ChargeModel ToChargeModel(this string chargeText)
        {
            return new ChargeModel
            {
                Info = chargeText
            };
        }

        internal static Dictionary<LanguageEnum, List<LinkModel>> ToLawDictionary(this IEnumerable<Law> laws, ILanguageCache languageCache)
        {
            var lawDictionary = new Dictionary<LanguageEnum, List<LinkModel>>();
            foreach (var law in laws)
            {
                foreach (var name in law.Names)
                {
                    var language = languageCache.GetByValue(name.LocalizationId).ToEnum<LanguageEnum>();
                    if (!lawDictionary.ContainsKey(language))
                    {
                        lawDictionary.Add(language, new List<LinkModel>());
                    }

                    var url = law.WebPages.FirstOrDefault(x => x.LocalizationId == name.LocalizationId)?.WebPage.Url;

                    lawDictionary[language].Add(new LinkModel
                    {
                        Name = name.Name.NullToEmpty(),
                        Url = url.NullToEmpty()
                    });
                }
            }

            return lawDictionary;
        }

        private static IEnumerable<VmLanguageAvailabilityInfo> ToOldLanguageModel(this Dictionary<LanguageEnum, PublishingModel> input, string userName, ITypesCache typesCache)
        {
            foreach (var (key, value) in input)
            {
                var languageCode = key.ToString();

                yield return new VmLanguageAvailabilityInfo
                {
                    // IsNew = value.IsNew, // not sure if we need this field
                    LanguageId = typesCache.Get<Language>(languageCode),
                    Modified = DateTime.UtcNow.ToEpochTime(),
                    ModifiedBy = userName,
                    StatusId = typesCache.Get<PublishingStatusType>(value.Status.ToString()),
                    ValidFrom = value.PublishAt.ToEpochTime(),
                    ValidTo = value.ArchiveAt.ToEpochTime()
                };
            }
        }

        internal static VmPublishingModel ToOldViewModel(this Dictionary<LanguageEnum, PublishingModel> input, Guid id, string userName, ITypesCache typesCache)
        {
            return new VmPublishingModel
            {
                Id = id,
                LanguagesAvailabilities = input.ToOldLanguageModel(userName, typesCache).ToList(),
                // PublishAction = PublishActionTypeEnum.SchedulePublishArchive
                UserName = userName
            };
        }

        public static string NullToEmpty(this string input)
        {
            if (input == null) return string.Empty;
            return input;
        }
    }
}
