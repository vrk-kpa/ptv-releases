using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class GdSearchResultModel
    {
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public int MaxPageCount { get; set; }
        public bool MoreAvailable { get; set; }
        public int Skip { get; set; }
        public List<GdSearchResultItemModel> Items { get; set; } = new List<GdSearchResultItemModel>();
    }

    public class GdSearchResultItemModel
    {
        public Guid Id { get; set; }
        public Guid UnificRootId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceTypeEnum ServiceType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GeneralDescriptionTypeEnum GeneralDescriptionType { get; set; }
        public Dictionary<LanguageEnum, string> Names { get; set; } = new Dictionary<LanguageEnum, string>();
    }
}
