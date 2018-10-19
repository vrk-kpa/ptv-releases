using System;
using System.Collections.Generic;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Interfaces.Caches
{
    public interface ILanguageCache : IEntityCache<string, Guid>
    {
        List<string> AllowedLanguageCodes { get; }
        List<Guid> AllowedLanguageIds { get; }
        
        List<string> TranslationOrderLanguageCodes { get; }
        List<Guid> TranslationOrderLanguageIds { get; }

        List<string> LanguageCodes { get; }
    }
}