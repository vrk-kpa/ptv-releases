using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Next.Organization;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Next.Model;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.ServiceChannels
{
    internal interface IGdServiceChannelMapper
    {
        List<GdServiceChannelModel> Map(List<ServiceChannelVersioned> channels);
    }

    [RegisterService(typeof(IGdServiceChannelMapper), RegisterType.Transient)]
    internal class GdServiceChannelMapper : IGdServiceChannelMapper
    {
        private readonly ITypesCache typesCache;
        private readonly IOrganizationMapper organizationMapper;
        private readonly ILanguageCache languageCache;

        public GdServiceChannelMapper(ITypesCache typesCache,
            ILanguageCache languageCache,
            IOrganizationMapper organizationMapper)
        {
            this.typesCache = typesCache;
            this.organizationMapper = organizationMapper;
            this.languageCache = languageCache;
        }

        public List<GdServiceChannelModel> Map(List<ServiceChannelVersioned> channels)
        {
            return channels.Select(x => Map(x)).ToList();
        }

        private GdServiceChannelModel Map(ServiceChannelVersioned source)
        {
            var result = new GdServiceChannelModel();
            result.Id = source.Id;
            result.UnificRootId = source.UnificRootId;
            result.ChannelType = typesCache.GetByValue<ServiceChannelType>(source.TypeId).ToEnum<ServiceChannelTypeEnum>();
            result.ConnectionType = typesCache.GetByValue<ServiceChannelConnectionType>(source.ConnectionTypeId).ToEnum<ServiceChannelConnectionTypeEnum>();
            result.Organization = organizationMapper.Map(source.OrganizationId);
            result.Modified = source.Modified.ToUtcDateWithoutConversion();
            result.ModifiedBy = source.ModifiedBy;
            result.LanguageVersions = source.LanguageAvailabilities.Select(x => Map(source, x)).ToDictionary(x => x.Language);
            return result;
        }

        private GdServiceChannelLvModel Map(ServiceChannelVersioned source, ServiceChannelLanguageAvailability languageInfo)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            var result = new GdServiceChannelLvModel();

            var languageId = languageInfo.LanguageId;
            var languageCode = languageCache.GetByValue(languageId);

            result.Language = languageCode.ToEnum<LanguageEnum>();
            result.Status = typesCache.GetByValue<PublishingStatusType>(languageInfo.StatusId).ToEnum<PublishingStatus>();
            result.Name = source.ServiceChannelNames.FirstOrDefault(x => x.LocalizationId == languageId && x.TypeId == nameTypeId)?.Name;
            result.Modified = languageInfo.Modified;
            result.ModifiedBy = languageInfo.ModifiedBy;
            result.ScheduledPublish = languageInfo.PublishAt;
            return result;
        }
    }
}
