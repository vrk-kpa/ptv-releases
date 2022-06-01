using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class ServiceModel
    {
        public Guid? Id { get; set; }
        public List<string> TargetGroups { get; set; } = new List<string>();
        public List<Guid> LifeEvents { get; set; } = new List<Guid>();
        public List<Guid> IndustrialClasses { get; set; } = new List<Guid>();
        public List<Guid> ServiceClasses { get; set; } = new List<Guid>();
        public List<OntologyTermModel> OntologyTerms { get; set; } = new List<OntologyTermModel>();
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceFundingTypeEnum FundingType { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceTypeEnum ServiceType { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChargeTypeEnum? ChargeType { get; set; }
        
        public Dictionary<LanguageEnum, ServiceLanguageVersionModel> LanguageVersions { get; set; } = new Dictionary<LanguageEnum, ServiceLanguageVersionModel>();
        public List<string> Languages { get; set; } = new List<string>();
        public AreaInformationModel AreaInformation { get; set; }
        public OrganizationModel ResponsibleOrganization { get; set; }
        public List<OrganizationModel> OtherResponsibleOrganizations{ get; set; } = new List<OrganizationModel>();
        public Guid? UnificRootId { get; set; }
        public GeneralDescriptionModel GeneralDescription { get; set; }
        public List<OrganizationModel> SelfProducers { get; set; } = new List<OrganizationModel>();
        public List<OrganizationModel> PurchaseProducers { get; set; } = new List<OrganizationModel>();
        public List<OrganizationModel> OtherProducers { get; set; } = new List<OrganizationModel>();
        public List<ConnectableChannelModel> ConnectedChannels { get; set; } = new List<ConnectableChannelModel>();

        [JsonConverter(typeof(StringEnumConverter))]
        public VoucherTypeEnum VoucherType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PublishingStatus Status { get; set; }
        public string Version { get; set; }
        public OtherVersionModel OtherPublishedVersion { get; set; }
        public OtherVersionModel OtherModifiedVersion { get; set; }
        
        public List<LastTranslationModel> LastTranslations { get; set; }
    }
}
