using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next
{
    public static class SimpleLanguageVersionMapper
    {
        internal static Dictionary<LanguageEnum, SimpleLanguageVersionModel> GetLanguageVersions<T>(this T input,
            ITypesCache typesCache) where T : ILanguagesAvailabilities, IName
        {
            return input.LanguagesAvailabilities.Select(x =>
                {
                    var languageCode = typesCache.GetByValue<Language>(x.LanguageId);
                    var status = typesCache.GetByValue<PublishingStatusType>(x.StatusId).ToEnum<PublishingStatus>();
                    var language = languageCode.ToEnum<LanguageEnum>();
                    var name = input.Name.TryGetOrDefault(languageCode);

                    if (name == null)
                    {
                        return null;
                    }

                    var result = new
                    {
                        Key = language, Value = new SimpleLanguageVersionModel
                        {
                            Status = status,
                            Name = name,
                            IsScheduled = x.ValidFrom.HasValue
                        }
                    };
                    return result;
                })
                .Where(x => x != null)
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}