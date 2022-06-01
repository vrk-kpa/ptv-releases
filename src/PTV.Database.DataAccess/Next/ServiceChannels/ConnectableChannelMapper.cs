using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Next.Organization;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Next.Model;
using System.Linq;

namespace PTV.Database.DataAccess.Next.ServiceChannels
{
    internal interface IConnectableChannelMapper
    {
        ConnectableChannelModel Map(ServiceChannelVersioned channel, ServiceServiceChannel connection);
    }

    [RegisterService(typeof(IConnectableChannelMapper), RegisterType.Transient)]
    internal class ConnectableChannelMapper: IConnectableChannelMapper
    {
        private readonly ITypesCache typesCache;
        private readonly IOrganizationMapper organizationMapper;
        private readonly ILanguageCache languageCache;

        public ConnectableChannelMapper(ITypesCache typesCache,
            ILanguageCache languageCache,
            IOrganizationMapper organizationMapper)
        {
            this.typesCache = typesCache;
            this.organizationMapper = organizationMapper;
            this.languageCache = languageCache;
        }

        public ConnectableChannelModel Map(ServiceChannelVersioned channel, ServiceServiceChannel connection)
        {
            var result = new ConnectableChannelModel();
            result.Id = channel.Id;
            result.IsASTIConnection = connection.IsASTIConnection;
            result.UnificRootId = channel.UnificRootId;
            result.ChannelType = typesCache.GetByValue<ServiceChannelType>(channel.TypeId).ToEnum<ServiceChannelTypeEnum>();
            result.ConnectionType = typesCache.GetByValue<ServiceChannelConnectionType>(channel.ConnectionTypeId).ToEnum<ServiceChannelConnectionTypeEnum>();
            result.Organization = organizationMapper.Map(channel.OrganizationId);
            result.Modified = channel.Modified.ToUtcDateWithoutConversion();
            result.ModifiedBy = channel.ModifiedBy;
            result.LanguageVersions = channel.LanguageAvailabilities.Select(x => Map(channel, x)).ToDictionary(x => x.Language);
            return result;
        }

        private ConnectableChannelLvModel Map(ServiceChannelVersioned source, ServiceChannelLanguageAvailability languageInfo)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            var result = new ConnectableChannelLvModel();

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
