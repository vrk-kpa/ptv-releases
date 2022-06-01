using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Next.Organization;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Framework;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    internal interface IConnectionMapper
    {
        ConnectionModel Map(ServiceServiceChannel connectionSource, 
            ServiceChannelVersioned channelSource,
            ServiceVersioned serviceSource);
        List<ServiceChannelConnectionModel> Map(VmServiceChannel sourceVm);
    }

    [RegisterService(typeof(IConnectionMapper), RegisterType.Transient)]
    internal class ConnectionMapper: IConnectionMapper
    {
        private readonly ITypesCache typesCache;
        private readonly IOrganizationMapper organizationMapper;
        private readonly ILanguageCache languageCache;
        private readonly IServiceHourMapper serviceHourMapper;
        private readonly IAddressMapper addressMapper;
        private readonly IPhoneMapper phoneMapper;
        private readonly IWebPageMapper webPageMapper;
        private readonly IEmailMapper emailMapper;

        public ConnectionMapper(ITypesCache typesCache,
            ILanguageCache languageCache,
            IServiceHourMapper serviceHourMapper,
            IAddressMapper addressMapper,
            IOrganizationMapper organizationMapper,
            IPhoneMapper phoneMapper,
            IWebPageMapper webPageMapper,
            IEmailMapper emailMapper)
        {
            this.typesCache = typesCache;
            this.organizationMapper = organizationMapper;
            this.languageCache = languageCache;
            this.serviceHourMapper = serviceHourMapper;
            this.addressMapper = addressMapper;
            this.phoneMapper = phoneMapper;
            this.webPageMapper = webPageMapper;
            this.emailMapper = emailMapper;
        }

        public ConnectionModel Map(ServiceServiceChannel connectionSource, 
            ServiceChannelVersioned channelSource,
            ServiceVersioned serviceSource)
        {
            var languages = GetCommonLanguages(channelSource, serviceSource);
            var channelType = typesCache.GetByValue<ServiceChannelType>(channelSource.TypeId).ToEnum<ServiceChannelTypeEnum>();
            
            var c = new ConnectionModel();
            
            c.ServiceId = serviceSource.Id;
            c.ServiceChannelUnificRootId = connectionSource.ServiceChannelId;
            c.ServiceUnificRootId = connectionSource.ServiceId;
            c.ChannelType = channelType;
            c.IsASTIConnection = connectionSource.IsASTIConnection;
            c.ChannelOrderNumber = connectionSource.ChannelOrderNumber;
            c.ServiceOrderNumber = connectionSource.ServiceOrderNumber;
            c.Modified = connectionSource.Modified.ToUtcDateWithoutConversion();
            c.ModifiedBy = connectionSource.ModifiedBy;           
            c.ChargeType = connectionSource.ChargeTypeId.HasValue
                ? typesCache.GetByValue<ServiceChargeType>(connectionSource.ChargeTypeId.Value).ToEnum<ServiceChargeTypeEnum>()
                : (ServiceChargeTypeEnum?)null;

            var hours = connectionSource.ServiceServiceChannelServiceHours.Select(x => x.ServiceHours).ToList();
            c.OpeningHours = serviceHourMapper.Map(hours, languages);
            c.DigitalAuthorizations = connectionSource.ServiceServiceChannelDigitalAuthorizations.Select(x => x.DigitalAuthorizationId).ToList();
            c.Addresses = connectionSource.ServiceServiceChannelAddresses.Select(x => addressMapper.Map(x, languages)).ToList();
            c.Emails = emailMapper.MapEmails(connectionSource, languages);
            c.WebPages = webPageMapper.MapWebPages(connectionSource, languages);
            c.FaxNumbers = phoneMapper.MapFaxNumbers(connectionSource, languages);
            c.PhoneNumbers = phoneMapper.MapPhoneNumbers(connectionSource, languages);

            c.LanguageVersions = languages
                .Select(x => Map(connectionSource, x))
                .ToDictionary(x => x.Language);

            return c;
        }

        private List<Guid> GetCommonLanguages(ServiceChannelVersioned channelSource, ServiceVersioned serviceSource)
        {
            // channel languages: fi, sv
            // service languages: fi, en
            // Result becomes  :  fi

            var l1 = channelSource.LanguageAvailabilities.Select(x => x.LanguageId).ToList();
            var l2 = serviceSource.LanguageAvailabilities.Select(x => x.LanguageId).ToList();
            return l1.Intersect(l2).ToList();
        }

        private ConnectionLvModel Map(ServiceServiceChannel connectionSource, Guid languageId)
        {
            var lv = new ConnectionLvModel();
            
            lv.Language = Helpers.MapLanguage(languageCache, languageId);

            var descriptions = connectionSource.ServiceServiceChannelDescriptions.Where(x => x.LocalizationId == languageId).ToList();
            lv.Description = descriptions.FirstOrDefault(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()))?.Description;
            lv.Charge = Map(descriptions.FirstOrDefault(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString()))?.Description);                        
            return lv;
        }

        private ChargeModel Map(string additionalInfo)
        {
            return new ChargeModel
            {
                Info = additionalInfo
            };
        }

        public List<ServiceChannelConnectionModel> Map(VmServiceChannel sourceVm)
        {
            var connections = sourceVm.Connections != null ? sourceVm.Connections.ToList() : new List<VmChannelConnectionOutput>();
            return connections.Select(x => Map(x)).ToList();
        }

        private ServiceChannelConnectionModel Map(VmChannelConnectionOutput sourceVm)
        {
            var c = new ServiceChannelConnectionModel();
            c.ServiceId = sourceVm.Id;
            c.ServiceUnificRootId = sourceVm.UnificRootId;
            c.ServiceOrganization = organizationMapper.Map(sourceVm.OrganizationId.Value);
            c.LanguageVersions = sourceVm.LanguagesAvailabilities.Select(x => Map(sourceVm, x)).ToDictionary(x => x.Language);
            return c;
        }

        private ServiceChannelConnectionLvModel Map(VmChannelConnectionOutput sourceVm, VmLanguageAvailabilityInfo languageInfo)
        {
            var lv = new ServiceChannelConnectionLvModel();

            var languageId = languageInfo.LanguageId;
            var languageCode = languageCache.GetByValue(languageId);

            lv.Language = languageCode.ToEnum<LanguageEnum>();
            lv.ServiceName = sourceVm.Name.GetValueOrDefault(languageCode);            
            return lv;
        }
    }
}
