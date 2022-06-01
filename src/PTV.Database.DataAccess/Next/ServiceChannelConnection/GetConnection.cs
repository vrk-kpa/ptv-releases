using PTV.Database.DataAccess.Interfaces.DbContext;
using System;
using PTV.Database.Model.Models;
using PTV.Database.DataAccess.Interfaces.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Framework;

namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    internal interface IGetConnection
    {
        ServiceServiceChannel GetWithAllDetails(IUnitOfWork unitOfWork, Guid serviceId, Guid serviceChannelId);
    }

    [RegisterService(typeof(IGetConnection), RegisterType.Transient)]
    internal class GetConnection: IGetConnection
    {
        public GetConnection()
        {
        }

        public ServiceServiceChannel GetWithAllDetails(IUnitOfWork unitOfWork, Guid serviceId, Guid serviceChannelId)
        {
            var connectionsRepository = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var connection = connectionsRepository.All()
                .FirstOrDefault(x => x.ServiceId == serviceId && x.ServiceChannelId == serviceChannelId);
            if (connection == null)
            {
                return null;
            }

            FillCommonDetails(unitOfWork, connection);
            FillContactDetails(unitOfWork, connection);
            FillServiceAddresses(unitOfWork, connection);

            return connection;
        }

        private void FillCommonDetails(IUnitOfWork unitOfWork, ServiceServiceChannel connection)
        {
            var connectionDescriptionRepo = unitOfWork.CreateRepository<IServiceServiceChannelDescriptionRepository>();
            var connectionDigitalAuthRepo = unitOfWork.CreateRepository<IServiceServiceChannelDigitalAuthorizationRepository>();
            var connectionServiceHoursRepo = unitOfWork.CreateRepository<IServiceServiceChannelServiceHoursRepository>();
            var connectionExtraTypesRepo = unitOfWork.CreateRepository<IServiceServiceChannelExtraTypeRepository>();
            var webPagesRepo = unitOfWork.CreateRepository<IServiceServiceChannelWebPageRepository>();
            var phoneRepository= unitOfWork.CreateRepository<IServiceServiceChannelPhoneRepository>();

            var descriptions = connectionDescriptionRepo.All()
                .Where(x => x.ServiceId == connection.ServiceId && x.ServiceChannelId == connection.ServiceChannelId)
                .ToList();

            var digitalAuthorizations = connectionDigitalAuthRepo.All()
                .Include(x => x.DigitalAuthorization)
                .ThenInclude(x => x.Names)
                .Where(x => x.ServiceId == connection.ServiceId && x.ServiceChannelId == connection.ServiceChannelId)
                .ToList();

            var serviceHours = connectionServiceHoursRepo.All()
                .Include(x => x.ServiceHours)
                .ThenInclude(x => x.DailyOpeningTimes)
                .Include(x => x.ServiceHours)
                .ThenInclude(x => x.AdditionalInformations)
                .Include(x => x.ServiceHours)
                .ThenInclude(x => x.HolidayServiceHour)
                .ThenInclude(x => x.Holiday)
                .Where(x => x.ServiceId == connection.ServiceId && x.ServiceChannelId == connection.ServiceChannelId)
                .ToList();

            var extraTypes = connectionExtraTypesRepo.All()
                .Include(x => x.ServiceServiceChannelExtraTypeDescriptions)
                .Where(x => x.ServiceId == connection.ServiceId && x.ServiceChannelId == connection.ServiceChannelId)
                .ToList();

            var webPages = webPagesRepo.All()
                .Include(x => x.WebPage)
                .Where(x => x.ServiceId == connection.ServiceId && x.ServiceChannelId == connection.ServiceChannelId)
                .ToList();

            var phones = phoneRepository.All()
                .Include(x => x.Phone)
                .ThenInclude(x => x.PrefixNumber)
                .ThenInclude(x => x.Country)
                .ThenInclude(x => x.CountryNames)
                .Include(x => x.Phone)
                .ThenInclude(x => x.ChargeType)
                .Where(x => x.ServiceId == connection.ServiceId && x.ServiceChannelId == connection.ServiceChannelId)
                .ToList();

            connection.ServiceServiceChannelDescriptions = descriptions;
            connection.ServiceServiceChannelDigitalAuthorizations = digitalAuthorizations;
            connection.ServiceServiceChannelServiceHours = serviceHours;
            connection.ServiceServiceChannelExtraTypes = extraTypes;
            connection.ServiceServiceChannelWebPages = webPages;
            connection.ServiceServiceChannelPhones = phones;
        }

        private void FillContactDetails(IUnitOfWork unitOfWork, ServiceServiceChannel connection)
        {
            var connectionEmailRepo = unitOfWork.CreateRepository<IServiceServiceChannelEmailRepository>();
            var connectionWebPageRepo = unitOfWork.CreateRepository<IServiceServiceChannelWebPageRepository>();
            var connectionPhoneRepo = unitOfWork.CreateRepository<IServiceServiceChannelPhoneRepository>();

            var emails = connectionEmailRepo.All()
                .Include(x => x.Email)
                .Where(x => x.ServiceId == connection.ServiceId && x.ServiceChannelId == connection.ServiceChannelId)
                .ToList();

            var webPages = connectionWebPageRepo.All()
                .Include(x => x.WebPage)
                .Include(x => x.Localization)
                .Where(x => x.ServiceId == connection.ServiceId && x.ServiceChannelId == connection.ServiceChannelId)
                .ToList();

            var phones = connectionPhoneRepo.All()
                .Include(x => x.Phone)
                .ThenInclude(x => x.PrefixNumber)
                .ThenInclude(x => x.Country)
                .ThenInclude(x => x.CountryNames)
                .Include(x => x.Phone)
                .Where(x => x.ServiceId == connection.ServiceId && x.ServiceChannelId == connection.ServiceChannelId)
                .ToList();

            connection.ServiceServiceChannelEmails = emails;
            connection.ServiceServiceChannelWebPages = webPages;
            connection.ServiceServiceChannelPhones = phones;
        }

        private void FillServiceAddresses(IUnitOfWork unitOfWork, ServiceServiceChannel connection)
        {
            var connectionAddressRepo = unitOfWork.CreateRepository<IServiceServiceChannelAddressRepository>();

            var addresses = connectionAddressRepo.All()
                .Include(x => x.Address).ThenInclude(x => x.AddressAdditionalInformations)
                .Include(x => x.Address).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                .Include(x => x.Address).ThenInclude(x => x.AddressForeigns).ThenInclude(x => x.ForeignTextNames)
                .Include(x => x.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(x => x.PostOfficeBoxNames)
                .Include(x => x.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(x => x.PostalCode).ThenInclude(x => x.PostalCodeNames)
                .Include(j => j.Address).ThenInclude(j => j.ClsAddressPoints).ThenInclude(j => j.AddressStreet).ThenInclude(j => j.StreetNames)
                .Include(x => x.Address).ThenInclude(x => x.ClsAddressPoints).ThenInclude(x => x.AddressStreet).ThenInclude(x => x.StreetNumbers).ThenInclude(x => x.PostalCode).ThenInclude(x => x.PostalCodeNames)
                .Include(j => j.Address).ThenInclude(j => j.ClsAddressPoints).ThenInclude(j => j.PostalCode).ThenInclude(j => j.PostalCodeNames)
                .Include(j => j.Address).ThenInclude(j => j.ClsAddressPoints).ThenInclude(j => j.Municipality).ThenInclude(j => j.MunicipalityNames)
                .Include(j => j.Address).ThenInclude(j => j.ClsAddressPoints).ThenInclude(j => j.AddressStreetNumber)
                .Where(x => x.ServiceId == connection.ServiceId && x.ServiceChannelId == connection.ServiceChannelId)
                .ToList();

            connection.ServiceServiceChannelAddresses = addresses;
        }
    }
}
