using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class GdSearchModel
    {
        public string Name { get; set; }
        public int PageNumber { get; set; }
        public int MaxPageCount { get; set; }
        public int Skip { get; set; }
        public LanguageEnum SortLanguage { get; set; }
        public List<VmSortParam> SortData { get; set; } = new List<VmSortParam>();
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceTypeEnum? ServiceType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GeneralDescriptionTypeEnum? GeneralDescriptionType { get; set; }
    }
}
