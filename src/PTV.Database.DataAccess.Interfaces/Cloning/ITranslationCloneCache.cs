using System;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Translators;

namespace PTV.Database.DataAccess.Interfaces.Cloning
{
    public interface ITranslationCloneCache
    {
        void AddToCachedSet<T>(T entityOriginal, T entityCloned) where T : class;

        List<IClonedTraceIdentification<T>> GetFromCachedSet<T>() where T : class;

        IList<object> GetEntitiesCascade(IList<object> rootEntites);

        void MarkAsProcessedByTranslator<T>(T clonedEntity) where T : class;
    }
}