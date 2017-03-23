using System;

namespace PTV.Database.DataAccess.Interfaces.Translators
{
    [Flags]
    public enum TranslationPolicy
    {
        Defaults = 0,
        MergeOutputCollections = 1
    }
}