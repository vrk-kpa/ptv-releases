using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Next.Organization;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;
using PTV.Framework;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.ServiceChannels
{
    internal interface ILegacyServiceChannelMapper
    {
        VmConnectableChannelSearch Map(ConnectableChannelSearchModel source);
        ConnectableChannelSearchResultModel Map(VmConnectableChannelSearchResult source);
        ServiceChannelModel Map(VmElectronicChannel sourceVm, ServiceChannelVersioned source);
        ServiceChannelModel Map(VmPhoneChannel sourceVm, ServiceChannelVersioned source);
        ServiceChannelModel Map(VmPrintableForm sourceVm, ServiceChannelVersioned source);
        ServiceChannelModel Map(VmServiceLocationChannel sourceVm, ServiceChannelVersioned source);
        ServiceChannelModel Map(VmWebPageChannel sourceVm, ServiceChannelVersioned source);
    }

    [RegisterService(typeof(ILegacyServiceChannelMapper), RegisterType.Transient)]
    internal class LegacyServiceChannelMapper : ILegacyServiceChannelMapper
    {
        private readonly ITypesCache typesCache;
        private readonly IOrganizationMapper organizationMapper;
        private readonly ILanguageCache languageCache;
        private readonly ServiceChannelConnection.IConnectionMapper connectionMapper;

        public LegacyServiceChannelMapper(ITypesCache typesCache,
            ILanguageCache languageCache,
            ServiceChannelConnection.IConnectionMapper connectionMapper,
            IOrganizationMapper organizationMapper)
        {
            this.typesCache = typesCache;
            this.organizationMapper = organizationMapper;
            this.connectionMapper = connectionMapper;
            this.languageCache = languageCache;
        }

        private List<VmSortParam> MapSortData(List<VmSortParam> source)
        {
            if (source == null || !source.Any())
            {
                return new List<VmSortParam>
                {
                    // Default sorting
                    new VmSortParam {Column = "Modified", Order = 1, SortDirection = SortDirectionEnum.Asc}
                };
            }

            return source;
        }

        public VmConnectableChannelSearch Map(ConnectableChannelSearchModel source)
        {
            var vm = new VmConnectableChannelSearch();
            vm.Language = source.Language;
            vm.Id = source.Id;
            vm.Type = source.Type;
            vm.OrganizationId = source.OrganizationId;
            vm.Name = source.Name;
            vm.PageNumber = source.PageNumber;
            vm.PageSize = source.PageSize;
            vm.SortData = MapSortData(source.SortData);
            vm.ChannelType = source.ChannelType.HasValue ? source.ChannelType.ToString() : null;
            return vm;
        }

        public ConnectableChannelSearchResultModel Map(VmConnectableChannelSearchResult source)
        {
            var result = new ConnectableChannelSearchResultModel();
            result.Count = source.Count;
            result.MaxPageCount = source.MaxPageCount;
            result.PageSize = source.PageSize;
            result.MoreAvailable = source.MoreAvailable;
            result.PageNumber = source.PageNumber;
            result.Skip = source.Skip;
            result.Items = source.SearchResult.Select(x => Map(x)).ToList();
            return result;
        }

        public ServiceChannelModel Map(VmElectronicChannel sourceVm, ServiceChannelVersioned source)
        {
            var channel = new ServiceChannelModel();
            FillCommonInformation(sourceVm, source, channel);
            channel.LanguageVersions = sourceVm.LanguagesAvailabilities.Select(x => Map(sourceVm, source, x)).ToDictionary(x => x.Language);
            channel.OnlineAuthenticationRequired = sourceVm.IsOnLineAuthentication.Value;
            channel.ElectronicSigningRequired = sourceVm.IsOnlineSign;
            channel.NumberOfSignaturesRequired = sourceVm.SignatureCount.HasValue ? sourceVm.SignatureCount.Value : 0;
            return channel;
        }

        public ServiceChannelModel Map(VmPhoneChannel sourceVm, ServiceChannelVersioned source)
        {
            var channel = new ServiceChannelModel();
            FillCommonInformation(sourceVm, source, channel);
            channel.LanguageVersions = sourceVm.LanguagesAvailabilities.Select(x => Map(sourceVm, source, x)).ToDictionary(x => x.Language);
            return channel;
        }

        public ServiceChannelModel Map(VmPrintableForm sourceVm, ServiceChannelVersioned source)
        {
            var channel = new ServiceChannelModel();
            FillCommonInformation(sourceVm, source, channel);
            channel.LanguageVersions = sourceVm.LanguagesAvailabilities.Select(x => Map(sourceVm, source, x)).ToDictionary(x => x.Language);
            return channel;
        }

        public ServiceChannelModel Map(VmServiceLocationChannel sourceVm, ServiceChannelVersioned source)
        {
            var channel = new ServiceChannelModel();
            FillCommonInformation(sourceVm, source, channel);
            channel.LanguageVersions = sourceVm.LanguagesAvailabilities.Select(x => Map(sourceVm, source, x)).ToDictionary(x => x.Language);
            return channel;
        }

        public ServiceChannelModel Map(VmWebPageChannel sourceVm, ServiceChannelVersioned source)
        {
            var channel = new ServiceChannelModel();
            FillCommonInformation(sourceVm, source, channel);
            channel.LanguageVersions = sourceVm.LanguagesAvailabilities.Select(x => Map(sourceVm, source, x)).ToDictionary(x => x.Language);
            return channel;
        }

        private void FillCommonInformation(VmServiceChannel sourceVm,
            ServiceChannelVersioned source,
            ServiceChannelModel channel)
        {
            channel.Id = source.Id;
            channel.UnificRootId = source.UnificRootId;
            channel.Modified = source.Modified.ToUtcDateWithoutConversion();
            channel.ModifiedBy = source.ModifiedBy;
            channel.ChannelType = typesCache.GetByValue<ServiceChannelType>(source.TypeId).ToEnum<ServiceChannelTypeEnum>();
            channel.ConnectionType = typesCache.GetByValue<ServiceChannelConnectionType>(source.ConnectionTypeId).ToEnum<ServiceChannelConnectionTypeEnum>();
            channel.Organization = organizationMapper.Map(sourceVm.OrganizationId);
            channel.BusinessRegions = sourceVm.AreaInformation != null ? sourceVm.AreaInformation.BusinessRegions : new List<Guid>();
            channel.HospitalRegions = sourceVm.AreaInformation != null ? sourceVm.AreaInformation.HospitalRegions : new List<Guid>();
            channel.Provinces = sourceVm.AreaInformation != null ? sourceVm.AreaInformation.Provinces : new List<Guid>();
            channel.Municipalities = sourceVm.AreaInformation != null ? sourceVm.AreaInformation.Municipalities : new List<Guid>();
            channel.Languages = sourceVm.Languages.Select(x => languageCache.GetByValue(x)).ToList();
            channel.Connections = connectionMapper.Map(sourceVm);
        }

        private void FillCommonInformation(VmServiceChannel sourceVm, 
            VmLanguageAvailabilityInfo languageInfo,
            ServiceChannelLvModel target)
        {
            var languageId = languageInfo.LanguageId;
            var languageCode = languageCache.GetByValue(languageId);

            target.Status = typesCache.GetByValue<PublishingStatusType>(languageInfo.StatusId).ToEnum<PublishingStatus>();
            target.Language = languageCode.ToEnum<LanguageEnum>();
            target.Name = sourceVm.Name.GetValueOrDefault(languageCode);
            target.AlternativeName = sourceVm.AlternateName.GetValueOrDefault(languageCode);
            target.Description = sourceVm.Description.GetValueOrDefault(languageCode);
            target.ShortDescription = sourceVm.ShortDescription.GetValueOrDefault(languageCode);
            target.ScheduledPublish = languageInfo.ValidFrom?.FromEpochTime();
            // TODO
            // PhoneNumbers
            // Emails
            // Connections
        }

        private ServiceChannelLvModel Map(VmElectronicChannel sourceVm, 
            ServiceChannelVersioned source,
            VmLanguageAvailabilityInfo languageInfo)
        {
            var languageId = languageInfo.LanguageId;
            var languageCode = languageCache.GetByValue(languageId);

            var lv = new ServiceChannelLvModel();
            FillCommonInformation(sourceVm, languageInfo, lv);
            lv.Website = sourceVm.WebPage.GetValueOrDefault(languageCode);
            return lv;
        }

        private ServiceChannelLvModel Map(VmWebPageChannel sourceVm, 
            ServiceChannelVersioned source,
            VmLanguageAvailabilityInfo languageInfo)
        {
            var languageId = languageInfo.LanguageId;
            var languageCode = languageCache.GetByValue(languageId);

            var lv = new ServiceChannelLvModel();
            FillCommonInformation(sourceVm, languageInfo, lv);
            lv.Website = sourceVm.WebPage.GetValueOrDefault(languageCode);
            return lv;
        }

        private ServiceChannelLvModel Map(VmPhoneChannel sourceVm, 
            ServiceChannelVersioned source,
            VmLanguageAvailabilityInfo languageInfo)
        {
            var lv = new ServiceChannelLvModel();
            FillCommonInformation(sourceVm, languageInfo, lv);            
            return lv;
        }

        private ServiceChannelLvModel Map(VmPrintableForm sourceVm, 
            ServiceChannelVersioned source,
            VmLanguageAvailabilityInfo languageInfo)
        {
            var languageId = languageInfo.LanguageId;
            var languageCode = languageCache.GetByValue(languageId);

            var lv = new ServiceChannelLvModel();
            FillCommonInformation(sourceVm, languageInfo, lv);
            lv.FormIdentifier = sourceVm.FormIdentifier.GetValueOrDefault(languageCode);
            return lv;
        }
        private ServiceChannelLvModel Map(VmServiceLocationChannel sourceVm, 
            ServiceChannelVersioned source,
            VmLanguageAvailabilityInfo languageInfo)
        {
            var lv = new ServiceChannelLvModel();
            FillCommonInformation(sourceVm, languageInfo, lv);
            return lv;
        }

        private ConnectableChannelModel Map(VmConnectableChannel source)
        {
            var result = new ConnectableChannelModel();
            result.Id = source.Id;
            result.UnificRootId = source.UnificRootId;            
            result.ModifiedBy = source.ModifiedBy;
            result.Modified = source.Modified.FromEpochTime().ToUtcDateWithoutConversion();
            result.Organization = organizationMapper.Map(source.OrganizationId.Value);
            result.ConnectionType = typesCache.GetByValue<ServiceChannelConnectionType>(source.ConnectionTypeId.Value).ToEnum<ServiceChannelConnectionTypeEnum>();
            result.ChannelType = typesCache.GetByValue<ServiceChannelType>(source.ChannelTypeId.Value).ToEnum<ServiceChannelTypeEnum>();
            result.LanguageVersions = source.LanguagesAvailabilities.Select(x => Map(source, x)).ToDictionary(x => x.Language);
            
            return result;
        }

        private ConnectableChannelLvModel Map(VmConnectableChannel source, VmLanguageAvailabilityInfo languageInfo)
        {
            var result = new ConnectableChannelLvModel();

            var languageId = languageInfo.LanguageId;
            var languageCode = languageCache.GetByValue(languageId);

            result.Language = languageCode.ToEnum<LanguageEnum>();
            result.Status = typesCache.GetByValue<PublishingStatusType>(languageInfo.StatusId).ToEnum<PublishingStatus>();
            result.Name = source.Name.GetValueOrDefault(languageCode);

            result.Modified = languageInfo.Modified.FromEpochTime();
            result.ModifiedBy = languageInfo.ModifiedBy;
            result.ScheduledPublish = languageInfo.ValidFrom?.FromEpochTime();

            return result;
        }
    }
}
