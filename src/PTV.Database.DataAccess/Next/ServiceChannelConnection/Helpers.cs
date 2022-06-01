using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    internal static class Helpers
    {
        public static List<(Guid LanguageId, LanguageEnum Language)> ToLanguageList(ILanguageCache cache, List<Guid> languages)
        {
            return languages.Select(x =>
            {
                return (LanguageId: x, Language: MapLanguage(cache, x));
            }).ToList();
        }

        public static LanguageEnum MapLanguage(ILanguageCache languageCache, Guid languageId)
        {
            var languageCode = languageCache.GetByValue(languageId);
            return languageCode.ToEnum<LanguageEnum>();
        }
    }
}
