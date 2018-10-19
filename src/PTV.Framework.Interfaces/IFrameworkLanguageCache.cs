using System;
using System.Collections.Generic;

namespace PTV.Framework.Interfaces
{
    public interface IFrameworkLanguageCache
    {
        List<string> AllowedLanguageCodes { get; }
        List<Guid> AllowedLanguageIds { get; }
    }
}