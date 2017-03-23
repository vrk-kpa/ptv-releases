using System;
using PTV.Database.DataAccess.Interfaces.Translators;

namespace PTV.Database.DataAccess.Interfaces.Cloning
{
    public class ClonedTraceIdentification<T> : IClonedTraceIdentification<T>
    {
        public T OriginalEntity { get; set; }
        public T ClonedEntity { get; set; }

        public bool ProcessedByTranslator { get; set; }
        object IClonedTraceIdentification.OriginalEntity
        {
            get { return OriginalEntity; }
            set { OriginalEntity = (T)value; }
        }

        object IClonedTraceIdentification.ClonedEntity
        {
            get { return ClonedEntity; }
            set { ClonedEntity = (T)value; }
        }
    }
}