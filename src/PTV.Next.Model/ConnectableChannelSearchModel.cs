using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class ConnectableChannelSearchModel
    {
        public string Language { get; set; }
        public Guid? Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DomainEnum Type { get; set; }

        public Guid? OrganizationId { get; set; }
        public string Name { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChannelTypeEnum? ChannelType {get; set;}

        public List<VmSortParam> SortData { get; set; } = new List<VmSortParam>();
    }

    public class ConnectableChannelSearchResultModel
    {
        public int Skip { get; set; }
        public int PageNumber { get; set; }
        public bool MoreAvailable { get; set; }
        public int MaxPageCount { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public List<ConnectableChannelModel> Items { get; set; } = new List<ConnectableChannelModel>();
    }

    public class ConnectableChannelModel
    {
        public Guid Id { get; set; }
        public Guid UnificRootId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChannelTypeEnum ChannelType { get; set; }
        
        public OrganizationModel Organization { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChannelConnectionTypeEnum ConnectionType { get; set; }

        public string ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Dictionary<LanguageEnum, ConnectableChannelLvModel> LanguageVersions { get; set; } = new Dictionary<LanguageEnum, ConnectableChannelLvModel>();
        public bool IsASTIConnection { get; set; }
    }

    public class ConnectableChannelLvModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public PublishingStatus Status { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum Language { get; set; }

        public string Name { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public DateTime? ScheduledPublish { get; set; }
    }
}
