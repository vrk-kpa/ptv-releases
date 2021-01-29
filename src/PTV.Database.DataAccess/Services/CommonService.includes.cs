/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    internal partial class CommonService
    {
        public void IncludeConnectionAddresses(IUnitOfWork unitOfWork, List<ServiceServiceChannel> connections)
        {
            var serviceIds = connections.Select(x => x.ServiceId).Distinct().ToList();
            var channelIds = connections.Select(x => x.ServiceChannelId).Distinct().ToList();
            var connectionAddressRepo = unitOfWork.CreateRepository<IServiceServiceChannelAddressRepository>();

            var addresses = connectionAddressRepo.All()
                .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNames)
                .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.AddressStreetNumber).ThenInclude(i => i.Coordinates)
                .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.Address).ThenInclude(i => i.ClsAddressPoints).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                .Include(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Where(x => serviceIds.Contains(x.ServiceId) && channelIds.Contains(x.ServiceChannelId))
                .Distinct()
                .AsEnumerable()
                .GroupBy(x => (x.ServiceId, x.ServiceChannelId))
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var connection in connections)
            {
                connection.ServiceServiceChannelAddresses = addresses.TryGetOrDefault(
                    (connection.ServiceId, connection.ServiceChannelId), new List<ServiceServiceChannelAddress>());
            }
        }

        public void IncludeConnectionCommonDetails(IUnitOfWork unitOfWork, List<ServiceServiceChannel> connections)
        {
            var serviceIds = connections.Select(x => x.ServiceId).Distinct().ToList();
            var channelIds = connections.Select(x => x.ServiceChannelId).Distinct().ToList();
            var connectionDescriptionRepo = unitOfWork.CreateRepository<IServiceServiceChannelDescriptionRepository>();
            var connectionDigitalAuthRepo = unitOfWork.CreateRepository<IServiceServiceChannelDigitalAuthorizationRepository>();
            var connectionServiceHoursRepo = unitOfWork.CreateRepository<IServiceServiceChannelServiceHoursRepository>();
            var connectionExtraTypesRepo = unitOfWork.CreateRepository<IServiceServiceChannelExtraTypeRepository>();
            
            var descriptions = connectionDescriptionRepo.All()
                .Where(x => serviceIds.Contains(x.ServiceId) && channelIds.Contains(x.ServiceChannelId))
                .Distinct()
                .AsEnumerable()
                .GroupBy(x => (x.ServiceId, x.ServiceChannelId))
                .ToDictionary(x => x.Key, x => x.ToList());
            
            var digitalAuthorizations = connectionDigitalAuthRepo.All()
                .Include(x => x.DigitalAuthorization).ThenInclude(x => x.Names)
                .Where(x => serviceIds.Contains(x.ServiceId) && channelIds.Contains(x.ServiceChannelId))
                .Distinct()
                .AsEnumerable()
                .GroupBy(x => (x.ServiceId, x.ServiceChannelId))
                .ToDictionary(x => x.Key, x => x.ToList());
            
            var serviceHours = connectionServiceHoursRepo.All()
                .Include(x => x.ServiceHours).ThenInclude(x => x.DailyOpeningTimes)
                .Include(x => x.ServiceHours).ThenInclude(x => x.AdditionalInformations)
                .Include(x => x.ServiceHours).ThenInclude(x => x.HolidayServiceHour).ThenInclude(x => x.Holiday).ThenInclude(x => x.Names)
                .Include(x => x.ServiceHours).ThenInclude(x => x.HolidayServiceHour).ThenInclude(x => x.Holiday).ThenInclude(x => x.HolidayDates)
                .Where(x => serviceIds.Contains(x.ServiceId) && channelIds.Contains(x.ServiceChannelId))
                .Distinct()
                .AsEnumerable()
                .GroupBy(x => (x.ServiceId, x.ServiceChannelId))
                .ToDictionary(x => x.Key, x => x.ToList());
            
            var extraTypes = connectionExtraTypesRepo.All()
                .Include(x => x.ExtraSubType).ThenInclude(x => x.Names)
                .Include(x => x.ServiceServiceChannelExtraTypeDescriptions)
                .Where(x => serviceIds.Contains(x.ServiceId) && channelIds.Contains(x.ServiceChannelId))
                .Distinct()
                .AsEnumerable()
                .GroupBy(x => (x.ServiceId, x.ServiceChannelId))
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var connection in connections)
            {
                connection.ServiceServiceChannelDescriptions = descriptions.TryGetOrDefault(
                    (connection.ServiceId, connection.ServiceChannelId), new List<ServiceServiceChannelDescription>());
                connection.ServiceServiceChannelDigitalAuthorizations = digitalAuthorizations.TryGetOrDefault(
                    (connection.ServiceId, connection.ServiceChannelId),
                    new List<ServiceServiceChannelDigitalAuthorization>());
                connection.ServiceServiceChannelServiceHours = serviceHours.TryGetOrDefault(
                    (connection.ServiceId, connection.ServiceChannelId), new List<ServiceServiceChannelServiceHours>());
                connection.ServiceServiceChannelExtraTypes = extraTypes.TryGetOrDefault(
                    (connection.ServiceId, connection.ServiceChannelId), new List<ServiceServiceChannelExtraType>());
            }
        }

        public void IncludeConnectionContactDetails(IUnitOfWork unitOfWork, List<ServiceServiceChannel> connections)
        {
            var serviceIds = connections.Select(x => x.ServiceId).Distinct().ToList();
            var channelIds = connections.Select(x => x.ServiceChannelId).Distinct().ToList();
            var connectionEmailRepo = unitOfWork.CreateRepository<IServiceServiceChannelEmailRepository>();
            var connectionWebPageRepo = unitOfWork.CreateRepository<IServiceServiceChannelWebPageRepository>();
            var connectionPhoneRepo = unitOfWork.CreateRepository<IServiceServiceChannelPhoneRepository>();
            
            var emails = connectionEmailRepo.All()
                .Include(x => x.Email)
                .Where(x => serviceIds.Contains(x.ServiceId) && channelIds.Contains(x.ServiceChannelId))
                .Distinct()
                .AsEnumerable()
                .GroupBy(x => (x.ServiceId, x.ServiceChannelId))
                .ToDictionary(x => x.Key, x => x.ToList());
            
            var webPages = connectionWebPageRepo.All()
                .Include(x => x.WebPage)
                .Where(x => serviceIds.Contains(x.ServiceId) && channelIds.Contains(x.ServiceChannelId))
                .Distinct()
                .AsEnumerable()
                .GroupBy(x => (x.ServiceId, x.ServiceChannelId))
                .ToDictionary(x => x.Key, x => x.ToList());
            
            var phones = connectionPhoneRepo.All()
                .Include(x => x.Phone).ThenInclude(x => x.PrefixNumber)
                .Where(x => serviceIds.Contains(x.ServiceId) && channelIds.Contains(x.ServiceChannelId))
                .Distinct()
                .AsEnumerable()
                .GroupBy(x => (x.ServiceId, x.ServiceChannelId))
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var connection in connections)
            {
                connection.ServiceServiceChannelEmails = emails.TryGetOrDefault(
                    (connection.ServiceId, connection.ServiceChannelId), new List<ServiceServiceChannelEmail>());
                connection.ServiceServiceChannelWebPages = webPages.TryGetOrDefault(
                    (connection.ServiceId, connection.ServiceChannelId), new List<ServiceServiceChannelWebPage>());
                connection.ServiceServiceChannelPhones = phones.TryGetOrDefault(
                    (connection.ServiceId, connection.ServiceChannelId), new List<ServiceServiceChannelPhone>());
            }
        }
    }
}
