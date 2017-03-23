using System;

namespace PTV.Database.DataAccess.Interfaces.Cloning
{
    public interface IClonedTraceIdentification<T> : IClonedTraceIdentification
    {
        new T OriginalEntity { get; set; }
        new T ClonedEntity { get; set; }
    }

    public interface IClonedTraceIdentification
    {
        object OriginalEntity { get; set; }
        object ClonedEntity { get; set; }
        bool ProcessedByTranslator { get; set; }
    }
}