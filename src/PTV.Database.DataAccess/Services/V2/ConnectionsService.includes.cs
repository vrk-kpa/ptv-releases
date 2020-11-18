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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Database.DataAccess.Services.V2
{
    internal partial class ConnectionsService : ServiceBase, IConnectionsService, IConnectionsServiceInternal
    {
        private void IncludeServiceCommonDetails(IUnitOfWork unitOfWork, Dictionary<Guid, List<ServiceServiceChannel>> serviceConnections)
        {
            var connectionDescriptionRepo = unitOfWork.CreateRepository<IServiceServiceChannelDescriptionRepository>();
            var connectionDigitalAuthRepo = unitOfWork.CreateRepository<IServiceServiceChannelDigitalAuthorizationRepository>();
            var connectionServiceHoursRepo = unitOfWork.CreateRepository<IServiceServiceChannelServiceHoursRepository>();
            var connectionExtraTypesRepo = unitOfWork.CreateRepository<IServiceServiceChannelExtraTypeRepository>();

            var descriptions = connectionDescriptionRepo.All()
                .Where(x => serviceConnections.Keys.Contains(x.ServiceId))
                .ToList()
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.ServiceChannelId).ToDictionary(y => y.Key, y => y.ToList()));

            var digitalAuthorizations = connectionDigitalAuthRepo.All()
                .Include(x => x.DigitalAuthorization)
                .ThenInclude(x => x.Names)
                .Where(x => serviceConnections.Keys.Contains(x.ServiceId))
                .ToList()
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.ServiceChannelId).ToDictionary(y => y.Key, y => y.ToList()));

            var serviceHours = connectionServiceHoursRepo.All()
                .Include(x => x.ServiceHours)
                .ThenInclude(x => x.DailyOpeningTimes)
                .Include(x => x.ServiceHours)
                .ThenInclude(x => x.AdditionalInformations)
                .Include(x => x.ServiceHours)
                .ThenInclude(x => x.HolidayServiceHour)
                .Where(x => serviceConnections.Keys.Contains(x.ServiceId))
                .ToList()
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.ServiceChannelId).ToDictionary(y => y.Key, y => y.ToList()));

            var extraTypes = connectionExtraTypesRepo.All()
                .Include(x => x.ServiceServiceChannelExtraTypeDescriptions)
                .Where(x => serviceConnections.Keys.Contains(x.ServiceId))
                .ToList()
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.ServiceChannelId).ToDictionary(y => y.Key, y => y.ToList()));

            foreach (var connections in serviceConnections)
            {
                var descriptionGroup = descriptions.GetValueOrDefault(connections.Key);
                var digitalAuthGroup = digitalAuthorizations.GetValueOrDefault(connections.Key);
                var serviceHourGroup = serviceHours.GetValueOrDefault(connections.Key);
                var extraTypeGroup = extraTypes.GetValueOrDefault(connections.Key);

                foreach (var connection in connections.Value)
                {
                    connection.ServiceServiceChannelDescriptions =
                        descriptionGroup?.GetValueOrDefault(connection.ServiceChannelId) ?? new List<ServiceServiceChannelDescription>();
                    connection.ServiceServiceChannelDigitalAuthorizations =
                        digitalAuthGroup?.GetValueOrDefault(connection.ServiceChannelId) ?? new List<ServiceServiceChannelDigitalAuthorization>();
                    connection.ServiceServiceChannelServiceHours =
                        serviceHourGroup?.GetValueOrDefault(connection.ServiceChannelId) ?? new List<ServiceServiceChannelServiceHours>();
                    connection.ServiceServiceChannelExtraTypes =
                        extraTypeGroup?.GetValueOrDefault(connection.ServiceChannelId) ?? new List<ServiceServiceChannelExtraType>();
                }
            }
        }
        private void IncludeChannelCommonDetails(IUnitOfWork unitOfWork, Dictionary<Guid, List<ServiceServiceChannel>> channelConnections)
        {
            var connectionDescriptionRepo = unitOfWork.CreateRepository<IServiceServiceChannelDescriptionRepository>();
            var connectionDigitalAuthRepo = unitOfWork.CreateRepository<IServiceServiceChannelDigitalAuthorizationRepository>();
            var connectionServiceHoursRepo = unitOfWork.CreateRepository<IServiceServiceChannelServiceHoursRepository>();
            var connectionExtraTypesRepo = unitOfWork.CreateRepository<IServiceServiceChannelExtraTypeRepository>();

            var descriptions = connectionDescriptionRepo.All()
                .Where(x => channelConnections.Keys.Contains(x.ServiceChannelId))
                .ToList()
                .GroupBy(x => x.ServiceChannelId)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.ServiceId).ToDictionary(y => y.Key, y => y.ToList()));

            var digitalAuthorizations = connectionDigitalAuthRepo.All()
                .Include(x => x.DigitalAuthorization)
                .ThenInclude(x => x.Names)
                .Where(x => channelConnections.Keys.Contains(x.ServiceChannelId))
                .ToList()
                .GroupBy(x => x.ServiceChannelId)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.ServiceId).ToDictionary(y => y.Key, y => y.ToList()));

            var serviceHours = connectionServiceHoursRepo.All()
                .Include(x => x.ServiceHours)
                .ThenInclude(x => x.DailyOpeningTimes)
                .Include(x => x.ServiceHours)
                .ThenInclude(x => x.AdditionalInformations)
                .Include(x => x.ServiceHours)
                .ThenInclude(x => x.HolidayServiceHour)
                .Where(x => channelConnections.Keys.Contains(x.ServiceChannelId))
                .ToList()
                .GroupBy(x => x.ServiceChannelId)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.ServiceId).ToDictionary(y => y.Key, y => y.ToList()));

            var extraTypes = connectionExtraTypesRepo.All()
                .Include(x => x.ServiceServiceChannelExtraTypeDescriptions)
                .Where(x => channelConnections.Keys.Contains(x.ServiceChannelId))
                .ToList()
                .GroupBy(x => x.ServiceChannelId)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.ServiceId).ToDictionary(y => y.Key, y => y.ToList()));

            foreach (var connections in channelConnections)
            {
                var descriptionGroup = descriptions.GetValueOrDefault(connections.Key);
                var digitalAuthGroup = digitalAuthorizations.GetValueOrDefault(connections.Key);
                var serviceHourGroup = serviceHours.GetValueOrDefault(connections.Key);
                var extraTypeGroup = extraTypes.GetValueOrDefault(connections.Key);

                foreach (var connection in connections.Value)
                {
                    connection.ServiceServiceChannelDescriptions =
                        descriptionGroup?.GetValueOrDefault(connection.ServiceId) ?? new List<ServiceServiceChannelDescription>();
                    connection.ServiceServiceChannelDigitalAuthorizations =
                        digitalAuthGroup?.GetValueOrDefault(connection.ServiceId) ?? new List<ServiceServiceChannelDigitalAuthorization>();
                    connection.ServiceServiceChannelServiceHours =
                        serviceHourGroup?.GetValueOrDefault(connection.ServiceId) ?? new List<ServiceServiceChannelServiceHours>();
                    connection.ServiceServiceChannelExtraTypes =
                        extraTypeGroup?.GetValueOrDefault(connection.ServiceId) ?? new List<ServiceServiceChannelExtraType>();
                }
            }
        }

        private void IncludeServiceContactDetails(IUnitOfWork unitOfWork, Dictionary<Guid, List<ServiceServiceChannel>> serviceConnections)
        {
            var connectionEmailRepo = unitOfWork.CreateRepository<IServiceServiceChannelEmailRepository>();
            var connectionWebPageRepo = unitOfWork.CreateRepository<IServiceServiceChannelWebPageRepository>();
            var connectionPhoneRepo = unitOfWork.CreateRepository<IServiceServiceChannelPhoneRepository>();

            var emails = connectionEmailRepo.All()
                .Include(x => x.Email)
                .Where(x => serviceConnections.Keys.Contains(x.ServiceId))
                .ToList()
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(y => y.ServiceChannelId).ToDictionary(y => y.Key, y => y.ToList()));

            var webPages = connectionWebPageRepo.All()
                .Include(x => x.WebPage)
                .Include(x => x.Localization)
                .Where(x => serviceConnections.Keys.Contains(x.ServiceId))
                .ToList()
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(y => y.ServiceChannelId).ToDictionary(y => y.Key, y => y.ToList()));

            var phones = connectionPhoneRepo.All()
                .Include(x => x.Phone)
                .ThenInclude(x => x.PrefixNumber)
                .ThenInclude(x => x.Country)
                .ThenInclude(x => x.CountryNames)
                .Where(x => serviceConnections.Keys.Contains(x.ServiceId))
                .ToList()
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(y => y.ServiceChannelId).ToDictionary(y => y.Key, y => y.ToList()));

            foreach (var connections in serviceConnections)
            {
                var emailGroup = emails.GetValueOrDefault(connections.Key);
                var webPageGroup = webPages.GetValueOrDefault(connections.Key);
                var phoneGroup = phones.GetValueOrDefault(connections.Key);

                foreach (var connection in connections.Value)
                {
                    connection.ServiceServiceChannelEmails = emailGroup?.GetValueOrDefault(connection.ServiceChannelId) ?? new List<ServiceServiceChannelEmail>();
                    connection.ServiceServiceChannelWebPages =
                        webPageGroup?.GetValueOrDefault(connection.ServiceChannelId) ?? new List<ServiceServiceChannelWebPage>();
                    connection.ServiceServiceChannelPhones = phoneGroup?.GetValueOrDefault(connection.ServiceChannelId) ?? new List<ServiceServiceChannelPhone>();
                }
            }
        }

        private void IncludeChannelContactDetails(IUnitOfWork unitOfWork, Dictionary<Guid, List<ServiceServiceChannel>> channelConnections)
        {
            var connectionEmailRepo = unitOfWork.CreateRepository<IServiceServiceChannelEmailRepository>();
            var connectionWebPageRepo = unitOfWork.CreateRepository<IServiceServiceChannelWebPageRepository>();
            var connectionPhoneRepo = unitOfWork.CreateRepository<IServiceServiceChannelPhoneRepository>();

            var emails = connectionEmailRepo.All()
                .Include(x => x.Email)
                .Where(x => channelConnections.Keys.Contains(x.ServiceChannelId))
                .ToList()
                .GroupBy(x => x.ServiceChannelId)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(y => y.ServiceId).ToDictionary(y => y.Key, y => y.ToList()));

            var webPages = connectionWebPageRepo.All()
                .Include(x => x.WebPage)
                .Include(x => x.Localization)
                .Where(x => channelConnections.Keys.Contains(x.ServiceChannelId))
                .ToList()
                .GroupBy(x => x.ServiceChannelId)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(y => y.ServiceId).ToDictionary(y => y.Key, y => y.ToList()));

            var phones = connectionPhoneRepo.All()
                .Include(x => x.Phone)
                .ThenInclude(x => x.PrefixNumber)
                .ThenInclude(x => x.Country)
                .ThenInclude(x => x.CountryNames)
                .Where(x => channelConnections.Keys.Contains(x.ServiceChannelId))
                .ToList()
                .GroupBy(x => x.ServiceChannelId)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(y => y.ServiceId).ToDictionary(y => y.Key, y => y.ToList()));

            foreach (var connections in channelConnections)
            {
                var emailGroup = emails.GetValueOrDefault(connections.Key);
                var webPageGroup = webPages.GetValueOrDefault(connections.Key);
                var phoneGroup = phones.GetValueOrDefault(connections.Key);

                foreach (var connection in connections.Value)
                {
                    connection.ServiceServiceChannelEmails = emailGroup?.GetValueOrDefault(connection.ServiceId) ?? new List<ServiceServiceChannelEmail>();
                    connection.ServiceServiceChannelWebPages =
                        webPageGroup?.GetValueOrDefault(connection.ServiceId) ?? new List<ServiceServiceChannelWebPage>();
                    connection.ServiceServiceChannelPhones = phoneGroup?.GetValueOrDefault(connection.ServiceId) ?? new List<ServiceServiceChannelPhone>();
                }
            }
        }

        private void IncludeServiceAddresses(IUnitOfWork unitOfWork, Dictionary<Guid, List<ServiceServiceChannel>> serviceConnections)
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
                .Where(x => serviceConnections.Keys.Contains(x.ServiceId))
                .ToList()
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(y => y.ServiceChannelId).ToDictionary(y => y.Key, y => y.ToList()));

            foreach (var connections in serviceConnections)
            {
                var addressGroup = addresses.GetValueOrDefault(connections.Key);

                foreach (var connection in connections.Value)
                {
                    connection.ServiceServiceChannelAddresses =
                        addressGroup?.GetValueOrDefault(connection.ServiceChannelId) ?? new List<ServiceServiceChannelAddress>();
                }
            }
        }

        private void IncludeChannelAddresses(IUnitOfWork unitOfWork, Dictionary<Guid, List<ServiceServiceChannel>> channelConnections)
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
                .Where(x => channelConnections.Keys.Contains(x.ServiceChannelId))
                .ToList()
                .GroupBy(x => x.ServiceChannelId)
                .ToDictionary(x => x.Key,
                    x => x.GroupBy(y => y.ServiceId).ToDictionary(y => y.Key, y => y.ToList()));

            foreach (var connections in channelConnections)
            {
                var addressGroup = addresses.GetValueOrDefault(connections.Key);

                foreach (var connection in connections.Value)
                {
                    connection.ServiceServiceChannelAddresses =
                        addressGroup?.GetValueOrDefault(connection.ServiceId) ?? new List<ServiceServiceChannelAddress>();
                }
            }
        }
    }
}
