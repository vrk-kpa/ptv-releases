using System;
using System.Collections.Generic;

namespace DbAnonymizer.models
{
    public class MetaData
    {
        public MetaData()
        {
            TargetLanguageIds = new List<Guid>();
            LanguagesMetaData = new List<LanguagesMetaData>();
        }
        
        public Guid EntityStatusId { get; set; }
        public int HistoryAction { get; set; }
        public List<LanguagesMetaData> LanguagesMetaData { get; set; }
        public Guid? TemplateId { get; set; }
        public Guid? TemplateOrganizationId { get; set; }
        public Guid? SourceLanguageId { get; set; }
        public List<Guid> TargetLanguageIds { get; set; }
        public string ExpirationPeriod { get; set; }
    }

    public class LanguagesMetaData
    {
        public Guid EntityStatusId { get; set; }
        public Guid LanguageId { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? Reviewed { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public DateTime? Archived { get; set; }
        public string ArchivedBy { get; set; }
    }
}